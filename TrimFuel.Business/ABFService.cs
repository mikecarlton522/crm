using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using MySql.Data.MySqlClient;
using TrimFuel.Business.Gateways;
using System.Xml.Serialization;
using TrimFuel.Business.Utils;
using TrimFuel.Business.Gateways.ABF;
using System.IO;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business
{
    public class ABFService : ShipperService
    {
        //TODO: to config
        protected const int ABF_CHECK_SHIPMENTS_SATE_LIMIT = 15000;

        protected SaleService saleService { get { return new SaleService(); } }

        public override int ShipperID
        {
            get { return TrimFuel.Model.Enums.ShipperEnum.ABF; }
        }

        public override string TableName
        {
            get { return "ABFRecord"; }
        }

        public override bool SendShippingOrderInternal(Registration r, Billing b, IList<SaleShipperView> saleList)
        {
            bool res = false;
            try
            {
                var cfg = GetAndCheckConfig();

                string companyName = "";
                long? regID = saleList[0].SaleID;

                ABFApi.TransactionInfo transInfo = new ABFApi.TransactionInfo()
                {
                    ReferenceNum = regID.ToString()
                };

                string country = DEFAULT_COUNTRY;
                RegistrationInfo rInfo = (new RegistrationService()).GetRegistrationInfo(r.RegistrationID.Value);
                if (rInfo != null && !string.IsNullOrEmpty(rInfo.Country))
                {
                    country = rInfo.Country;
                }

                ABFApi.Address address = new TrimFuel.Business.ABFApi.Address()
                {
                    Address1 = r.Address1,
                    Address2 = r.Address2,
                    City = r.City,
                    State = FixABFState(r.State),
                    Zip = r.Zip,
                    Country = FixABFCountry(country)
                };

                ABFApi.ContactInfo shipTo = new ABFApi.ContactInfo()
                {
                    Name = r.FullName,
                    CompanyName = companyName,
                    Address = address,
                    PhoneNumber1 = r.Phone,
                    //EmailAddress1 = r.Email,
                    CustomerName = r.FullName
                };

                bool priority = false;
                foreach (SaleShipperView sale in saleList)
                {
                    if (sale.SaleID != null)
                    {
                        if (IsPriority((long)sale.SaleID))
                        {
                            priority = true;
                            break;
                        }
                    }
                }
                
                ABFApi.ShippingInstructions instructions = new ABFApi.ShippingInstructions()
                {
                    Carrier = "USPS",
                    Mode = priority ? "PRIORITY" : "First-Class Parcel",
                    BillingCode = "Prepaid",
                    Account = "01E779"
                };

                // CoAction specific fields
                if (Config.Current.APPLICATION_ID == ApplicationEnum.CoActionCRM)
                {
                    shipTo.CompanyName = r.FullName;
                    instructions.Carrier = "CanadaPost";
                }

                //TODO, hardcoded notes
                string notes = null;
                if (saleList.Count > 0 && saleList[0].ProductCode.StartsWith("ABF-KA") ||
                    saleList.Count > 0 && saleList[0].ProductCode.StartsWith("VG-KA") ||
                    saleList.Count > 0 && saleList[0].ProductCode.StartsWith("ABF-MSCK"))
                {
                    notes = "Kaboom";
                }
                else if (saleList.Count > 0 && saleList[0].ProductCode.StartsWith("VG-"))
                {
                    notes = "Ecig";
                }
                else if (saleList.Count > 0 && saleList[0].ProductCode.StartsWith("BT-"))
                {
                    notes = "Beauty";
                }
                ABFApi.Order order = new ABFApi.Order()
                {
                    TransInfo = transInfo,
                    ShippingInstructions = instructions,
                    ShipTo = shipTo,
                    Notes = notes
                };

                IList<ABFApi.OrderLineItem> orderLineItemList = new List<ABFApi.OrderLineItem>();
                foreach (SaleShipperView sale in saleList)
                {
                    IList<InventoryView> invList = inventoryService.GetInventoryListForShipping(sale.ProductCode, ShipperID);
                    //remove validation, sales can contain Inventories with InventoryType = Service
                    //if (invList.Count == 0)
                    //{
                    //    throw new Exception(string.Format("Can't find Inventories for ProductCode({0}), Sale({1})", sale.ProductCode, sale.SaleID));
                    //}
                    foreach (InventoryView inv in invList)
                    {
                        orderLineItemList.Add(GetOrderLineItem(sale, inv));
                    }
                }

                if (orderLineItemList.Count == 0)
                {
                    throw new Exception(string.Format("Can't find Inventories for ProductCodes({0}), Sales({1})", string.Join(",", saleList.Select(i => i.ProductCode).ToArray()), string.Join(",", saleList.Select(i => i.SaleID.ToString()).ToArray())));
                }

                order.OrderLineItems = orderLineItemList.ToArray();

                long? errId = null;
                string response = "Accepted";
                if (!Config.Current.SHIPPING_TEST_MODE)
                {
                    ABFApi.ServiceExternal apiService = new ABFApi.ServiceExternal();
                    string warnings = "";
                    try
                    {
                        errId = apiService.CreateOrders(GetLoginData(cfg), new ABFApi.Order[] { order }, ref warnings);
                        response = warnings;
                        res = true;
                    }
                    catch (Exception ex)
                    {
                        regID = null;
                        errId = null;
                        response = "Error: " + ex.Message;
                    }
                }
                else
                {
                    response = "Test";
                    res = true;
                    //Random rnd = new Random();
                    //regId = 1000000000 +  (long)(10000.0 * rnd.NextDouble());
                }

                OnOrderSent(saleList, order, response, regID);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = false;
            }
            return res;
        }

        protected virtual ABFApi.OrderLineItem GetOrderLineItem(SaleShipperView sale, InventoryView inv)
        {
            return new ABFApi.OrderLineItem()
                        {
                            SKU = FixSKU(inv.SKU),
                            Qty = Convert.ToDecimal(sale.Quantity * inv.Quantity),
                            Packed = 0M //Convert.ToDecimal(sale.Quantity * inv.Quantity)
                        };
        }

        private string FixSKU(string sku)
        {
            //if (sku == null)
            //    return null;
            //if (sku == "ABF-START")
            //    return "ABF-STARTBXMINI";
            //if (sku.StartsWith("BT-"))
            //    return "ABF-" + sku.Substring(3);
            return sku;
        }

        protected virtual void OnOrderSent(IList<SaleShipperView> saleList, ABFApi.Order order, string response, long? regID)
        {
            try
            {
                dao.BeginTransaction();

                foreach (SaleShipperView sale in saleList)
                {
                    ABFRecord abfRecord = new ABFRecord();

                    abfRecord.SaleID = sale.SaleID;
                    try
                    {
                        abfRecord.Request = Utility.XmlSerializeUTF8(order);
                    }
                    catch
                    {
                        abfRecord.Request = "Can't serialize to XML";
                    }
                    abfRecord.Response = response;
                    abfRecord.RegID = regID;
                    abfRecord.Completed = false;
                    abfRecord.CreateDT = DateTime.Now;

                    dao.Save<ABFRecord>(abfRecord);

                    OnOrderSentBase(abfRecord);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }
        }

        protected string FixABFState(string state)
        {
            if (string.IsNullOrEmpty(state))
                return state;

            return state.Replace("APO-", "");
        }

        protected string FixABFCountry(string country)
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

        public override void CheckShipmentsState()
        {
            if (Config.Current.SHIPPING_TEST_MODE)
                return;

            try
            {
                var cfg = GetAndCheckConfig();

                ABFApi.FindOrderCriteria focr = new ABFApi.FindOrderCriteria()
                {
                    OverAlloc = ABFApi.FindTriStateType.Any,
                    Closed = ABFApi.FindTriStateType.Any,
                    ASNSent = ABFApi.FindTriStateType.Any,
                    RouteSent = ABFApi.FindTriStateType.Any,
                    FacilityID = GetConfig_FacilityID(cfg),
                    DateRangeType = ABFApi.FindOrderDateRangeType.Confirm,
                };

                using (ABFApi.ServiceExternal apiService = new ABFApi.ServiceExternal())
                {
                    apiService.Timeout = 1000 * 60 * 10;
                    int totalOrders = 0;
                    // 4 * 5 last days
                    int daysCount = 5;
                    int periodsCount = 4;

                    for (int i = 0; i < periodsCount; i++)
                    {
                        focr.BeginDate = new TrimFuel.Business.ABFApi.I18nDateTime() { Value = DateTime.Now.AddDays(0 - (i + 1) * daysCount) };
                        focr.EndDate = new TrimFuel.Business.ABFApi.I18nDateTime() { Value = DateTime.Now.AddDays(0 - i * daysCount) };

                        FindOrderResult apiRes = ConvertResult(apiService.FindOrders(GetUserLoginData(cfg), focr, ABF_CHECK_SHIPMENTS_SATE_LIMIT, out totalOrders));
                        foreach (FoundOrder order in apiRes.Orders)
                        {
                            if (!string.IsNullOrEmpty(order.TrackingNumber) && order.ReferenceNumLong != null)
                            {
                                OnOrderShipped(order);
                            }
                        }
                    }
                }

                //MySqlCommand q = new MySqlCommand("select abf.* from ABFRecord abf " +
                //    "where abf.Completed = 0 and abf.RegID is not null " +
                //    "order by abf.CreateDT " +
                //    "limit 1");
                //ABFRecord firstNotSent = dao.Load<ABFRecord>(q).FirstOrDefault();
                //if (firstNotSent != null)
                //{

                //    ABFApi.FindOrderCriteria focr = new ABFApi.FindOrderCriteria()
                //    {
                //        OverAlloc = ABFApi.FindTriStateType.Any,
                //        Closed = ABFApi.FindTriStateType.Any,
                //        ASNSent = ABFApi.FindTriStateType.Any,
                //        RouteSent = ABFApi.FindTriStateType.Any,
                //        FacilityID = Config.Current.ABF_FACILITY_ID.Value,
                //        DateRangeType = ABFApi.FindOrderDateRangeType.Confirm,
                //        BeginDate = new TrimFuel.Business.ABFApi.I18nDateTime() { Value = firstNotSent.CreateDT.Value.AddDays(-2) },
                //        EndDate = new TrimFuel.Business.ABFApi.I18nDateTime() { Value = DateTime.Now }
                //    };

                //    int totalOrders = 0;
                //    ABFApi.ServiceExternal apiService = new ABFApi.ServiceExternal();
                //    apiService.Timeout = 1000 * 60 * 10;
                //    FindOrderResult apiRes = ConvertResult(apiService.FindOrders(UserLoginData, focr, ABF_CHECK_SHIPMENTS_SATE_LIMIT, out totalOrders));
                //    foreach (FoundOrder order in apiRes.Orders)
                //    {
                //        if (!string.IsNullOrEmpty(order.TrackingNumber) && order.ReferenceNumLong != null)
                //        {
                //            OnOrderShipped(order);
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public override void CheckReturns()
        {
            throw new NotImplementedException();
        }

        protected ABFApi.ExternalLoginData GetLoginData(IDictionary<ShipperConfig.ID, ShipperConfig> config)
        {
            return new ABFApi.ExternalLoginData()
            {
                ThreePLKey = GetConfig_ThreePLKey(config),
                Login = GetConfig_Login(config),
                Password = GetConfig_Password(config),
                FacilityID = GetConfig_FacilityID(config)
            };
        }

        protected virtual ABFApi.UserLoginData GetUserLoginData(IDictionary<ShipperConfig.ID, ShipperConfig> config)
        {
            return new ABFApi.UserLoginData()
            {
                ThreePLID = GetConfig_ThreePLID(config),
                Login = GetConfig_Login(config),
                Password = GetConfig_Password(config)
            };
        }

        protected FindOrderResult ConvertResult(string findOrderResult)
        {
            StringReader sr = new StringReader(findOrderResult);
            XmlSerializer s = new XmlSerializer(typeof(FindOrderResult));
            return (FindOrderResult)s.Deserialize(sr);
        }

        public void OnOrderShipped(FoundOrder order)
        {
            //Invalid ReferenceNum
            if (order.ReferenceNumLong == null)
                return;
            OnOrderShipped(order.ReferenceNumLong.Value, order.TrackingNumber, order.ProcessDate);
        }

        public override void OnOrderShipped(long regID, string trackingNumber, DateTime shippedDate)
        {
            OnOrderShippedBase<ABFRecord>(regID, trackingNumber, shippedDate);
        }

        public override void OnTrackingNumberUpdated(long regID, string trackingNumber, DateTime shippedDate)
        {
            OnTrackingNumberUpdatedBase<ABFRecord>(regID, trackingNumber, shippedDate);
        }

        public override void OnOrderReturned(long regID, string reason, DateTime? returnDate)
        {
            throw new NotImplementedException();
        }

        private bool IsPriority(long saleID)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("select count(*) as Count from SaleShippingOption where ShippingOptionID = 1 and SaleID = @SaleID");
                cmd.Parameters.AddWithValue("@SaleID", saleID);

                int n = (int)dao.ExecuteScalar<int>(cmd);

                return n > 0;
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                return false;
            }
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
