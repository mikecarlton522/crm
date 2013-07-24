using System;
using System.Collections.Generic;
using System.Linq;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Enums;
using TrimFuel.Business.Flow;

namespace TrimFuel.Business
{
    public class ReturnSaleService : SaleService
    {
        public BusinessError<bool> ReturnSale(long saleID, string reason, DateTime? returnDate, int adminID)
        {
            BusinessError<bool> res = new BusinessError<bool>(false, BusinessErrorState.Error, "Unknown error occurred");
            try
            {
                Sale sale = Load<Sale>(saleID);
                if (sale.SaleTypeID == SaleTypeEnum.OrderSale)
                {
                    var res2 = (new SaleFlow()).ReturnSale(saleID, reason, returnDate, adminID);
                    res.State = res2.State;
                    res.ErrorMessage = res2.ErrorMessage;
                    res.ReturnValue = (res.State == BusinessErrorState.Success);
                }
                else
                {
                    if (IsSaleReturned(saleID).Value)
                    {
                        res.ErrorMessage = "Sale is alredy returned";
                    }
                    else
                    {
                        var billing = GetBillingBySale(saleID);
                        if (CreateReturnSale(saleID, billing.BillingID.Value, reason, returnDate, adminID) != null)
                        {
                            //Notes from actionRes could be added to billing Notes
                            BusinessError<bool> actionRes = ProcessReturnAction(billing, saleID);

                            res.ReturnValue = true;
                            res.State = BusinessErrorState.Success;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public ReturnedSale CreateReturnSale(long saleID, long billingID, string reason, DateTime? returnDate, int adminID)
        {
            ReturnedSale res = null;
            try
            {
                res = new ReturnedSale()
                {
                    SaleID = saleID,
                    ReturnDate = returnDate ?? DateTime.Now,
                    Reason = reason
                };
                dao.Save<ReturnedSale>(res);

                string noteContent = "Sale (" + saleID + ") returned. " + reason;
                Notes noteToInsert = new Notes()
                {
                    AdminID = adminID,
                    BillingID = (int)billingID,
                    CreateDT = DateTime.Now,
                    Content = noteContent
                };
                dao.Save<Notes>(noteToInsert);

            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public bool? IsSaleReturned(long saleID)
        {
            bool? res = false;
            try
            {
                MySqlCommand q = new MySqlCommand("select * from ReturnedSale where SaleID = @saleID");
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;
                if (dao.Load<ReturnedSale>(q).Count > 0)
                {
                    res = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<Notes> GetAdminNotesByBillingID(long billingID)
        {
            IList<Notes> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("Select * From Notes Where BillingID=@billingID and AdminID<>0");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                res = dao.Load<Notes>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        private BusinessError<bool> ProcessReturnAction(Billing billing, long saleID)
        {
            BusinessError<bool> res = new BusinessError<bool>(true, BusinessErrorState.Success, string.Empty);
            MySqlCommand q = null;

            try
            {
                q = new MySqlCommand(@"
                    select rp.*, rpa.Name from SaleReturnProcessing rp
                    inner join ReturnProcessingAction rpa on rpa.ReturnProcessingActionID = rp.ReturnProcessingActionID
                    where rp.SaleID=@saleID
                ");
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;
                var returnedSaleView = dao.Load<ReturnedSaleView>(q).SingleOrDefault();

                if ((returnedSaleView != null) && (returnedSaleView.ReturnProcessingActionID != null))
                {
                    switch (returnedSaleView.ReturnProcessingActionID.Value)
                    {
                        case TrimFuel.Model.Enums.ReturnProcessingActionEnum.ISSUE_REFUND:
                            {
                                var history = GetChargeHistoryBySale(saleID);
                                if (history == null)
                                {
                                    res = new BusinessError<bool>(false, BusinessErrorState.Error, "Refund failed. Charge was not found.");
                                }
                                else if (returnedSaleView.RefundAmount == null || returnedSaleView.RefundAmount.Value <= 0)
                                {
                                    res = new BusinessError<bool>(false, BusinessErrorState.Error, "Refund failed. Invalid refund amount.");
                                }
                                else
                                {
                                    DoRefund(history, returnedSaleView.RefundAmount.Value);
                                }
                                break;
                            }
                        case TrimFuel.Model.Enums.ReturnProcessingActionEnum.CANCEL_ACCOUNT:
                            {
                                BillingSubscription billingSubscription = GetBillingSubscriptionBySale(saleID);
                                if (billingSubscription == null)
                                {
                                    res = new BusinessError<bool>(false, BusinessErrorState.Error, "Cancel subscription failed. Can't find subscription.");
                                }
                                else
                                {
                                    subscriptionService.CancelSubscription(billingSubscription);
                                }
                                break;
                            }
                        case TrimFuel.Model.Enums.ReturnProcessingActionEnum.CHANGE_PLAN:
                            {
                                BillingSubscription billingSubscription = GetBillingSubscriptionBySale(saleID);
                                if (billingSubscription == null)
                                {
                                    res = new BusinessError<bool>(false, BusinessErrorState.Error, "Change plan failed. Can't find subscription.");
                                }
                                else if (returnedSaleView.NewSubscriptionID == null)
                                {
                                    res = new BusinessError<bool>(false, BusinessErrorState.Error, "Change plan failed. Invalid subscription is specified.");
                                }
                                else
                                {
                                    subscriptionService.ChangePlan(billingSubscription, returnedSaleView.NewSubscriptionID.Value);
                                }
                                break;
                            }
                        case TrimFuel.Model.Enums.ReturnProcessingActionEnum.SHIP_FREE_ITEM:
                            {
                                if (returnedSaleView.ExtraTrialShipTypeID == null)
                                {
                                    res = new BusinessError<bool>(false, BusinessErrorState.Error, "Send free item failed. Invalid free item is specified.");
                                }
                                else
                                {
                                    ExtraTrialShipType type = dao.Load<ExtraTrialShipType>(returnedSaleView.ExtraTrialShipTypeID);
                                    if (type == null)
                                    {
                                        res = new BusinessError<bool>(false, BusinessErrorState.Error, "Send free item failed. Invalid free item is specified.");
                                    }
                                    else
                                    {
                                        SendFreeProduct(billing, type.ProductCode, 1);
                                    }
                                }
                                break;
                            }
                        case TrimFuel.Model.Enums.ReturnProcessingActionEnum.BILL_AND_SHIP_ITEM:
                            {
                                if (returnedSaleView.UpsellTypeID == null)
                                {
                                    res = new BusinessError<bool>(false, BusinessErrorState.Error, "Bill and ship item failed. Invalid upsell item is specified.");
                                }
                                else
                                {
                                    UpsellType upsellType = dao.Load<UpsellType>(returnedSaleView.UpsellTypeID);
                                    if (upsellType == null)
                                    {
                                        res = new BusinessError<bool>(false, BusinessErrorState.Error, "Bill and ship item failed. Invalid upsell item is specified.");
                                    }
                                    else
                                    {
                                        CreateUpsellSale(billing, upsellType, 1, null, null, null, null, null, null, false);
                                    }
                                    
                                }
                                break;
                            }
                    }
                }
                else
                {
                    bool adminNoteExists = GetAdminNotesByBillingID(billing.BillingID.Value).Count > 0;
                    if (!adminNoteExists)
                    {
                        BillingSubscription billingSubscription = GetBillingSubscriptionBySale(saleID);
                        if (billingSubscription == null)
                        {
                            res = new BusinessError<bool>(false, BusinessErrorState.Error, "Cancel subscription failed. Can't find subscription.");
                        }
                        else
                        {
                            subscriptionService.CancelSubscription(billingSubscription);
                        }
                    }
                    else
                    {
                        BillingSubscription billingSubscription = GetBillingSubscriptionBySale(saleID);
                        if (billingSubscription == null)
                        {
                            res = new BusinessError<bool>(false, BusinessErrorState.Error, "Change subscription to Return no RMA failed. Can't find subscription.");
                        }
                        else
                        {
                            subscriptionService.ReturnNoRMASubscription(billingSubscription);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }

            return res;
        }

        private void SendFreeProduct(Billing billing, string productCode, int quantity)
        {
            try
            {
                dao.BeginTransaction();

                CreateExtraTrialShipSale(billing, productCode, quantity);

                Notes notes = new Notes()
                {
                    AdminID = 0,
                    Content = "Free item added: " + productCode + " / 1",
                    BillingID = (int)billing.BillingID.Value,
                    CreateDT = DateTime.Now
                };
                dao.Save<Notes>(notes);

                dao.CommitTransaction();                
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }
        }

        public SaleReturnProcessing GetReturnActionBySaleID(long saleID)
        {
            SaleReturnProcessing res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select rp.* from SaleReturnProcessing rp
                    where rp.SaleID=@saleID
                ");
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;
                res = dao.Load<SaleReturnProcessing>(q).SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        public BusinessError<SaleReturnProcessing> SetReturnActionForSale(long saleID, 
            bool enableReturnProcessing, short? returnProcessingActionID, 
            decimal? refundAmount,
            int? extraTrialShipTypeID, int? upsellTypeID, int? quantity, 
            int? newSubscriptionID, int? newRecurringPlanID)
        {
            BusinessError<SaleReturnProcessing> res = new BusinessError<SaleReturnProcessing>(null, BusinessErrorState.Error, "Unknown error occurred");
            try
            {
                if (enableReturnProcessing && returnProcessingActionID == null)
                {
                    res.ErrorMessage = "Action is not specified";
                }
                else
                {
                    if (!enableReturnProcessing)
                    {
                        returnProcessingActionID = null;
                    }

                    if (returnProcessingActionID == null)
                    {
                        refundAmount = null;
                        extraTrialShipTypeID = null;
                        upsellTypeID = null;
                        quantity = null;
                        newSubscriptionID = null;
                        newRecurringPlanID = null;
                    }
                    if (returnProcessingActionID != TrimFuel.Model.Enums.ReturnProcessingActionEnum.ISSUE_REFUND)
                    {
                        refundAmount = null;
                    }
                    if (returnProcessingActionID != TrimFuel.Model.Enums.ReturnProcessingActionEnum.BILL_AND_SHIP_ITEM)
                    {
                        upsellTypeID = null;
                    }
                    if (returnProcessingActionID != TrimFuel.Model.Enums.ReturnProcessingActionEnum.SHIP_FREE_ITEM)
                    {
                        extraTrialShipTypeID = null;
                    }
                    if (returnProcessingActionID != TrimFuel.Model.Enums.ReturnProcessingActionEnum.SHIP_FREE_ITEM &&
                        returnProcessingActionID != TrimFuel.Model.Enums.ReturnProcessingActionEnum.BILL_AND_SHIP_ITEM)
                    {
                        quantity = null;
                    }
                    if (returnProcessingActionID != TrimFuel.Model.Enums.ReturnProcessingActionEnum.CHANGE_PLAN)
                    {
                        newSubscriptionID = null;
                        newRecurringPlanID = null;
                    }
                    else if (newSubscriptionID != null)
                    {
                        newRecurringPlanID = null;
                    }
                    else if (newRecurringPlanID != null)
                    {
                        newSubscriptionID = null;
                    }

                    if (returnProcessingActionID == TrimFuel.Model.Enums.ReturnProcessingActionEnum.ISSUE_REFUND &&
                        refundAmount == null)
                    {
                        res.ErrorMessage = "Refund Amount is not specified";
                    }
                    else if ((returnProcessingActionID == TrimFuel.Model.Enums.ReturnProcessingActionEnum.SHIP_FREE_ITEM ||
                        returnProcessingActionID == TrimFuel.Model.Enums.ReturnProcessingActionEnum.BILL_AND_SHIP_ITEM)
                        && quantity == null)
                    {
                        res.ErrorMessage = "Quantity is not specified";
                    }
                    else if (returnProcessingActionID == TrimFuel.Model.Enums.ReturnProcessingActionEnum.SHIP_FREE_ITEM
                        && extraTrialShipTypeID == null)
                    {
                        res.ErrorMessage = "Product is not specified";
                    }
                    else if (returnProcessingActionID == TrimFuel.Model.Enums.ReturnProcessingActionEnum.BILL_AND_SHIP_ITEM
                        && upsellTypeID == null)
                    {
                        res.ErrorMessage = "Product is not specified";
                    }
                    else if (returnProcessingActionID == TrimFuel.Model.Enums.ReturnProcessingActionEnum.CHANGE_PLAN
                        && newSubscriptionID == null && newRecurringPlanID == null)
                    {
                        res.ErrorMessage = "Plan is not specified";
                    }
                    else
                    {
                        res.ReturnValue = GetReturnActionBySaleID(saleID);
                        if (res.ReturnValue == null)
                        {
                            res.ReturnValue = new SaleReturnProcessing();
                            res.ReturnValue.SaleID = saleID;
                        }
                        res.ReturnValue.ReturnProcessingActionID = returnProcessingActionID;
                        res.ReturnValue.RefundAmount = refundAmount;
                        res.ReturnValue.ExtraTrialShipTypeID = extraTrialShipTypeID;
                        res.ReturnValue.UpsellTypeID = upsellTypeID;
                        res.ReturnValue.Quantity = quantity;
                        res.ReturnValue.NewSubscriptionID = newSubscriptionID;
                        res.ReturnValue.NewRecurringPlanID = newRecurringPlanID;
                        dao.Save(res.ReturnValue);
                        res.State = BusinessErrorState.Success;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                res.ReturnValue = null;
            }
            return res;
        }
    }
}
