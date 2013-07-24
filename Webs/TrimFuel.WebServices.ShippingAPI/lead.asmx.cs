using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using TrimFuel.Business.Utils;
using MySql.Data.MySqlClient;
using TrimFuel.Business.Gateways.LeadGateways;
using TrimFuel.Business.Gateways;
using TrimFuel.Model.Enums;

namespace TrimFuel.WebServices.ShippingAPI
{
    /// <summary>
    /// Summary description for leads
    /// </summary>
    [WebService(Namespace = "http://trianglecrm.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class lead : System.Web.Services.WebService
    {
        LeadService leadService = new LeadService();
        BillingService billingService = new BillingService();

        [WebMethod]
        public void ProcessLeads(int leadType)
        {
            //get all rules by typeID
            var routingRules = leadService.GetRoutingRules().Where(u => u.LeadRouting.LeadTypeID == leadType);
            //get not dublicated productID from routingRules
            List<int?> productIDNotDublicatedList = routingRules.Select(u => u.LeadRouting.ProductID).Distinct().ToList();
            Random rand = new Random();
            foreach (var productID in productIDNotDublicatedList)
            {
                //get routing rules by productID from routingRules
                var productRoutingRules = routingRules.Where(u => u.LeadRouting.ProductID == productID).ToList();
                var index = new IndexOfOutbound(productRoutingRules.Select(u => u.LeadRouting.Percentage).ToArray());

                switch (leadType)
                {
                    case TrimFuel.Model.Enums.LeadTypeEnum.All:
                        {
                            IList<Registration> abandons = leadService.GetAbandonList(productID, DateTime.Now.AddHours(-1).AddMinutes(-30), DateTime.Now.AddMinutes(-30), false);
                            IList<Billing> confirms = leadService.GetConfirmList(productID, DateTime.Now.AddHours(-60), DateTime.Now, false);
                            IList<Billing> declines = leadService.GetDeclinedList(productID, DateTime.Now.AddHours(-12), DateTime.Now, false);
                            IList<Billing> inactives = leadService.GetInactiveList(productID, DateTime.Now.AddHours(-12), DateTime.Now, false);

                            //LeadType=All - send to all partners specified in routingrules
                            foreach (LeadRoutingView routingRule in productRoutingRules)
                            {
                                foreach (Registration abandon in abandons)
                                {
                                    SaveAbandon(abandon, routingRule);
                                }
                                foreach (Billing confirm in confirms)
                                {
                                    SaveConfirm(confirm, routingRule);
                                }
                                foreach (Billing decline in declines)
                                {
                                    SaveDeclined(decline, routingRule);
                                }
                                foreach (Billing inactive in inactives)
                                {
                                    SaveInactive(inactive, routingRule);
                                }
                            }
                            break;
                        }

                    case TrimFuel.Model.Enums.LeadTypeEnum.Abandons:
                        {
                            var regList = leadService.GetAbandonList(productID, DateTime.Now.AddHours(-1).AddMinutes(-30), DateTime.Now.AddMinutes(-30), true);
                            foreach (var reg in regList)
                            {
                                //send
                                //random routing rule
                                var routingRule = productRoutingRules[index.GetIndex(rand.Next(0, 100))];
                                SaveAbandon(reg, routingRule);
                            }
                            break;
                        }
                    case TrimFuel.Model.Enums.LeadTypeEnum.OrderConfirmations:
                        {
                            var bilList = leadService.GetConfirmList(productID, DateTime.Now.AddHours(-12), DateTime.Now, true);
                            foreach (var bill in bilList)
                            {
                                //send
                                //random routing rule
                                var routingRule = productRoutingRules[index.GetIndex(rand.Next(0, 100))];
                                SaveConfirm(bill, routingRule);
                            }
                            break;
                        }
                    case TrimFuel.Model.Enums.LeadTypeEnum.CancellationsDeclines:
                        {
                            var bilList = leadService.GetDeclinedList(productID, DateTime.Now.AddHours(-12), DateTime.Now, true);
                            foreach (var bill in bilList)
                            {
                                //send
                                //random routing rule
                                var routingRule = productRoutingRules[index.GetIndex(rand.Next(0, 100))];
                                SaveDeclined(bill, routingRule);
                            }

                            bilList = leadService.GetInactiveList(productID, DateTime.Now.AddHours(-12), DateTime.Now, true);
                            foreach (var bill in bilList)
                            {
                                //send
                                //random routing rule
                                var routingRule = productRoutingRules[index.GetIndex(rand.Next(0, 100))];
                                SaveInactive(bill, routingRule);
                            }
                            break;
                        }
                }
            }
        }

        [WebMethod]
        public void ProcessAllAbandonLeads(int productID)
        {
            //get all rules by typeID
            var routingRules = leadService.GetRoutingRules().Where(u => u.LeadRouting.LeadTypeID == LeadTypeEnum.Abandons);
            Random rand = new Random();
            //get routing rules by productID from routingRules
            var productRoutingRules = routingRules.Where(u => u.LeadRouting.ProductID == productID).ToList();
            var index = new IndexOfOutbound(productRoutingRules.Select(u => u.LeadRouting.Percentage).ToArray());

            var regList = leadService.GetAbandonList(productID, DateTime.Now.AddDays(-1000), DateTime.Now.AddMinutes(-30), true);
            foreach (var reg in regList)
            {
                //send
                //random routing rule
                var routingRule = productRoutingRules[index.GetIndex(rand.Next(0, 100))];
                SaveAbandon(reg, routingRule);
            }
        }

        [WebMethod]
        public void ProcessCampaignLeads(int leadType)
        {
            //get all rules by typeID
            IList<CampaignLeadRoutingView> routingRules = leadService.GetRoutingRulesForCampaign().Where(u => u.CampaignLeadRouting.LeadTypeID == leadType).ToList();

            //send to all partners specified in routingrules (ignore percentage for campaign lead routing)

            foreach (var routingRule in routingRules)
            {
                LeadRoutingView leadRouting = new LeadRoutingView();
                leadRouting.FillFromCampaignLeadRoutingView(routingRule);

                switch (leadType)
                {
                    case TrimFuel.Model.Enums.LeadTypeEnum.All:
                        {
                            IList<Registration> abandons = leadService.GetAbandonListForCampaign(routingRule.CampaignLeadRouting.CampaignID, routingRule.ProductID, DateTime.Now.AddHours(-1).AddMinutes(-30), DateTime.Now.AddMinutes(-30), false);
                            IList<Billing> confirms = leadService.GetConfirmListForCampaign(routingRule.CampaignLeadRouting.CampaignID, routingRule.ProductID, DateTime.Now.AddHours(-12), DateTime.Now, false);
                            IList<Billing> declines = leadService.GetDeclinedListForCampaign(routingRule.CampaignLeadRouting.CampaignID, routingRule.ProductID, DateTime.Now.AddHours(-12), DateTime.Now, false);
                            IList<Billing> inactives = leadService.GetInactiveListForCampaign(routingRule.CampaignLeadRouting.CampaignID, routingRule.ProductID, DateTime.Now.AddHours(-12), DateTime.Now, false);

                            foreach (Registration abandon in abandons)
                            {
                                SaveAbandon(abandon, leadRouting);
                            }
                            foreach (Billing confirm in confirms)
                            {
                                SaveConfirm(confirm, leadRouting);
                            }
                            foreach (Billing decline in declines)
                            {
                                SaveDeclined(decline, leadRouting);
                            }
                            foreach (Billing inactive in inactives)
                            {
                                SaveInactive(inactive, leadRouting);
                            }

                            break;
                        }

                    case TrimFuel.Model.Enums.LeadTypeEnum.Abandons:
                        {
                            var regList = leadService.GetAbandonListForCampaign(routingRule.CampaignLeadRouting.CampaignID, routingRule.ProductID, DateTime.Now.AddHours(-1).AddMinutes(-30), DateTime.Now.AddMinutes(-30), true);
                            foreach (var reg in regList)
                            {
                                //send
                                SaveAbandon(reg, leadRouting);
                            }
                            break;
                        }
                    case TrimFuel.Model.Enums.LeadTypeEnum.OrderConfirmations:
                        {
                            var bilList = leadService.GetConfirmListForCampaign(routingRule.CampaignLeadRouting.CampaignID, routingRule.ProductID, DateTime.Now.AddHours(-12), DateTime.Now, true);
                            foreach (var bill in bilList)
                            {
                                //send                               
                                SaveConfirm(bill, leadRouting);
                            }
                            break;
                        }
                    case TrimFuel.Model.Enums.LeadTypeEnum.CancellationsDeclines:
                        {
                            var bilList = leadService.GetDeclinedListForCampaign(routingRule.CampaignLeadRouting.CampaignID, routingRule.ProductID, DateTime.Now.AddHours(-12), DateTime.Now, true);
                            foreach (var bill in bilList)
                            {
                                //send                               
                                SaveDeclined(bill, leadRouting);
                            }

                            bilList = leadService.GetInactiveListForCampaign(routingRule.CampaignLeadRouting.CampaignID, routingRule.ProductID, DateTime.Now.AddHours(-12), DateTime.Now, true);
                            foreach (var bill in bilList)
                            {
                                //send
                                SaveInactive(bill, leadRouting);
                            }
                            break;
                        }
                }
            }
        }

        [WebMethod]
        public void SendLeads(int leadType)
        {
            IList<LeadPost> leadPostList = leadService.GetLeadPostListNotCompleted(leadType);

            IList<LeadPost> batch = new List<LeadPost>();

            foreach (LeadPost leadPost in leadPostList)
            {
                ILeadGateway gateway = GetGatewayByPartnerID(leadPost.LeadPartnerID);

                if (leadPost.LeadPartnerID == TrimFuel.Model.Enums.LeadPartnerEnum.Five9 && leadPost.ProductID == 10)
                    batch.Add(leadPost);

                else
                {
                    gateway.Send(leadPost);
                    System.Threading.Thread.Sleep(250);
                }
            }

            if (batch.Count > 0)
            {
                FiveNineGateway gateway = new FiveNineGateway();

                gateway.SendBatch(batch);
            }
        }

        [WebMethod]
        public void SendAragonClubLead(long registrationID, int clubID)
        {
            new LeadService().SendAragonClubLead(registrationID, clubID);
        }

        [WebMethod]
        public void SendREGUnsubscribe(string email)
        {
            if (string.IsNullOrEmpty(email))
                return;

            int leadPartnerID = 8;
            int leadTypeID = 1;
            int productID;
            REGGateway leadGateway = new REGGateway();
            var reg = leadService.GetAbandonByEmail(email, out productID);
            if (reg != null)
            {
                LeadRouting leadRouting = new LeadRouting() { LeadPartnerID = leadPartnerID, LeadTypeID = leadTypeID, ProductID = productID };
                LeadRoutingView routingRule = new LeadRoutingView() { LeadRouting = leadRouting };
                if (leadGateway != null)
                    leadGateway.SaveUnsub(reg, routingRule);
            }
        }

        void SaveAbandon(Registration registration, LeadRoutingView routingRule)
        {
            if (leadService.IsAlredySent(routingRule.LeadRouting.ProductID, registration.RegistrationID, routingRule.LeadRouting.LeadTypeID, routingRule.LeadRouting.LeadPartnerID))
                return;

            ILeadGateway leadGateway = GetGatewayByPartnerID(routingRule.LeadRouting.LeadPartnerID);
            if (leadGateway != null)
            {
                try
                {
                    leadGateway.SaveAbandon(registration, routingRule);
                }
                catch
                {
                    //catch not implemented exception
                }
            }
        }

        void SaveConfirm(Billing billing, LeadRoutingView routingRule)
        {
            if (leadService.IsAlredySent(routingRule.LeadRouting.ProductID, billing.RegistrationID, routingRule.LeadRouting.LeadTypeID, routingRule.LeadRouting.LeadPartnerID))
                return;

            ILeadGateway leadGateway = GetGatewayByPartnerID(routingRule.LeadRouting.LeadPartnerID);
            if (leadGateway != null)
            {
                try
                {
                    leadGateway.SaveConfirm(billing, routingRule);
                }
                catch
                {
                    //catch not implemented exception
                }
            }
        }

        void SaveDeclined(Billing billing, LeadRoutingView routingRule)
        {
            if (leadService.IsAlredySent(routingRule.LeadRouting.ProductID, billing.RegistrationID, routingRule.LeadRouting.LeadTypeID, routingRule.LeadRouting.LeadPartnerID))
                return;

            ILeadGateway leadGateway = GetGatewayByPartnerID(routingRule.LeadRouting.LeadPartnerID);
            if (leadGateway != null)
            {
                try
                {
                    leadGateway.SaveDeclined(billing, routingRule);
                }
                catch
                {
                    //catch not implemented exception 
                }
            }
            //
        }

        void SaveInactive(Billing billing, LeadRoutingView routingRule)
        {
            if (leadService.IsAlredySent(routingRule.LeadRouting.ProductID, billing.RegistrationID, routingRule.LeadRouting.LeadTypeID, routingRule.LeadRouting.LeadPartnerID))
                return;

            ILeadGateway leadGateway = GetGatewayByPartnerID(routingRule.LeadRouting.LeadPartnerID);
            if (leadGateway != null)
            {
                try
                {
                    leadGateway.SaveInactive(billing, routingRule);
                }
                catch
                {
                    //catch not implemented exception 
                }
            }
        }

        ILeadGateway GetGatewayByPartnerID(int? leadPartnerID)
        {
            switch (leadPartnerID)
            {
                case TrimFuel.Model.Enums.LeadPartnerEnum.REC:
                    {
                        return new RECGateway();
                    }
                case TrimFuel.Model.Enums.LeadPartnerEnum.CPAResponse:
                    {
                        return new CPAResponseGateway();
                    }
                case TrimFuel.Model.Enums.LeadPartnerEnum.Aragon:
                    {
                        return new AragonGateway();
                    }
                case TrimFuel.Model.Enums.LeadPartnerEnum.REG:
                    {
                        return new REGGateway();
                    }
                case TrimFuel.Model.Enums.LeadPartnerEnum.Five9:
                    {
                        return new FiveNineGateway();
                    }
                case TrimFuel.Model.Enums.LeadPartnerEnum.Focus:
                    {
                        return new FocusGateway();
                    }
            }
            return null;
        }
    }

    class IndexOfOutbound
    {
        private List<double> _intervals;

        public IndexOfOutbound(int?[] percentages)
        {
            _intervals = new List<double>();
            int sum = 0;
            foreach (var perc in percentages)
            {
                sum += perc.Value;
                _intervals.Add(sum / 100.0);
            }
        }

        public int GetIndex(int value)
        {
            double p = value / 100.0;
            for (int i = 0; i < _intervals.Count; i++)
                if (_intervals[i] >= p)
                    return i;
            return _intervals.Count - 1;
        }
    }
}
