using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Business.Gateways.LeadGateways;
using System.Net;
using log4net;

namespace TrimFuel.Business.Gateways.LeadGateways
{
    public class BaseLeadGateway
    {
        protected ILog logger = LogManager.GetLogger(typeof(BaseLeadGateway));
        protected LeadService leadService = new LeadService();

        protected string GetExpDate(int? expMonth, int? expYear)
        {
            string strExpMonth = String.Format("${0:f2}", expMonth.Value);
            string strExpYear = expYear.ToString();
            strExpYear = strExpYear.Length == 4 ? strExpYear.Substring(2, 2) : strExpYear;
            return strExpMonth + strExpYear;
        }

        protected string GetLeadPartnerConfigValues(LeadPartnerConfigValue.ID configKey, int? productID, int? leadTypeID)
        {
            return leadService.GetLeadPartnerConfigValues(
                                                configKey.LeadPartnerID,
                                                productID,
                                                leadTypeID)
                                                .Where(u => u.Key.ToLower() == configKey.Key.ToLower())
                                                .Select(u => u.Value).FirstOrDefault();
        }

        protected void CompleteRequest(string request, LeadPost leadPost)
        {
            try
            {
                string responseString = string.Empty;
                using (WebClient wc = new WebClient())
                {
                    responseString = wc.DownloadString(request);
                }
                leadPost.Completed = true;
                leadPost.PostResponse = responseString;
                leadService.SaveLeadPost(leadPost);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected void SaveLeadPostNotCompleted(Billing bill, TrimFuel.Model.Views.LeadRoutingView routingRule, string request)
        {
            LeadPost leadPostToInsert = new LeadPost()
            {
                CreateDT = DateTime.Now,
                LeadPartnerID = routingRule.LeadRouting.LeadPartnerID,
                LeadPostID = null,
                LeadTypeID = routingRule.LeadRouting.LeadTypeID,
                PostRequest = request,
                PostResponse = string.Empty,
                ProductID = routingRule.LeadRouting.ProductID,
                RegistrationDT = bill.CreateDT,
                RegistrationID = bill.RegistrationID,
                Telephone = bill.Phone,
                Completed = false
            };
            leadService.SaveLeadPost(leadPostToInsert);
        }

        protected void SaveLeadPostNotCompleted(Registration reg, TrimFuel.Model.Views.LeadRoutingView routingRule, string request)
        {
            LeadPost leadPostToInsert = new LeadPost()
            {
                CreateDT = DateTime.Now,
                LeadPartnerID = routingRule.LeadRouting.LeadPartnerID,
                LeadPostID = null,
                LeadTypeID = routingRule.LeadRouting.LeadTypeID,
                PostRequest = request,
                PostResponse = string.Empty,
                ProductID = routingRule.LeadRouting.ProductID,
                RegistrationDT = reg.CreateDT,
                RegistrationID = reg.RegistrationID,
                Telephone = reg.Phone,
                Completed = false
            };
            leadService.SaveLeadPost(leadPostToInsert);
        }

        public virtual void Send(LeadPost leadPost)
        {
            CompleteRequest(leadPost.PostRequest, leadPost);
        }
    }
}
