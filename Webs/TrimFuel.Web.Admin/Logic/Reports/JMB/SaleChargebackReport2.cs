using DocumentFormat.OpenXml.Packaging;
using TrimFuel.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Xml.Serialization;
using System.IO;
using System.Web;
using TrimFuel.Business;
using System.Web.Hosting;
using DocumentFormat.OpenXml.Wordprocessing;
using TrimFuel.Business.Utils;
using TrimFuel.Model.Enums;

namespace TrimFuel.Web.Admin.Logic.Reports.JMB
{
    public class SaleChargebackReport2
    {
        private string JMB_INFO_URL = "http://www.biggerbidder.com/livesupport/";
        //private string JMB_INFO_URL_TEST = "http://localhost/gateways/jmb_info.aspx";
        private string JMB_INFO_URL_TEST = "http://www.biggerbidder.com/livesupport/";
        private string JMB_INFO_POST_TEMPLATE = "username=triglecrm&password=Tra8a5ap!cha96ye&by=id&for={0}&page=userXML&login=login";

        public SaleChargebackReport2(string caseNumber, string arnNumber, string companyName, string companyAddress1, string companyAddress2, string assertigyMIDID, ChargeHistoryEx ch, Billing b, string internalID)
        {
            CaseNumber = caseNumber;
            ARNNumber = arnNumber;
            CompanyName = companyName;
            CompanyAddress1 = companyAddress1;
            CompanyAddress2 = companyAddress2;
            AssertigyMIDID = assertigyMIDID;
            ChargeHistory = ch;
            Billing = b;
            InternalID = internalID;
            GetJMBInfo();
        }

        private void GetJMBInfo()
        {
            if (!string.IsNullOrEmpty(InternalID))
            {
                try
                {
                    string url = (Config.Current.APPLICATION_ID == "jmb.trianglecrm.com" ? JMB_INFO_URL : JMB_INFO_URL_TEST);
                    //string url = JMB_INFO_URL;

                    HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url + "login.php");
                    httpRequest.Method = "POST";
                    httpRequest.ContentType = "application/x-www-form-urlencoded";
                    httpRequest.AllowAutoRedirect = false;

                    StreamWriter strOut = new StreamWriter(httpRequest.GetRequestStream());
                    strOut.Write(string.Format(JMB_INFO_POST_TEMPLATE, InternalID));
                    strOut.Close();

                    HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                    StreamReader strIn = new StreamReader(httpResponse.GetResponseStream());
                    string response = strIn.ReadToEnd();
                    strIn.Close();

                    httpRequest = (HttpWebRequest)WebRequest.Create(url + httpResponse.Headers["location"]);
                    httpRequest.Headers.Add("Cookie", httpResponse.Headers["Set-Cookie"].Replace("; path=/", "").Split(',')[1]);
                    httpRequest.Method = "GET";

                    httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                    strIn = new StreamReader(httpResponse.GetResponseStream());
                    response = strIn.ReadToEnd();
                    strIn.Close();

                    response = HttpUtility.HtmlDecode(response);

                    XmlSerializer s = new XmlSerializer(typeof(member), "");
                    using (StringReader sr = new StringReader(response))
                    {
                        JMBInfo = (member)s.Deserialize(sr);
                    }
                }
                catch (System.Exception ex) { throw ex; }
            }
        }

        public void Create(System.IO.Stream outputStream)
        {
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/MidImages/" + TrimFuel.Business.Config.Current.APPLICATION_ID + "/template.docx");
            byte[] byteArray = File.ReadAllBytes(path);
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                using (System.IO.Packaging.Package package = System.IO.Packaging.Package.Open(ms, FileMode.OpenOrCreate))
                {
                    using (System.IO.Packaging.Package outPackage = System.IO.Packaging.Package.Open(outputStream, FileMode.OpenOrCreate))
                    {
                        System.IO.Packaging.PackagePartCollection partList = package.GetParts();
                        foreach (System.IO.Packaging.PackagePart part in partList)
                        {
                            System.IO.Packaging.PackagePart newPart = outPackage.CreatePart(part.Uri, part.ContentType);
                            if (part.Uri.OriginalString == "/word/document.xml")
                            {
                                using (Stream inStr = Utility.OpenStringAsStreamUTF8(GenerateReportXML(JMBInfo.registration_details)))
                                {
                                    using (Stream outStr = newPart.GetStream())
                                    {
                                        CopyStream(inStr, outStr);
                                    }
                                }
                            }
                            else if (part.Uri.OriginalString == "/word/header1.xml")
                            {
                                using (Stream inStr = Utility.OpenStringAsStreamUTF8(GenerateReportHeaderXML()))
                                {
                                    using (Stream outStr = newPart.GetStream())
                                    {
                                        CopyStream(inStr, outStr);
                                    }
                                }
                            }
                            else
                            {
                                using (Stream inStr = part.GetStream())
                                {
                                    using (Stream outStr = newPart.GetStream())
                                    {
                                        CopyStream(inStr, outStr);
                                    }
                                }
                            }
                        }
                    }
                    //ProcessTemplate(package);
                }
                //ms.Flush();
                //ms.Seek(0, SeekOrigin.Begin);
                //ms.WriteTo(outputStream);
            }
        }

        public string CaseNumber { get; set; }
        public string ARNNumber { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress1 { get; set; }
        public string CompanyAddress2 { get; set; }
        public string AssertigyMIDID { get; set; }
        public string InternalID { get; set; }

        public member JMBInfo { get; set; }
        public ChargeHistoryEx ChargeHistory { get; set; }
        public Billing Billing { get; set; }

        private const long BUFFER_SIZE = 4096;

        private void CopyStream(System.IO.Stream inputStream, System.IO.Stream outputStream)
        {
            long bufferSize = inputStream.Length < BUFFER_SIZE ? inputStream.Length : BUFFER_SIZE;
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;
            long bytesWritten = 0;
            while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                outputStream.Write(buffer, 0, bytesRead);
                bytesWritten += bufferSize;
            }
        }

        private string GenerateReportXML(registration_details reg)
        {
            ChargeHistoryCard chargeHistoryCard;
            ICreditCardContainer creditCardContainer;
            chargeHistoryCard = new ReportService().Load<ChargeHistoryCard>(ChargeHistory.ChargeHistoryID);
            creditCardContainer = Billing;
            if (chargeHistoryCard != null)
                creditCardContainer = chargeHistoryCard;

            string document = Utility.LoadFromEmbeddedResource(typeof(SaleChargebackReport2), @"OpenXMLTemplates.document.xml");

            document = document.Replace("#ARN#", "<![CDATA[ " + ARNNumber + " ]]>");
            document = document.Replace("#RECORD_ID#", string.Empty);
            document = document.Replace("#CC64#", "<![CDATA[ " + creditCardContainer.CreditCardLeft6 + "------" + creditCardContainer.CreditCardRight4 + " ]]>");
            document = document.Replace("#NAME_UP#", "<![CDATA[ " + Billing.FullName.ToUpper() + " ]]>");
            document = document.Replace("#DATE_WRITING#", "<![CDATA[ " + DateTime.Today.ToString() + " ]]>");
            document = document.Replace("#DATE_PURCHASE#", "<![CDATA[ " + ChargeHistory.ChargeDate.Value.ToString() + " ]]>");
            document = document.Replace("#AMOUNT#", "<![CDATA[ " + ChargeHistory.Amount.Value.ToString("0.00") + " ]]>");
            document = document.Replace("#BIDS_TOTAL#", string.Empty);
            document = document.Replace("#BIDS_USED#", string.Empty);

            document = document.Replace("#EMAIL#", "<![CDATA[ " + Billing.Email + " ]]>");
            document = document.Replace("#FNAME#", "<![CDATA[ " + Billing.FirstName + " ]]>");
            document = document.Replace("#LNAME#", "<![CDATA[ " + Billing.LastName + " ]]>");
            document = document.Replace("#USERNAME#", "<![CDATA[ " + reg.username + " ]]>");
            document = document.Replace("#USER_ID#", "<![CDATA[ " + InternalID + " ]]>");
            document = document.Replace("#DATE_DIGNUP#", "<![CDATA[ " + reg.registration_date + " ]]>");
            document = document.Replace("#IP#", "<![CDATA[ " + Billing.IP + " ]]>");
            document = document.Replace("#DATE_CREATED#", string.Empty);
            document = document.Replace("#DATE_MODIFIED#", string.Empty);
            document = document.Replace("#ADDRESS1#", "<![CDATA[ " + Billing.Address1 + " ]]>");
            document = document.Replace("#CITY#", "<![CDATA[ " + Billing.City + " ]]>");
            document = document.Replace("#STATE#", "<![CDATA[ " + Billing.State + " ]]>");
            document = document.Replace("#ZIP#", "<![CDATA[ " + Billing.Zip + " ]]>");
            document = document.Replace("#PHONE#", "<![CDATA[ " + Billing.Phone + " ]]>");
            document = document.Replace("#GENDER#", "<![CDATA[ " + reg.sex + " ]]>");


            document = document.Replace("#DATE_TRANS#", "<![CDATA[ " + ChargeHistory.ChargeDate.Value.ToString() + " ]]>");
            document = document.Replace("#TRANS_ID#", "<![CDATA[ " + ChargeHistory.TransactionNumber + " ]]>");
            document = document.Replace("#CC_TYPE#", "<![CDATA[ " + PaymentTypeEnum.Types[creditCardContainer.PaymentTypeID] + " ]]>");
            document = document.Replace("#CC64_1#", "<![CDATA[ " + creditCardContainer.CreditCardLeft6 + "******" + creditCardContainer.CreditCardRight4 + " ]]>");
            document = document.Replace("#CC_EXP#", "<![CDATA[ " + creditCardContainer.ExpMonth.ToString() + "/" + (creditCardContainer.ExpYear.Value > 2000 ? creditCardContainer.ExpYear.Value - 2000 : creditCardContainer.ExpYear.Value).ToString() + " ]]>");

            TrimFuel.Business.Gateways.NetworkMerchants.NetworkMerchantsGatewayResponseParams nmiParams = new TrimFuel.Business.Gateways.NetworkMerchants.NetworkMerchantsGatewayResponseParams(ChargeHistory.Response);

            document = document.Replace("#AUTH_CODE#", "<![CDATA[ " + ChargeHistory.AuthorizationCode + " ]]>");
            document = document.Replace("#AVS_STATUS#", "<![CDATA[ " + nmiParams.GetParam("avsresponse") + " ]]>");
            document = document.Replace("#CVV_STATUS#", "<![CDATA[ " + nmiParams.GetParam("cvvresponse") + " ]]>");
            document = document.Replace("#PROCESSOR#", "<![CDATA[ " + ChargeHistory.ChildMID + " ]]>");
            document = document.Replace("#ORDER_ID#", string.Empty);

            if (JMBInfo.auctions_bid_on != null)
            {
                document = document.Replace("#AUCTIONS_TRs#", GetAuctionsXML(JMBInfo.auctions_bid_on.auctionList));
            }
            else
            {
                document = document.Replace("#AUCTIONS_TRs#", "");
            }

            if (JMBInfo.purchased_bidpacks != null)
            {
                document = document.Replace("#BIDDING_TRs#", GetBiddingsXML(JMBInfo.purchased_bidpacks.bidpackList));
            }
            else
            {
                document = document.Replace("#BIDDING_TRs#", "");
            }

            return document;
        }

        private string GetAuctionsXML(IList<auction> info)
        {
            string tmp = Utility.LoadFromEmbeddedResource(typeof(SaleChargebackReport2), @"OpenXMLTemplates.auction_tr.xml");
            string res = "";

            foreach (auction item in info)
            {
                if (item.auction_details != null)
                {
                    string auctXml = tmp;
                    auctXml = auctXml.Replace("#AUCTION_NAME#", "<![CDATA[ " + HttpUtility.HtmlDecode(item.auction_details.productname) + " ]]>");
                    auctXml = auctXml.Replace("#AUCTION_STATUS#", "<![CDATA[ " + item.auction_details.auction_status + " ]]>");
                    auctXml = auctXml.Replace("#AUCTION_BIDS#", "<![CDATA[ " + item.auction_details.total_bids_placed + " ]]>");
                    auctXml = auctXml.Replace("#BID_STATUS#", "<![CDATA[ " + item.auction_details.bidding_outcome + " ]]>");
                    auctXml = auctXml.Replace("#AUCTION_END_DATE#", "<![CDATA[ " + item.auction_details.enddate + " ]]>");
                    auctXml = auctXml.Replace("#AUCTION_START_DATE#", "<![CDATA[ " + item.auction_details.startdate + " ]]>");
                    auctXml = auctXml.Replace("#AUCTION_OWNER#", "<![CDATA[ " + item.auction_details.seller + " ]]>");
                    if (item.auction_details.bidding_outcome.ToLower() == "lost")
                    {
                        auctXml = auctXml.Replace("#STATUS_COLOR#", "FF0000");
                    }
                    else
                    {
                        auctXml = auctXml.Replace("#STATUS_COLOR#", "00FF00");
                    }
                    res += auctXml;
                }
            }

            return res;
        }

        private string GetBiddingsXML(IList<bidpack> info)
        {
            string tmp = Utility.LoadFromEmbeddedResource(typeof(SaleChargebackReport2), @"OpenXMLTemplates.bidding_tr.xml");
            string res = "";

            bool alt = false;
            foreach (bidpack item in info)
            {
                string bidXml = tmp;
                if (alt)
                {
                    bidXml = bidXml.Replace("cnfStyle w:val=\"000000100000\"", "cnfStyle w:val=\"000000000000\"");
                }
                bidXml = bidXml.Replace("#DATE#", "<![CDATA[ " + item.date_purchased + " ]]>");
                bidXml = bidXml.Replace("#COUNT#", "<![CDATA[ " + item.bids_left + " ]]>");
                bidXml = bidXml.Replace("#PACK_ID#", string.Empty);
                bidXml = bidXml.Replace("#FLAG#", string.Empty);
                bidXml = bidXml.Replace("#TYPE#", string.Empty);
                bidXml = bidXml.Replace("#RECHARGE_TYPE#", string.Empty);
                bidXml = bidXml.Replace("#DESCRIPTION#", "<![CDATA[ " + item.description + " ]]>");
                bidXml = bidXml.Replace("#TRANS_ID#", string.Empty);
                res += bidXml;
                alt = !alt;
            }

            return res;
        }

        private string GenerateReportHeaderXML()
        {
            string header = Utility.LoadFromEmbeddedResource(typeof(SaleChargebackReport2), @"OpenXMLTemplates.header.xml");

            return header.Replace("#RECORD_ID#", string.Empty);
        }
    }
}
