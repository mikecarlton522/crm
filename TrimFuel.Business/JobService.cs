using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using TrimFuel.Business.Gateways;
using TrimFuel.Business.Gateways.DefaultEmail;
using TrimFuel.Model.Enums;
using TrimFuel.Model.Views;
using TrimFuel.Business.Flow;

namespace TrimFuel.Business
{
    public class JobService : BaseService
    {
        //TODO: config
        private const string ROB_EMAIL = "rob.wertheim@gmail.com";
        private const string VOIDQUEUE_ERROR_EMAIL_FROM_ADDRESS = "dtcoder@gmail.com";
        private const string VOIDQUEUE_ERROR_EMAIL_SUBJECT = "VoidQueue({0}) processing error";
        private const string VOIDQUEUE_ERROR_EMAIL_BODY = "Error details:<br/>VoidQueue({0})<br/>BillingID({1})<br/>SaleID({2})<br/>Refund Amount({3})<br/><br/>NMI Response:<br/>{4}";

        private SaleService saleService { get { return new SaleService(); } }
        private EmailService emailService { get { return new EmailService(); } }
        private SubscriptionService subscriptionService { get { return new SubscriptionService(); } }
        private IEmailGateway emailGateway = new DefaultEmailGateway();

        #region VoidQueue processing

        public void ProcessVoidQueueReadyForRefund()
        {
            try
            {
                IList<VoidQueue> voidList = GetVoidQueueListReadyForRefund();
                if (voidList == null)
                {
                    throw new Exception("Can't load VoidQueue list");
                }

                foreach (VoidQueue voidQueue in voidList)
                {
                    ProcessVoidQueue(voidQueue);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        private bool ProcessVoidQueue(VoidQueue voidQueue)
        {
            bool res = false;

            try
            {
                Sale sl = EnsureLoad<Sale>(voidQueue.SaleID.Value);

                //Set Completed = true in any case
                voidQueue.Completed = true;
                dao.Save<VoidQueue>(voidQueue);

                string response = null;
                if (sl.SaleTypeID == SaleTypeEnum.OrderSale)
                {
                    var res2 = (new SaleFlow()).ProcessRefundOrVoid(sl.SaleID.Value, voidQueue.Amount.Value);
                    if (res2.State == BusinessErrorState.Success)
                        res = true;
                    if (res2.ReturnValue != null)
                        response = res2.ReturnValue.ChargeHistory.Response;
                }
                else
                {
                    ChargeHistoryEx chargeHistory = saleService.GetChargeHistoryBySale(voidQueue.SaleID.Value);
                    if (chargeHistory == null)
                    {
                        throw new Exception(string.Format("ChargeHistoryEx was not found for Sale({0})", voidQueue.SaleID.Value));
                    }

                    var res3 = saleService.DoRefundOrVoid(chargeHistory, voidQueue.Amount.Value);
                    if (res3.State == BusinessErrorState.Success)
                        res = true;
                    if (res3.ReturnValue != null)
                        response = res3.ReturnValue.Response;
                }

                if (res != true)
                {
                    SendInvalidProcessVoidQueueEmail(voidQueue, response);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }

            return res;
        }

        private void SendInvalidProcessVoidQueueEmail(VoidQueue voidQueue, string error)
        {
            try
            {
                string subject = string.Format(VOIDQUEUE_ERROR_EMAIL_SUBJECT, voidQueue.VoidQueueID);
                string body = string.Format(VOIDQUEUE_ERROR_EMAIL_BODY, voidQueue.VoidQueueID, voidQueue.BillingID, 
                    voidQueue.SaleID, voidQueue.Amount, error);
                emailGateway.SendEmail(string.Empty, VOIDQUEUE_ERROR_EMAIL_FROM_ADDRESS, string.Empty, ROB_EMAIL, subject, body);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        private IList<VoidQueue> GetVoidQueueListReadyForRefund()
        {
            IList<VoidQueue> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select vq.* from VoidQueue vq " +
                    "where vq.Completed = 0 and vq.SaleChargeDT < @nowMinus24hours");
                q.Parameters.Add("@nowMinus24hours", MySqlDbType.Timestamp).Value = DateTime.Now.AddHours(-24);

                res = dao.Load<VoidQueue>(q);
            }
            catch (Exception ex)
            {
                res = null;
                logger.Error(GetType(), ex);
            }
            return res;
        }

        #endregion

        public void SendRefererPromotion1BatchEmail()
        {
            try
            {
                MySqlCommand q = new MySqlCommand("select r.* from Referer r " +
                    "where r.Username not in " +
                    "( " +
                    "select e.Email from Email e " +
                    "inner join DynamicEmail de on de.DynamicEmailID = e.DynamicEmailID  " +
                    "where de.DynamicEmailTypeID = @dynamicEmailTypeRefererPromotion1 " +
                    ")");
                q.Parameters.Add("@dynamicEmailTypeRefererPromotion1", MySqlDbType.Int32).Value = DynamicEmailTypeEnum.RefererPromotion1;
                
                IList<Referer> referers = dao.Load<Referer>(q);

                foreach (Referer referer in referers)
                {
                    emailService.SendRefererPromotion1(referer, ProductEnum.ECigarettes);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public void SendAbandonEmails()
        {
            try
            {
                IList<DynamicEmail> dynamicEmailList = emailService.GetActiveDynamicEmails(TrimFuel.Model.Enums.DynamicEmailTypeEnum.Abandons);
                foreach (var item in dynamicEmailList)
                {
                    SendAbandonEmails(item);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public void SendAbandonEmails(DynamicEmail dynamicEmail)
        {
            try
            {
                DateTime endDate = DateTime.Now.AddHours(-dynamicEmail.Days.Value);
                DateTime startDate = endDate.AddHours(-1);
                IList<Registration> abandons = subscriptionService.GetEmailAbandons(startDate, endDate, dynamicEmail.ProductID.Value);
                foreach (var item in abandons)
                {
                    if (!ExistsAbandonEmail(dynamicEmail, item))
                    {
                        emailService.SendAbandonEmail(item, dynamicEmail);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public bool ExistsAbandonEmail(DynamicEmail dynamicEmail, Registration registration)
        {
            bool res = true;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select count(*) as Value from Email e
                    where e.DynamicEmailID = @dynamicEmailID
                    and e.Email = @email
                ");
                q.Parameters.Add("@dynamicEmailID", MySqlDbType.Int32).Value = dynamicEmail.DynamicEmailID;
                q.Parameters.Add("@email", MySqlDbType.VarChar).Value = registration.Email;

                View<int> resView = dao.Load<View<int>>(q).FirstOrDefault();
                if (resView.Value.Value == 0)
                {
                    res = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public void SendNewsletterEmails()
        {
            try
            {
                IList<DynamicEmail> dynamicEmailList = emailService.GetActiveDynamicEmails(TrimFuel.Model.Enums.DynamicEmailTypeEnum.Newsletter);
                foreach (var item in dynamicEmailList)
                {
                    SendNewsletterEmails(item);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public void SendNewsletterEmails(DynamicEmail dynamicEmail)
        {
            try
            {
                DateTime endDate = DateTime.Now.AddHours(-dynamicEmail.Days.Value);
                DateTime startDate = endDate.AddHours(-1);
                IList<Registration> abandons = subscriptionService.GetEmailNewsletters(startDate, endDate, dynamicEmail.ProductID.Value);
                foreach (var item in abandons)
                {
                    if (!ExistsNewsletterEmail(dynamicEmail, item))
                    {
                        emailService.SendNewsletterEmail(item, dynamicEmail);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public bool ExistsNewsletterEmail(DynamicEmail dynamicEmail, Registration registration)
        {
            bool res = true;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select count(*) as Value from Email e
                    where e.DynamicEmailID = @dynamicEmailID
                    and e.Email = @email
                ");
                q.Parameters.Add("@dynamicEmailID", MySqlDbType.Int32).Value = dynamicEmail.DynamicEmailID;
                q.Parameters.Add("@email", MySqlDbType.VarChar).Value = registration.Email;

                View<int> resView = dao.Load<View<int>>(q).FirstOrDefault();
                if (resView.Value.Value == 0)
                {
                    res = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }
    }
}
