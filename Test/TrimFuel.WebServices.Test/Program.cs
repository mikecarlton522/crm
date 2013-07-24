using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Business.Dao;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Business.Gateways;
using TrimFuel.Business.Gateways.ABF;
using TrimFuel.Business.Gateways.BadCustomer;
using TrimFuel.Business.Utils;
using System.Xml.Serialization;
using System.IO;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using TrimFuel.Model.Views;
using TrimFuel.Business.Gateways.AtLastFulfillment;
using System.Net;
using TrimFuel.Business.Gateways.GoFulfillmentGateway;
using TrimFuel.Model.Enums;
using TrimFuel.Business.Gateways.MoldingBox;

namespace TrimFuel.WebServices.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestBL();
            //TestReadSubscription();
            //TestRegistration();
            //TestBilling();
            //TestAssignPlan();
            //TestCancellationEmails();
            //TestBadCustomer();
            //TestBadCustomerService();
            //TestFraudService();
            //TestABF();
            //AbfFoundOrders();
            //SalesShipped();
            //TestInvalidTrackingNumber();
            //TestYIG();
            //GenerateEncKey();
            //GeneratePGPKeyPair("Username", "Password");
            //TestBlueOctopus();
            //TestTutorVista();
            //TestConfig();
            //TestSupportService();
            //TestSupportServiceLocal();
            //TestCustomerServiceFeed();
            //TestPop3()
            //TestBillingAPI();
            //TestJMB();
            //TestAtLastFulfillment();
            //TestAtLastFulfillment2();
            //TestJMBProspect();
            //TestCoActionProspect();
            //TestLocalProspect();
            //TestDirectAction();
            //TestLearn2BeFree();
            //TestDirectActionSOAPPost();
            //TestDirectActionSOAPPostSub();            
            //TestLocalSTOBillingAPI();
            TestSTOBillingAPI();
            //TestGoFulfillment();
            //TestLocalSTOBillingAPI();
            //TestDirectActionSOAPPost_CreateSubscription();
            //TestApexAPI();
            //TestLocalChargeSales();
            //TestShipperConfig();
            //TestMoldingBoxTrackingNumber();

            Console.Read();
        }

        private static void TestMoldingBoxTrackingNumber()
        {
            try 
	        {
		        TrimFuel.Business.MBApi.MBAPI MBApi = new TrimFuel.Business.MBApi.MBAPI();
                List<TrimFuel.Business.MBApi.StatusResponse> ResList =
                    MBApi.Retrieve_Shipment_Status("MB4C8S2N5J6M3F2$X8G2#C3S2K85GE", new int[] { 964985 }).ToList();

                string trackingNumber = null;
                TrimFuel.Business.MBApi.StatusResponse res = ResList[0];

                if (res.RequestSuccessfullyReceived && res.ShipmentExists && (res.ShipmentStatusID == 4))
                {
                    trackingNumber = res.TrackingNumber;
                }                
	        }
	        catch (Exception ex)
	        {		
	        }
        }

        private static void TestShipperConfig()
        {
            try
            {
                Console.WriteLine("Get config for ABF...");
                var cfg = GetAndCheckShipperConfig(ShipperEnum.ABF);
                foreach (var item in cfg)
                {
                    Console.WriteLine(item.Key.Key + ": " + item.Value.Value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void TestApexAPI()
        {
            com.trianglecrm.apex.billing_ws cl = new com.trianglecrm.apex.billing_ws();
            var res = cl.Charge("apex", "jwie832)*w1q", 1.00M, 0.00M, false, 5, true, 0, false, 0, false, "Success", "Test",
                "Test", "Test", "Test", "NY", "00000", "US", "0000000", "dtcoder@gmail.com", "127.0.0.1",
                "", "", null, 0, false, 0, false, "4111111111111111", "000", 11, 2012, null, null, null, null, null, null);
        }

        private static void TestGoFulfillment()
        {
            var cfg = GetAndCheckShipperConfig(ShipperEnum.GoFulfillment);
            string r = null;
            string res = null;
            (new GoFulfillmentGateway(cfg)).PostShipment(0, new Registration()
            {
                FirstName = "Test",
                LastName = "Test",
                Address1 = "Test"
            }, new Billing()
            {
                FirstName = "Test",
                LastName = "Test",
                Address1 = "Test"
            }, "Test", new List<KeyValuePair<InventoryView, long>>(){
                new KeyValuePair<InventoryView, long>(new InventoryView(){InventoryID = 1, Product = "TEST-SKU", Quantity = 1, SKU = "TEST-SKU"}, 0)
            }, out r, out res);
        }

        private static void TestCustomerServiceFeed()
        {
            string soapRequest = Utility.LoadFromEmbeddedResource(typeof(Program), "testCustomServiceFeed.txt");
            string soapResponse = null;
            try
            {
                soapResponse = HttpSOAPRequest("http://trimfuel.localhost/support/customer_service_feed.asmx", "support/Process", soapRequest);
            }
            catch (Exception ex)
            {
            }
        }

        private static void TestDirectActionSOAPPostSub()
        {
            string soapRequest = Utility.LoadFromEmbeddedResource(typeof(Program), "DirectActionSOAP_Sub.txt");
            string soapResponse = null;
            try
            {
                soapResponse = HttpSOAPRequest("https://directaction.trianglecrm.com/api/billing_ws.asmx", "CreateSubscription", soapRequest);
            }
            catch (Exception ex)
            {
            }
        }

        private static void TestDirectActionSOAPPost()
        {
            string soapRequest = Utility.LoadFromEmbeddedResource(typeof(Program), "DirectActionSOAP.txt");
            string soapResponse = null;
            try
            {
                soapResponse = HttpSOAPRequest("https://directaction.trianglecrm.com/api/billing_ws.asmx", "Charge", soapRequest);
            }
            catch (Exception ex)
            {
            }
        }

        private static void TestDirectActionSOAPPost_CreateSubscription()
        {
            string soapRequest = Utility.LoadFromEmbeddedResource(typeof(Program), "DirectActionSOAP_CreateSubscription.txt");
            string soapResponse = null;
            try
            {
                soapResponse = HttpSOAPRequest("https://directaction.trianglecrm.com/api/billing_ws.asmx", "CreateSubscription", soapRequest);
            }
            catch (Exception ex)
            {
            }
        }

        private static string HttpSOAPRequest(string url, string action, string body)
        {
            string res = null;

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Headers.Add("SOAPAction", "\"http://trianglecrm.com/" + action + "\"");
            httpRequest.ContentType = "text/xml; charset=utf-8";
            httpRequest.Accept = "text/xml";
            httpRequest.Method = "POST";

            StreamWriter strOut = new StreamWriter(httpRequest.GetRequestStream());
            strOut.Write(body);
            strOut.Close();

            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            StreamReader strIn = new StreamReader(httpResponse.GetResponseStream());
            res = strIn.ReadToEnd();
            strIn.Close();

            return res;
        }

        public string PrepareChargeSOAP(string username, string password, string amount, string shipping, string firstName, string lastName, string address1, string address2, string city, string state, string zip, string phone, string email, string ip, string affiliate, string subAffiliate, string internalID, string paymentType, string creditCard, string cvv, string expMonth, string expYear)
        {
            string request = string.Empty;
            request += "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n";
            request += "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">";
            request += "<soap:Body>";
            request += "<Charge xmlns=\"http://trianglecrm.com/\">";

            if (!string.IsNullOrEmpty(username)) request += "<username>" + username + "</username>";
            if (!string.IsNullOrEmpty(password)) request += "<password>" + password + "</password>";
            if (!string.IsNullOrEmpty(amount)) request += "<amount>" + amount + "</amount>";
            if (!string.IsNullOrEmpty(shipping)) request += "<shipping>" + shipping + "</shipping>";
            if (!string.IsNullOrEmpty(firstName)) request += "<firstName>" + firstName + "</firstName>";
            if (!string.IsNullOrEmpty(lastName)) request += "<lastName>" + lastName + "</lastName>";
            if (!string.IsNullOrEmpty(address1)) request += "<address1>" + address1 + "</address1>";
            if (!string.IsNullOrEmpty(address2)) request += "<address2>" + address2 + "</address2>";
            if (!string.IsNullOrEmpty(city)) request += "<city>" + city + "</city>";
            if (!string.IsNullOrEmpty(state)) request += "<state>" + state + "</state>";
            if (!string.IsNullOrEmpty(zip)) request += "<zip>" + zip + "</zip>";
            if (!string.IsNullOrEmpty(phone)) request += "<phone>" + phone + "</phone>";
            if (!string.IsNullOrEmpty(email)) request += "<email>" + email + "</email>";
            if (!string.IsNullOrEmpty(ip)) request += "<ip>" + ip + "</ip>";
            if (!string.IsNullOrEmpty(affiliate)) request += "<affiliate>" + affiliate + "</affiliate>";
            if (!string.IsNullOrEmpty(subAffiliate)) request += "<subAffiliate>" + subAffiliate + "</subAffiliate>";
            if (!string.IsNullOrEmpty(internalID)) request += "<internalID>" + internalID + "</internalID>";
            if (!string.IsNullOrEmpty(paymentType)) request += "<paymentType>" + paymentType + "</paymentType>";
            if (!string.IsNullOrEmpty(creditCard)) request += "<creditCard>" + creditCard + "</creditCard>";
            if (!string.IsNullOrEmpty(cvv)) request += "<cvv>" + cvv + "</cvv>";
            if (!string.IsNullOrEmpty(expMonth)) request += "<expMonth>" + expMonth + "</expMonth>";
            if (!string.IsNullOrEmpty(expYear)) request += "<expYear>" + expYear + "</expYear>";

            request += "</Charge>";
            request += "</soap:Body>";
            request += "</soap:Envelope>";

            return request;
        }

        static void TestDirectAction()
        {
            com.trianglecrm.directaction.billing_ws cl = new com.trianglecrm.directaction.billing_ws();
            com.trianglecrm.directaction.BusinessErrorOfChargeHistory res = null;
            res = cl.Charge("directaction", "dew9JKq2az", 11.2M, 13.1M, true, 0, false, 0, false, "Test", "Test", "Test", null, "Test", "NY", "00000", "0000000000", "dtcoder@gmail.com",
                 "127.0.0.1", "testAff", "testSub", null, 0, false, 2, "4111111111111111", "000", 2, 2014, null);
            res = cl.Charge("directaction", "dew9JKq2az", 11.2M, 13.1M, true, 0, false, 0, false, "Success", "Test", "Test", null, "Test", "NY", "00000", "0000000000", "dtcoder@gmail.com",
                "127.0.0.1", "testAff", "testSub", null, 0, false, 2, "4111111111111111", "000", 2, 2014, null);
        }

        static void TestLearn2BeFree()
        {
            com.trianglecrm.learn2befree.billing_ws cl = new com.trianglecrm.learn2befree.billing_ws();
            com.trianglecrm.learn2befree.BusinessErrorOfChargeHistory res = null;
            res = cl.Charge("learn2befree", "81Aw_KNx5R", 11.2M, 13.1M, true, 0, false, 0, false, "Test", "Test", "Test", null, "Test", "NY", "00000", "0000000000", "dtcoder@gmail.com",
                 "127.0.0.1", "testAff", "testSub", null, 0, false, 2, "4111111111111111", "000", 2, 2014, null);
            res = cl.Charge("learn2befree", "81Aw_KNx5R", 11.2M, 13.1M, true, 0, false, 0, false, "Success", "Test", "Test", null, "Test", "NY", "00000", "0000000000", "dtcoder@gmail.com",
                "127.0.0.1", "testAff", "testSub", null, 0, false, 2, "4111111111111111", "000", 2, 2014, null);
        }

        static void TestJMBProspect()
        {
            com.trianglecrm.jmb.billing_ws cl = new com.trianglecrm.jmb.billing_ws();
            //com.trianglecrm.jmb.BusinessErrorOfProspect res = cl.CreateProspect("jmb", "rt67-12a", "Test", "Test", "Test", null, "Test", "NY", "00000", "000001234", "dtcoder@gmail.com",
            //    "127.0.0.1", "testAff", "testSub", "test00002");
        }

        static void TestCoActionProspect()
        {
            com.trianglecrm.coaction.billing_ws cl = new com.trianglecrm.coaction.billing_ws();
            com.trianglecrm.coaction.BusinessErrorOfProspect res = cl.CreateProspect("Coaction_Triangle", "h37A_qMt1", 2, true, "Test", "Test", "Test", null, "Test", "NY", "00000", "AU", "000001234", "mrsenya@tut.by",
                "127.0.0.1", "testAff", "testSub", "test000000005", null, null, null, null, null);
        }

        static void TestLocalProspect()
        {
            localhost.client1.billing_ws cl = new localhost.client1.billing_ws();
            localhost.client1.BusinessErrorOfProspect res = cl.CreateProspect("aa1", "bb", 10, true, "First", "Last", null, null, null, null, null, null, "+000111", "unique3@mail.non",
                null, null, null, "test00009", 
                "username", "password", null, null, null);
        }

        protected static IDictionary<ShipperConfig.ID, ShipperConfig> GetAndCheckShipperConfig(short shipperID)
        {
            ShipperConfigService srv = new ShipperConfigService();
            Shipper sh = new BaseService().Load<Shipper>(shipperID);
            IDictionary<ShipperConfig.ID, ShipperConfig> res = srv.GetShipperConfig(sh.ShipperID);
            srv.CheckShipperConfig(sh, res);
            return res;
        }

        static void TestAtLastFulfillment2()
        {
            var cfg = GetAndCheckShipperConfig(ShipperEnum.AtLastFulfillment);
            AtLastFulfillmentGateway gateway = new AtLastFulfillmentGateway(cfg);
            string tracknum = gateway.CheckTrackingNumber(15638697);
        }

        static void TestAtLastFulfillment()
        {
            IDao dao = new MySqlDao("Datasource=localhost;Database=TrimFuel_Client1;uid=root;pwd=1;");
            Registration r = dao.Load<Registration>(65583);
            IList<InventoryView> inv = new List<InventoryView>();
            inv.Add(new InventoryView() { 
                SKU = "SKU_1",
                Quantity = 1
            });
            inv.Add(new InventoryView()
            {
                SKU = "SKU_2",
                Quantity = 3
            });

            var cfg = GetAndCheckShipperConfig(ShipperEnum.AtLastFulfillment);
            AtLastFulfillmentGateway gateway = new AtLastFulfillmentGateway(cfg);
            string request = null;
            string response = null;
            long? regID = null;
            try
            {
                regID = gateway.PostShipment(1, r, "United States", inv, "", out request, out response);
            }
            catch (Exception ex)
            {
                response = "Error: " + ex.ToString();
            }
            Console.WriteLine(request);
            Console.WriteLine(response);
        }

        static void TestJMB()
        {
            com.trianglecrm.jmb.billing_ws cl = new com.trianglecrm.jmb.billing_ws();
            com.trianglecrm.jmb.BusinessErrorOfChargeHistory res = cl.Charge("jmb", "rt67-12a", 11.2M, 13.1M, true, 3, true, 0, false, "Test", "Test", "Test", null, "Test", "NY", "00000", "0000000000", "dtcoder@gmail.com",
                "127.0.0.1", "testAff", "testSub", null, 0, false, 2, "4111111111111111", "000", 2, 2014, null, 
                "Abandon=true", null, null, null, null);
            res = cl.Charge("jmb", "rt67-12a", 11.2M, 13.1M, true, 3, true, 0, false, "Success", "Test", "Test", null, "Test", "NY", "00000", "0000000000", "dtcoder@gmail.com",
                "127.0.0.1", "testAff", "testSub", null, 0, false, 2, "4111111111111111", "000", 2, 2014, null,
                "Abandon=false", null, null, null, null);
        }

        static void TestLocalChargeSales()
        {
            localhost.client1.billing_ws cl = new localhost.client1.billing_ws();
            localhost.client1.BusinessErrorOfChargeHistorySales res = cl.ChargeSales("aa1", "bb", 10, true, 
                new localhost.client1.ProductDesc[]{
                    new localhost.client1.ProductDesc(){
                        Amount = 24.3M,
                        ProductID = 5
                    },
                    new localhost.client1.ProductDesc(){
                        Amount = 9.99M,
                        ProductID = 6
                    },
                    new localhost.client1.ProductDesc(){
                        Amount = 79.55M,
                        ProductID = 7
                    }
                },
                0, false, "Success", "Test", "Test Address", null, "Test", "NY", "00000", "Canada", "0000000000", "dtcoder@gmail.com",
                "127.0.0.1", "aff", "sub", "", 0, false, 2, false, "4111111111211112", "111", 2, 2014,
                "custom11", null, "1", "custom14", "custom15");

            //cl.CreateSubscription("aa1", "bb", 409, true, "First4", "Last7", "Address6", null, "City", "NY", "0000", "+0000000", "dtcoder@gmail.com",
            //    "127.0.0.1", "aff", "sub", "test00005", 65610, true, 2, "4211111111111111", "111", 2, 2014, null);

            //cl.CreateSubscription("aa1", "bb", 409, true, 0, false, "First4", "Last7", "Address6", null, "City", "NY", "0000", "US", "+0000000", "dtcoder@gmail.com",
            //    "127.0.0.1", "aff", "sub", null, 0, false, 2, false, "4211111111111111", "111", 2, 2014, null);
        }

        static void TestBillingAPI()
        {
            localhost.client1.billing_ws cl = new localhost.client1.billing_ws();
            localhost.client1.BusinessErrorOfChargeHistory res = cl.Charge("aa1", "bb", 11.2M, 13.1M, true, 12, true, 76, true, 0, false, "Test", "Test", "Test Address", null, "Test", "NY", "00000", "Canada", "0000000000", "dtcoder@gmail.com",
                "127.0.0.1", "aff", "sub", "", 0, false, 2, false, "4454545454545454", "111", 2, 2014, null,
                "custom11", null, "1", "custom14", "custom15");

            //cl.CreateSubscription("aa1", "bb", 409, true, "First4", "Last7", "Address6", null, "City", "NY", "0000", "+0000000", "dtcoder@gmail.com",
            //    "127.0.0.1", "aff", "sub", "test00005", 65610, true, 2, "4211111111111111", "111", 2, 2014, null);

            cl.CreateSubscription("aa1", "bb", 409, true, 0, false, "First4", "Last7", "Address6", null, "City", "NY", "0000", "US", "+0000000", "dtcoder@gmail.com",
                "127.0.0.1", "aff", "sub", null, 0, false, 2, false, "4211111111111111", "111", 2, 2014, null);
        }

        static void TestLocalSTOBillingAPI()
        {
            localhost.trimfuel1.billing_ws cl = new localhost.trimfuel1.billing_ws();
            
            var res1 = cl.CreateSubscription("sto_aa1", "to_bb", 409, true, 1100, true, "First4", "Last7", "Address6", null, "City", "NY", "0000", "US", "+0000000", "dtcoder@gmail.com",
                "127.0.0.1", "aff", "sub", "new_test_00010", 0, false, 10, false, "4211111111111111", "111", 2, 2014, null);

            localhost.trimfuel1.BusinessErrorOfChargeHistory res = cl.Charge("sto_aa1", "to_bb", 1M, 13.1M, true, 10, true, 76, true, 1101, true, "Success", "Test", "Test Address", null, "Test", "NY", "00000", "Canada", "0000000000", "dtcoder@gmail.com",
                "127.0.0.1", "aff", "sub", "new_test_00010", 0, false, 10, false, "4111111111111111", "111", 2, 2014, null,
                null, null, null, null, null);
        }

        static void TestSTOBillingAPI()
        {
            com.trianglecrm.dashboard1.billing_ws cl = new com.trianglecrm.dashboard1.billing_ws();
            com.trianglecrm.dashboard1.BusinessErrorOfChargeHistory res = cl.Charge("triangleEcigs", "lkMlvjq1_o$2", 2.2M, 1.1M, true, 20, true, 94, true, 1050, true, "Success", "Test", "Test", null, "Test", "NY", "00000", "US", "0000000000", "dtcoder@gmail.com",
                "127.0.0.1", "test", "test", null, 0, false, 2, "4111111111111111", "000", 2, 2014, null,
                null, null, null, null, null);

            cl.CreateSubscription("triangleEcigs", "lkMlvjq1_o$2", 466, true, 1050, true, "Success", "Test", "Test", null, "Test", "NY", "00000", "US", "0000000000", "dtcoder@gmail.com",
                "127.0.0.1", "test", "test", null, 0, false, 2, "4111111111111111", "000", 2, 2014, null);
        }

        static void TestPop3()
        {
            //System.Net.Mail.WebClient wc = new System.Net.WebClient();
        }

        static void TestSupportService()
        {
            com.trianglecrm.dashboard.customer_service_feed cl = new com.trianglecrm.dashboard.customer_service_feed();
            Console.Write(cl.Process(new TrimFuel.WebServices.Test.com.trianglecrm.dashboard.Triangle(){
                csi = new TrimFuel.WebServices.Test.com.trianglecrm.dashboard.csi(){
                    partner = "p",
                    version = "v",
                    call = new TrimFuel.WebServices.Test.com.trianglecrm.dashboard.call[]{
                        new TrimFuel.WebServices.Test.com.trianglecrm.dashboard.call(){
                            id = "123123123123",
                            time = new TrimFuel.WebServices.Test.com.trianglecrm.dashboard.time(){start= "4/7/2011 2:06:00 AM", end="5/7/2011 2:06:00 AM", ani = "ANI 1", dnis = "DNIS 1"},
                            agent = new TrimFuel.WebServices.Test.com.trianglecrm.dashboard.agent(){id= "222222", location = "Agent Location", name = "Agent Location 1"},
                            disposition = new TrimFuel.WebServices.Test.com.trianglecrm.dashboard.disposition(){id="2222222", name="DispositionName", agent_notes="Agent Notes 1"},
                            customer = new TrimFuel.WebServices.Test.com.trianglecrm.dashboard.customer(){id = "33333333333", product = "Customer Product 1"}
                        }
                    }
                }
            }));
            
        }

        static void TestSupportServiceLocal()
        {
            localhost.trimfuel.customer_service_feed cl = new localhost.trimfuel.customer_service_feed();
            Console.Write(cl.Process(new TrimFuel.WebServices.Test.localhost.trimfuel.Triangle()
            {
                csi = new TrimFuel.WebServices.Test.localhost.trimfuel.csi()
                {
                    partner = "payter",
                    version = "v1.1.1.1",
                    call = new TrimFuel.WebServices.Test.localhost.trimfuel.call[]{
                        new TrimFuel.WebServices.Test.localhost.trimfuel.call(){
                            id = "1334522345",
                            time = new TrimFuel.WebServices.Test.localhost.trimfuel.time(){start= "1/7/2011 2:06:00 AM", end="1/7/2011 2:06:00 AM", ani = "ANI1", dnis = "DNIS1"},
                            agent = new TrimFuel.WebServices.Test.localhost.trimfuel.agent(){id= "10111111", location = "Agent Location1", name = "Agent Location1"},
                            disposition = new TrimFuel.WebServices.Test.localhost.trimfuel.disposition(){id="2002222", name="DispositionName1", agent_notes="Agent Notes1"},
                            customer = new TrimFuel.WebServices.Test.localhost.trimfuel.customer(){id = "30333333333", product = "Customer Product1"},
                            contactid = "some contact id"
                        }
                    }
                }
            }));
        }

        static void TestConfig()
        {
            Console.WriteLine(string.Join(",", Config.Current.CONNECTION_STRINGS.Values.ToArray()));
            Console.WriteLine(Config.Current.NETWORKMERCHANTS_URL);
            Console.WriteLine(Config.Current.SHW_GATEWAY_URL);
            Console.WriteLine(Config.Current.BAD_CUSTOMER_URL);
            Console.WriteLine(Config.Current.MAX_MIND_URL);
            Console.WriteLine(Config.Current.SHIPPING_TEST_MODE);
            //Console.WriteLine(Config.Current.ABF_TEST_MODE);
            //Console.WriteLine(Config.Current.ABF_THREE_PL_KEY);
            //Console.WriteLine(Config.Current.ABF_THREE_PL_ID);
            //Console.WriteLine(Config.Current.ABF_LOGIN);
            //Console.WriteLine(Config.Current.ABF_PASSWORD);
            //Console.WriteLine(Config.Current.ABF_FACILITY_ID);
            //Console.WriteLine(string.Join(",", Config.Current.KEYMAIL_EMAIL_ADDRESS));
        }

        static void TestTutorVista()
        {
            TutorVista.billing_wsSoapClient cl = new TrimFuel.WebServices.Test.TutorVista.billing_wsSoapClient();
            var res = cl.Charge("tutorvista", "0zm_5APvV2a", 10.6M, "Test", "Test", "Test", null, "Test", "AZ", "00000", null, null, null, "testAff", "testSubAff", "Test00001", 2, "4111111111111111", "000", 11, 2020);
            res = cl.Charge("tutorvista", "0zm_5APvV2a", 10.7M, "Success", "Test", "Test", null, "Test", "AZ", "00000", null, null, null, "testAff", "testSubAff", "Test00002", 2, "4111111111111111", "000", 11, 2020);
        }

        static void TestBlueOctopus()
        {
            BlueOctopus.billing_apiSoapClient cl = new TrimFuel.WebServices.Test.BlueOctopus.billing_apiSoapClient();
            var res = cl.Charge(10.6M, "testA", "testA", "testA", null, "testA", "AZ", "00000", null, null, null, 2, "4111111111111111", "000", 11, 2020);
            res = cl.Charge(10.7M, "Success", "testA", "testA", null, "testA", "AZ", "00000", null, null, null, 2, "4111111111111111", "000", 11, 2020);
        }

        public static void GeneratePGPKeyPair(
            string publicIdentity, string passPhrase)
        {
            IAsymmetricCipherKeyPairGenerator kpg = GeneratorUtilities.GetKeyPairGenerator("RSA");

            kpg.Init(new RsaKeyGenerationParameters(
                BigInteger.ValueOf(0x10001), new SecureRandom(), 1024, 25));

            AsymmetricCipherKeyPair kp = kpg.GenerateKeyPair();

            Stream out1, out2;
            out1 = File.Create("secret.asc");
            out2 = File.Create("pub.asc");

            ExportKeyPair(out1, out2, kp.Public, kp.Private, publicIdentity, passPhrase.ToCharArray(), true);

            out1.Close();
            out2.Close();
        }

        private static void ExportKeyPair(
            Stream secretOut,
            Stream publicOut,
            AsymmetricKeyParameter publicKey,
            AsymmetricKeyParameter privateKey,
            string identity,
            char[] passPhrase,
            bool armor)
        {
            if (armor)
            {
                secretOut = new ArmoredOutputStream(secretOut);
            }

            PgpSecretKey secretKey = new PgpSecretKey(
                PgpSignature.DefaultCertification,
                PublicKeyAlgorithmTag.RsaGeneral,
                publicKey,
                privateKey,
                DateTime.UtcNow,
                identity,
                SymmetricKeyAlgorithmTag.Cast5,
                passPhrase,
                null,
                null,
                new SecureRandom()
                );

            secretKey.Encode(secretOut);

            if (armor)
            {
                secretOut.Close();
                publicOut = new ArmoredOutputStream(publicOut);
            }

            PgpPublicKey key = secretKey.PublicKey;

            key.Encode(publicOut);

            if (armor)
            {
                publicOut.Close();
            }
        }

        static void GenerateEncKey()
        {
            System.Security.Cryptography.TripleDES tdes = new System.Security.Cryptography.TripleDESCryptoServiceProvider();
            tdes.GenerateKey();
            Convert.ToBase64String(tdes.Key);
        }

        static void TestYIG()
        {
            YIG.billing_apiSoapClient cl = new TrimFuel.WebServices.Test.YIG.billing_apiSoapClient();
            var res = cl.Charge(10.6M, "testA", "testA", "testA", null, "testA", "AZ", "00000", null, null, null, 2, "4111111111111111", "000", 11, 2020);
            res = cl.Charge(10.7M, "Success", "testA", "testA", null, "testA", "AZ", "00000", null, null, null, 2, "4111111111111111", "000", 11, 2020);
        }

        static void TestInvalidTrackingNumber()
        {
            string abfRes = Utility.LoadFromEmbeddedResource(typeof(Program), "abf.xml");
            XmlSerializer s = new XmlSerializer(typeof(FindOrderResult));
            StringReader sr = new StringReader(abfRes);
            try
            {
                FindOrderResult orderList = (FindOrderResult)s.Deserialize(sr);
                IList<FoundOrder> invOrders = orderList.Orders.Where(i => string.IsNullOrEmpty(i.TrackingNumber)).ToList();
            }
            catch (Exception ex)
            {
            }
        }

        static void SalesShipped()
        {
            ABFService abf = new ABFService();
            abf.OnOrderShipped(new FoundOrder() { ProcessDate = DateTime.Now, TrackingNumber = "00134523452452345345'", ReferenceNum = "63878" });
        }

        static void AbfFoundOrders()
        {
            string abfRes = Utility.LoadFromEmbeddedResource(typeof(Program), "abf.xml");
            XmlSerializer s = new XmlSerializer(typeof(FindOrderResult));
            StringReader sr = new StringReader(abfRes);
            try
            {
                FindOrderResult orderList = (FindOrderResult)s.Deserialize(sr);
            }
            catch (Exception ex)
            {
            }
        }

        static void TestABF()
        {
            ABFService abf = new ABFService();
            abf.CheckShipmentsState();
        }

        static void TestFraudService()
        {
            SaleWebService.SaleService_ c = new SaleWebService.SaleService_();
            c.ValidateFraud(62795);
        }

        static void TestBadCustomer()
        {
            IBadCustomerGateway g = new BadCustomerGateway();
            Billing b = new Billing()
            {
                FirstName = "TestFirst",
                LastName = "TestLast",
                CreditCard = "4111111111111111"
            };
            BusinessError<GatewayResult> res = g.ValidateCustomer(b);
            Console.WriteLine(res.ReturnValue.Request);
            Console.WriteLine(res.ReturnValue.Response);

            //transactionId
            //error
            //found
            //result
        }


        static void TestBadCustomerService()
        {
            SaleWebService.SaleService_ c = new TrimFuel.WebServices.Test.SaleWebService.SaleService_();
            c.ValidateCustomer(37);
        }

        static void TestCancellationEmails()
        {
            //(new EmailService()).SendCancellationEmailsOnCurrentDateAndHour();
        }

        static void TestReadSubscription()
        {
            Subscription s = (new BaseService()).Load<Subscription>(228);
        }

        static void TestBL()
        {
            SubscriptionService s = new SubscriptionService();
            s.AssignPlan(4, 5, 228);
        }

        static void TestAssignPlan()
        {
            SaleWebService.SaleService_ c = new TrimFuel.WebServices.Test.SaleWebService.SaleService_();

            //Console.WriteLine(c.AssignPlan(null, 5, 227));
            //Console.WriteLine(c.AssignPlan(1, 5, 228));
            //Console.WriteLine(c.AssignPlan(null, 5, 228));
            Console.WriteLine(c.AssignPlan(4, 5, 228));
        }

        static void TestRegistration()
        {
            SaleWebService.SaleService_ c = new TrimFuel.WebServices.Test.SaleWebService.SaleService_();

            Console.WriteLine(
                c.UpdateRegistration(9, null, "Sergei", "Kozakov", "Address1", null, "Grodno", null, "230000", "test@test.com", 
                    "+375 29 ...", DateTime.Now, "Aff", "Sub", "192.168...", "www.tut.by"));

            Console.WriteLine(
                c.UpdateRegistration(19, null, "Sergei", "Kozakov", "Address1", null, "Grodno", null, "230000", "test@test.com",
                    "+375 29 ...", DateTime.Now, "Aff", "Sub", "192.168...", "www.tut.by"));

            Console.WriteLine(
                c.UpdateRegistration(9, 3, "Sergei", "Kozakov", "Address1", null, "Grodno", null, "230000", "test@test.com",
                    "+375 29 ...", DateTime.Now, "Aff", "Sub", "192.168...", "www.tut.by"));
        }

        static void TestBilling()
        {
            SaleWebService.SaleService_ c = new TrimFuel.WebServices.Test.SaleWebService.SaleService_();
            
            Console.WriteLine(
                c.UpdateBilling(5, 9, null,
                    "Sergei", "Kozakov", "34523452345", "123", 1, 8, 2010, "Address1", null, "Grodno", null, "230000", "Belarus", 
                    "test@test.com", "+375 29 ...", DateTime.Today, "Aff", "Aub", "192.168.0.0", "www.tut.by"));

            Console.WriteLine(
                c.UpdateBilling(5, 9, null,
                    "SergeiSergeiSergeiSergeiSergeiSergeiSergeiSergeiSergeiSergeiSergeiSergeiSergeiSergeiSergeiSergei", 
                    "Kozakov", "34523452345", "123", 1, 8, 2010, "Address1", null, "Grodno", null, "230000", "Belarus",
                    "test@test.com", "+375 29 ...", DateTime.Today, "Aff", "Aub", "192.168.0.0", "www.tut.by"));

            Console.WriteLine(
                c.UpdateBilling(55, 9, null,
                    "Sergei",
                    "Kozakov", "34523452345", "123", 1, 8, 2010, "Address1", null, "Grodno", null, "230000", "Belarus",
                    "test@test.com", "+375 29 ...", DateTime.Today, "Aff", "Aub", "192.168.0.0", "www.tut.by"));
        }
    }
}
