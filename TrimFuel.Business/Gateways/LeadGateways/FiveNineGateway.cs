using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using TrimFuel.Business.FiveNineApi;

namespace TrimFuel.Business.Gateways.LeadGateways
{
    public class FiveNineGateway : BaseLeadGateway, ILeadGateway
    {
        private const char SEPERATOR = ';';
        private const int ABANDON = 1;
        private const int CONFIRM = 2;
        private const int DECLINED = 3;

        private WsAdminService webService = new WsAdminService();

        public override void Send(LeadPost leadPost)
        {
            try
            {
                string csvRequest = leadPost.PostRequest;

                if (csvRequest == null)
                    return;

                string[] request = csvRequest.Split(SEPERATOR);

                string username = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.FiveNine_Password, leadPost.ProductID, leadPost.LeadTypeID);

                string password = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.FiveNine_Username, leadPost.ProductID, leadPost.LeadTypeID);

                string domainID = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.FiveNine_DomainID, leadPost.ProductID, leadPost.LeadTypeID);

                string listname = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.FiveNine_ListName, leadPost.ProductID, leadPost.LeadTypeID);

                webService.Username = username;
                webService.Password = password;

                addRecordToList newRecord = new addRecordToList();

                newRecord.record = request;
                newRecord.listName = listname;
                newRecord.listUpdateSettings = GetUpdateSettings((int)leadPost.LeadTypeID);                
                
                addRecordToListResponse response = webService.addRecordToList(newRecord);

                if (response.@return.uploadErrorsCount > 0 || !string.IsNullOrEmpty(response.@return.failureMessage))
                {
                    leadPost.Completed = false;
                    leadPost.PostResponse = webService.ResponseXML;
                }
                else
                {
                    leadPost.PostRequest = webService.RequestXml;
                    leadPost.Completed = true;
                    leadPost.PostResponse = webService.ResponseXML;
                }                
                                
                leadService.SaveLeadPost(leadPost);
                
            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                leadPost.Completed = false;
                //leadPost.PostRequest = webService.RequestXml;
                leadPost.PostResponse = webService.ResponseXML;
                leadService.SaveLeadPost(leadPost);
            }
            catch (Exception ex)
            {
                leadPost.Completed = false;
                //leadPost.PostRequest = webService.RequestXml;
                leadPost.PostResponse = ex.Message;
                leadService.SaveLeadPost(leadPost);
            }
        }

        public void SendBatch(IList<LeadPost> leads)
        {
            if (leads.Count < 1)
                return;

            string username = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.FiveNine_Password, leads[0].ProductID, leads[0].LeadTypeID);

            string password = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.FiveNine_Username, leads[0].ProductID, leads[0].LeadTypeID);

            string domainID = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.FiveNine_DomainID, leads[0].ProductID, leads[0].LeadTypeID);

            string listname = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.FiveNine_ListName, leads[0].ProductID, leads[0].LeadTypeID);

            webService.Username = username;
            webService.Password = password;

            string[][] importData = new string[leads.Count][];

            for (int i = 0; i < leads.Count; i++)
            {
                importData[i] = leads[i].PostRequest.Split(SEPERATOR);
            }

            addToList request = new addToList();
            request.importData = importData;
            request.listName = listname;
            request.listUpdateSettings = GetUpdateSettings((int)leads[0].LeadTypeID);

            addToListResponse response = webService.addToList(request);

            if (string.IsNullOrEmpty(response.@return.identifier))
            {
                foreach (LeadPost leadPost in leads)
                {
                    leadPost.Completed = false;
                    leadPost.PostResponse = webService.ResponseXML;
                }
            }
            else
            {
                importIdentifier identifier = new importIdentifier(){ identifier = response.@return.identifier};
                getListImportResult result = new getListImportResult() { identifier = identifier };

                getListImportResultResponse importResult = webService.getListImportResult(result);

                foreach (LeadPost leadPost in leads)
                {
                    //leadPost.PostRequest = webService.RequestXml;
                    leadPost.Completed = true;
                    leadPost.PostResponse = webService.ResponseXML;
                }
            }

            foreach (LeadPost leadPost in leads)
            {
                leadService.SaveLeadPost(leadPost);
            }
        }

        public void SaveConfirm(Billing bill, LeadRoutingView routingRule)
        {
            SaveLeadPostNotCompleted(bill, routingRule, ConvertBillingToRequestString(bill, routingRule.ProductName));
        }

        public void SaveAbandon(Registration reg, LeadRoutingView routingRule)
        {
            SaveLeadPostNotCompleted(reg, routingRule, ConvertRegistrationToRequestString(reg, routingRule.ProductName));
        }

        public void SaveDeclined(Billing bill, LeadRoutingView routingRule)
        {
            SaveLeadPostNotCompleted(bill, routingRule, ConvertBillingToRequestString(bill, routingRule.ProductName));
        }

        public void SaveInactive(Billing bill, LeadRoutingView routingRule)
        {
            SaveLeadPostNotCompleted(bill, routingRule, ConvertBillingToRequestString(bill, routingRule.ProductName));
        }

        private listUpdateSettings GetUpdateSettings(int leadTypeID)
        {
            fieldEntry[] fields;
            
            fields = new fieldEntry[10];

            fields[0] = new fieldEntry() { columnNumber = 1, fieldName = "number1", key = true };
            fields[1] = new fieldEntry() { columnNumber = 2, fieldName = "number2", key = false };
            fields[2] = new fieldEntry() { columnNumber = 3, fieldName = "number3", key = false };
            fields[3] = new fieldEntry() { columnNumber = 4, fieldName = "first_name", key = false };
            fields[4] = new fieldEntry() { columnNumber = 5, fieldName = "last_name", key = false };
            fields[5] = new fieldEntry() { columnNumber = 6, fieldName = "company", key = false };
            fields[6] = new fieldEntry() { columnNumber = 7, fieldName = "street", key = false };
            fields[7] = new fieldEntry() { columnNumber = 8, fieldName = "city", key = false };
            fields[8] = new fieldEntry() { columnNumber = 9, fieldName = "state", key = false };
            fields[9] = new fieldEntry() { columnNumber = 10, fieldName = "zip", key = false };
            //fields[10] = new fieldEntry() { columnNumber = 11, fieldName = "Date in Store", key = false };

            listUpdateSettings settings = new listUpdateSettings()
            {
                fieldsMapping = fields,
                reportEmail = null,
                separator = SEPERATOR.ToString(),
                skipHeaderLine = false,
                crmAddMode = crmAddMode.ADD_NEW,
                crmAddModeSpecified = true,
                crmUpdateMode = crmUpdateMode.UPDATE_SOLE_MATCHES,
                crmUpdateModeSpecified = true,
                listAddMode = listAddMode.ADD_FIRST,
                listAddModeSpecified = true
            };

            return settings;
        }

        private string ConvertBillingToRequestString(Billing b, string productName)
        {
            string[] fields = new string[10];

            fields[0] = b.Phone;
            fields[1] = "";
            fields[2] = "";
            fields[3] = b.FirstName;
            fields[4] = b.LastName;
            fields[5] = "";
            fields[6] = b.Address1;
            fields[7] = b.City;
            fields[8] = b.State;
            fields[9] = b.Zip;
            //fields[10] = ((DateTime)b.CreateDT).ToString("yyyy-MM-dd hh:mm:ss.fff");

            StringBuilder csvBuilder = new StringBuilder();

            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].Contains(SEPERATOR.ToString()))
                    fields[i].Replace(SEPERATOR.ToString(), "");

                csvBuilder.Append(fields[i]);

                if (i != fields.Length - 1)
                    csvBuilder.Append(SEPERATOR);
            }

            return csvBuilder.ToString();
        }

        private string ConvertRegistrationToRequestString(Registration r, string productName)
        {
            string[] fields = new string[10];

            fields[0] = r.Phone;
            fields[1] = "";
            fields[2] = "";
            fields[3] = r.FirstName;
            fields[4] = r.LastName;
            fields[5] = "";
            fields[6] = r.Address1;
            fields[7] = r.City;
            fields[8] = r.State;
            fields[9] = r.Zip;
            //fields[10] = ((DateTime)r.CreateDT).ToString("YYYY-MM-DD HH:MM:SS.mmm");

            StringBuilder csvBuilder = new StringBuilder();

            for (int i = 0; i < fields.Length; i++)
            {
                csvBuilder.Append(fields[i]);

                if (fields[i].Contains(SEPERATOR.ToString()))
                    fields[i].Replace(SEPERATOR.ToString(), "");

                if (i != fields.Length - 1)
                    csvBuilder.Append(SEPERATOR);

            }

            return csvBuilder.ToString();
        }
    }
}
