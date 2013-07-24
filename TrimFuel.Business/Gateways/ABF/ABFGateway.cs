using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Containers;
using TrimFuel.Model;
using TrimFuel.Model.Enums;
using TrimFuel.Business.Utils;
using TrimFuel.Model.Views;
using System.Xml.Serialization;
using System.IO;

namespace TrimFuel.Business.Gateways.ABF
{
    public class ABFGateway : ShipmentGatewayOneByOne
    {
        private const string TEST_MODE_URL = "http://localhost/gateways/abf/";
        protected const int ABF_CHECK_SHIPMENTS_SATE_LIMIT = 15000;

        protected override ShipmentGatewayOneByOne.SubmitResult SubmitShipment(Person person, Address address, string email, string phone, 
            IList<ShipmentGatewayBase.Shipment> products,
            IList<int> shippingOptionList, Product productGroup, DateTime orderDate, long uniqueID, 
            SubmitAdditionalData additionalData,
            IDictionary<ShipperConfig.ID, ShipperConfig> config, bool testMode)
        {
            ABFApi.TransactionInfo transInfo = new ABFApi.TransactionInfo()
            {
                ReferenceNum = uniqueID.ToString()
            };

            ABFApi.Address addr = new TrimFuel.Business.ABFApi.Address()
            {
                Address1 = address.Address1,
                Address2 = address.Address2,
                City = address.City,
                State = address.State,
                Zip = address.Zip,
                Country = address.Country
            };

            ABFApi.ContactInfo shipTo = new ABFApi.ContactInfo()
            {
                Name = person.FullName,
                CompanyName = string.Empty,
                Address = addr,
                PhoneNumber1 = phone,
                //EmailAddress1 = email,
                CustomerName = person.FullName
            };

            ABFApi.ShippingInstructions instructions = new ABFApi.ShippingInstructions()
            {
                Carrier = "USPS",
                Mode = shippingOptionList.Contains(ShippingOptionEnum.PriorityShipping) ? "PRIORITY" : "First-Class Parcel",
                BillingCode = "Prepaid",
                Account = "01E779"
            };

            // CoAction specific fields
            if (Config.Current.APPLICATION_ID == ApplicationEnum.CoActionCRM)
            {
                shipTo.CompanyName = person.FullName;
                instructions.Carrier = "CanadaPost";
            }

            string notes = null;
            if (productGroup != null && productGroup.ProductName != null)
            {
                if (productGroup.ProductName.Contains("e-Cigarettes"))
                    notes = "Ecig";
                else if (productGroup.ProductName.Contains("Kaboom"))
                    notes = "Kaboom";
                else if (productGroup.ProductName.Contains("Wrinkle Rewind"))
                    notes = "Beauty";
            }

            IList<ABFApi.OrderLineItem> orderLineItemList = new List<ABFApi.OrderLineItem>();
            foreach (var p in products)
            {
                orderLineItemList.Add(new ABFApi.OrderLineItem()
                {
                    SKU = p.SKU,
                    Qty = p.Quantity,
                    Packed = 0M
                });
            }

            ABFApi.Order order = new ABFApi.Order()
            {
                TransInfo = transInfo,
                ShippingInstructions = instructions,
                ShipTo = shipTo,
                Notes = notes,
                OrderLineItems = orderLineItemList.ToArray()
            };

            ABFApi.ExternalLoginData loginData = new ABFApi.ExternalLoginData()
            {
                ThreePLKey = GetConfig_ThreePLKey(config),
                Login = GetConfig_Login(config),
                Password = GetConfig_Password(config),
                FacilityID = GetConfig_FacilityID(config)
            };

            string request = "Can't serialize to XML";
            try
            {
                request = Utility.XmlSerializeUTF8(order);
            }
            catch {}

            string response = "";
            string regID = null;

            if (testMode)
            {
                try
                {
                    response = HTTPPost(TEST_MODE_URL + "CreateOrders.aspx", 
                        "extLoginData=" + Utility.XmlSerializeUTF8(loginData) + "&" +
                        "orders=" + request);
                    regID = uniqueID.ToString();
                }
                catch (Exception ex)
                {
                    response = ex.ToString();
                }
            }
            else
            {
                using (ABFApi.ServiceExternal api = CreateService(4 * 60 * 1000))
                {
                    try
                    {
                        api.CreateOrders(loginData, new ABFApi.Order[] { order }, ref response);
                        regID = uniqueID.ToString();
                    }
                    catch (Exception ex)
                    {
                        response = "Error: " + ex.Message;
                    }
                }
            }

            return new SubmitResult()
            {
                Request = request,
                Response = response,
                ShipperRegID = regID
            };
        }

        public override bool IsCheckShippedImplemented
        {
            get
            {
                return true;
            }
        }

        public override IList<ShipmentGatewayResult<ShipmentPackageShipResult>> CheckShipped(IList<string> shipperRegIDList, IDictionary<ShipperConfig.ID, ShipperConfig> config, bool testMode, CanContinueCallback canContinue)
        {
            IList<ShipmentGatewayResult<ShipmentPackageShipResult>> res = new List<ShipmentGatewayResult<ShipmentPackageShipResult>>();

            ABFApi.UserLoginData userLoginData = new ABFApi.UserLoginData()
            {
                ThreePLID = GetConfig_ThreePLID(config),
                Login = GetConfig_Login(config),
                Password = GetConfig_Password(config)
            };

            ABFApi.FindOrderCriteria focr = new ABFApi.FindOrderCriteria()
            {
                OverAlloc = ABFApi.FindTriStateType.Any,
                Closed = ABFApi.FindTriStateType.Any,
                ASNSent = ABFApi.FindTriStateType.Any,
                RouteSent = ABFApi.FindTriStateType.Any,
                FacilityID = GetConfig_FacilityID(config),
                DateRangeType = ABFApi.FindOrderDateRangeType.Confirm,
            };
                
            int totalOrders = 0;
            // 4 * 5 last days
            int daysCount = 5;
            int periodsCount = 4;

            for (int i = 0; i < periodsCount; i++)
            {
                focr.BeginDate = new TrimFuel.Business.ABFApi.I18nDateTime() { Value = DateTime.Now.AddDays(0 - (i + 1) * daysCount) };
                focr.EndDate = new TrimFuel.Business.ABFApi.I18nDateTime() { Value = DateTime.Now.AddDays(0 - i * daysCount) };

                ShipmentGatewayResult<ShipmentPackageShipResult> res1 = new ShipmentGatewayResult<ShipmentPackageShipResult>();
                try
                {
                    res1.Request = Utility.XmlSerializeUTF8(focr);
                }
                catch (Exception ex)
                {
                    res1.Request = "Can't serialize to XML";
                }                
                res1.PackageList = new List<ShipmentPackageShipResult>();

                if (testMode)
                {
                    try
                    {
                        string response = HTTPPost(TEST_MODE_URL + "FindOrders.aspx",
                            "userLoginData=" + Utility.XmlSerializeUTF8(userLoginData) + "&" +
                            "focr=" + res1.Request + "&" +
                            "limitCount=" + ABF_CHECK_SHIPMENTS_SATE_LIMIT
                            );
                        //don't store in DB, Response is too large (~5Mb for 1 request)
                        //res1.Response = response;
                        res1.Response = "Tracking numbers received.";
                        FindOrderResult apiRes = ConvertResult(response);
                        res1.PackageList = (
                            from regID in shipperRegIDList
                            join order in apiRes.Orders.Where(j => !string.IsNullOrEmpty(j.TrackingNumber) && j.ReferenceNumLong != null) on regID equals order.ReferenceNum
                            select new ShipmentPackageShipResult()
                            {
                                ShipperRegID = regID,
                                TrackingNumber = order.TrackingNumber,
                                ShipDT = order.ProcessDate
                            }).ToList();
                    }
                    catch (Exception ex)
                    {
                        res1.Response = ex.ToString();
                    }
                }
                else
                {
                    using (ABFApi.ServiceExternal apiService = new ABFApi.ServiceExternal())
                    {
                        apiService.Timeout = 1000 * 60 * 10;
                        try
                        {
                            string response = apiService.FindOrders(userLoginData, focr, ABF_CHECK_SHIPMENTS_SATE_LIMIT, out totalOrders);
                            //don't store in DB, Response is too large (~5Mb for 1 request)
                            //res1.Response = response;
                            res1.Response = "Tracking numbers received.";
                            FindOrderResult apiRes = ConvertResult(response);
                            res1.PackageList = (
                                from regID in shipperRegIDList
                                join order in apiRes.Orders.Where(j => !string.IsNullOrEmpty(j.TrackingNumber) && j.ReferenceNumLong != null) on regID equals order.ReferenceNum
                                select new ShipmentPackageShipResult()
                                {
                                    ShipperRegID = regID,
                                    TrackingNumber = order.TrackingNumber,
                                    ShipDT = order.ProcessDate
                                }).ToList();
                        }
                        catch (Exception ex)
                        {
                            res1.Response = ex.ToString();
                        }
                    }
                }
                res.Add(res1);

                //back control
                if (canContinue != null && !canContinue((i + 1) / periodsCount))
                {
                    break;
                }
            }
            
            return res;
        }

        private ABFApi.ServiceExternal CreateService(int timeOut)
        {
            ABFApi.ServiceExternal res = new ABFApi.ServiceExternal();
            res.Timeout = timeOut;
            return res;
        }

        protected FindOrderResult ConvertResult(string findOrderResult)
        {
            FindOrderResult res = null;
            using (StringReader sr = new StringReader(findOrderResult))
            {
                XmlSerializer s = new XmlSerializer(typeof(FindOrderResult));
                res = (FindOrderResult)s.Deserialize(sr);
            }
            return res;
        }

        protected override string FixState(string state)
        {
            return base.FixState(state).Replace("APO-", "");
        }

        protected override string FixCountry(string country)
        {
            if (string.IsNullOrEmpty(country))
                return DEFAULT_COUNTRY;

            if (country.ToLower() == "us" ||
                country.ToLower() == "usa")
            {
                return "United States";
            }

            if (country.ToLower() == "ca" ||
                country.ToLower() == "can")
            {
                return "Canada";
            }

            if (country.ToLower() == "au" ||
                country.ToLower() == "aus")
            {
                return "Australia";
            }

            return country;
        }

        protected virtual string GetConfig_ThreePLKey(IDictionary<ShipperConfig.ID, ShipperConfig> config)
        {
            return config[ShipperConfigEnum.ABF_ThreePLKey].Value;
        }
        protected virtual string GetConfig_Login(IDictionary<ShipperConfig.ID, ShipperConfig> config)
        {
            return config[ShipperConfigEnum.ABF_Login].Value;
        }
        protected virtual string GetConfig_Password(IDictionary<ShipperConfig.ID, ShipperConfig> config)
        {
            return config[ShipperConfigEnum.ABF_Password].Value;
        }
        protected virtual int GetConfig_FacilityID(IDictionary<ShipperConfig.ID, ShipperConfig> config)
        {
            return Convert.ToInt32(config[ShipperConfigEnum.ABF_FacilityID].Value);
        }
        protected virtual int GetConfig_ThreePLID(IDictionary<ShipperConfig.ID, ShipperConfig> config)
        {
            return Convert.ToInt32(config[ShipperConfigEnum.ABF_ThreePLID].Value);
        }        
    }
}
