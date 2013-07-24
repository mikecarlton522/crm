using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Business.Utils;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Views;
using TrimFuel.Model.Utility;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business
{
    public class RefererService : BaseService
    {
        //TODO: In config
        private const decimal REFERER_PRIMARY_SALE_COMMISSION = 0.2M;
        private const decimal REFERER_SECONDARY_SALE_COMMISSION = 0.04M;
        public const int REFERER_COMMISSION_INCUBATION_PERIOD = 45;
        public const decimal REFERER_COMMISSION_MIN_FOR_CASH = 100M;
        private const decimal ECIGS_DOLLAR_TO_USD_CONVERSION_RATE = 0.5M;

        private ShipperService shipperService { get { return new GeneralShipperService(); } }
        private EmailService emailService { get { return new EmailService(); } }

        //Const
        private const string DEFAULT_COUNTRY = "US";

        public BusinessError<Referer> CreateReferer(string firstName, string lastName, string company,
            string address1, string address2, string city, string state, string zip, string country,
            string refererCode, int? parentRefererID, string username, string password)
        {
            BusinessError<Referer> res = new BusinessError<Referer>(null, BusinessErrorState.Error, null);
            try
            {
                dao.BeginTransaction();

                if (string.IsNullOrEmpty(username))
                {
                    res.ErrorMessage = "Login is not specified";
                }
                else if (string.IsNullOrEmpty(refererCode))
                {
                    res.ErrorMessage = "Referer Code is not specified";
                }
                else if (GetByLogin(username) != null)
                {
                    res.ErrorMessage = "Login is already occupied";
                }
                else if (GetByCode(refererCode) != null)
                {
                    res.ErrorMessage = "Referer Code is already occupied";
                }
                else
                {
                    Referer rerefer = new Referer();
                    rerefer.FirstName = firstName;
                    rerefer.LastName = lastName;
                    rerefer.Company = company;
                    rerefer.Address1 = address1;
                    rerefer.Address2 = address2;
                    rerefer.City = city;
                    rerefer.State = state;
                    rerefer.Zip = zip;
                    rerefer.Country = (!string.IsNullOrEmpty(country)) ? country : DEFAULT_COUNTRY;
                    rerefer.RefererCode = refererCode;
                    rerefer.ParentRefererID = parentRefererID;
                    rerefer.Username = username;
                    rerefer.Password = password;
                    rerefer.CreateDT = DateTime.Now;
                    dao.Save<Referer>(rerefer);

                    res.ReturnValue = rerefer;
                    res.State = BusinessErrorState.Success;
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();

                res.ReturnValue = null;
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = "Unknown error occured";
            }
            return res;
        }

        public BusinessError<Referer> CreateReferer(string firstName, string lastName, string company,
            string address1, string address2, string city, string state, string zip, string country,
            string refererCode, string parentRefererCode, string username, string password)
        {
            BusinessError<Referer> res = new BusinessError<Referer>(null, BusinessErrorState.Error, null);
            try
            {
                Referer parentReferer = null;
                if (!string.IsNullOrEmpty(parentRefererCode))
                {
                    parentReferer = GetByCode(parentRefererCode);
                }

                res = CreateReferer(firstName, lastName, company,
                    address1, address2, city, state, zip, country,
                    refererCode, (parentReferer != null) ? parentReferer.RefererID : null,
                    username, password);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);

                res.ReturnValue = null;
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = "Unknown error occured";
            }
            return res;
        }

        public BusinessError<Referer> UpdateReferer(int? refererID, string firstName, string lastName, string company,
            string address1, string address2, string city, string state, string zip, string country,
            string refererCode, int? parentRefererID, string username, string password)
        {
            BusinessError<Referer> res = new BusinessError<Referer>(null, BusinessErrorState.Error, null);
            try
            {
                dao.BeginTransaction();

                if (string.IsNullOrEmpty(username))
                {
                    res.ErrorMessage = "Login is not specified";
                }
                else if (string.IsNullOrEmpty(refererCode))
                {
                    res.ErrorMessage = "Referer Code is not specified";
                }
                else
                {
                    Referer temp = GetByLogin(username);
                    if (temp != null && temp.RefererID != refererID)
                    {
                        res.ErrorMessage = "Login is already occupied";
                    }
                    else
                    {
                        temp = GetByCode(refererCode);
                        if (temp != null && temp.RefererID != refererID)
                        {
                            res.ErrorMessage = "Referer Code is already occupied";
                        }
                        else
                        {
                            Referer referer = EnsureLoad<Referer>(refererID);
                            referer.FirstName = firstName;
                            referer.LastName = lastName;
                            referer.Company = company;
                            referer.Address1 = address1;
                            referer.Address2 = address2;
                            referer.City = city;
                            referer.State = state;
                            referer.Zip = zip;
                            referer.Country = (!string.IsNullOrEmpty(country)) ? country : DEFAULT_COUNTRY;
                            referer.RefererCode = refererCode;
                            referer.ParentRefererID = parentRefererID;
                            referer.Username = username;
                            referer.Password = password;
                            dao.Save<Referer>(referer);

                            res.ReturnValue = referer;
                            res.State = BusinessErrorState.Success;
                        }
                    }
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();

                res.ReturnValue = null;
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = "Unknown error occured";
            }
            return res;
        }

        public BusinessError<Referer> CreateOrGetRefererFromBilling(Billing billing, Referer parentReferer)
        {
            return CreateOrGetRefererFromBilling(billing, parentReferer, null);
        }

        public BusinessError<Referer> CreateOrGetRefererFromBilling(Billing billing, Referer parentReferer, string password)
        {
            BusinessError<Referer> res = new BusinessError<Referer>(null, BusinessErrorState.Error, null);

            try
            {
                Referer referer = GetOwnRefererByBilling(billing.BillingID.Value);
                if (referer == null)
                {
                    //Try Get Referer by Email
                    referer = GetByLogin(billing.Email);
                    if (referer == null)
                    {
                        //Create new referer
                        if (string.IsNullOrEmpty(password))
                            password = Utility.Password(new Random(), 6);
                        res = CreateReferer(billing.FirstName, billing.LastName, null,
                            billing.Address1, billing.Address2, billing.City, billing.State, billing.Zip, billing.Country,
                            billing.BillingID.ToString(), (parentReferer != null) ? parentReferer.RefererID : null,
                            billing.Email, password);
                    }
                }

                if (referer != null)
                {
                    res.State = BusinessErrorState.Success;
                    res.ReturnValue = referer;
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public RefererBilling AddBillingToReferer(Referer referer, Billing billing)
        {
            RefererBilling res = null;
            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand("select * from RefererBilling " +
                    "where RefererID = @refererID and BillingID = @billingID");
                q.Parameters.Add("@refererID", MySqlDbType.Int32).Value = referer.RefererID;
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billing.BillingID;

                res = dao.Load<RefererBilling>(q).FirstOrDefault();
                if (res == null)
                {
                    res = new RefererBilling();
                }

                res.RefererID = referer.RefererID;
                res.BillingID = billing.BillingID;

                dao.Save<RefererBilling>(res);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res = null;
            }
            return res;
        }

        public Referer GetOwnRefererByBilling(long billingID)
        {
            Referer res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select r.* from Referer r " +
                    "inner join RefererBilling rb on rb.RefererID = r.RefererID " +
                    "where rb.BillingID = @billingID");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;

                res = dao.Load<Referer>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<Billing> GetBillingByReferer(int refererID)
        {
            IList<Billing> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select b.* from Billing b " +
                    "inner join RefererBilling rb on rb.BillingID = b.BillingID " +
                    "where rb.RefererID = @refererID " +
                    "order by b.BillingID");
                q.Parameters.Add("@refererID", MySqlDbType.Int32).Value = refererID;

                res = dao.Load<Billing>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public Billing GetLastBillingByReferer(int refererID)
        {
            Billing res = null;
            try
            {
                IList<Billing> billingList = GetBillingByReferer(refererID);
                if (billingList != null)
                {
                    res = billingList.LastOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public Referer GetByLogin(string username, string password)
        {
            Referer res = null;

            try
            {
                MySqlCommand q = new MySqlCommand("select * from Referer " +
                    "where Username = @username and Password = @password");
                q.Parameters.Add("@username", MySqlDbType.VarChar).Value = username;
                q.Parameters.Add("@password", MySqlDbType.VarChar).Value = password;

                res = dao.Load<Referer>(q).FirstOrDefault();

                if (res != null && res.Password != password)
                {
                    res = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public Referer GetByLogin(string username)
        {
            Referer res = null;

            try
            {
                MySqlCommand q = new MySqlCommand("select * from Referer " +
                    "where Username = @username");
                q.Parameters.Add("@username", MySqlDbType.VarChar).Value = username;

                res = dao.Load<Referer>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public Referer GetByRecoveryInfo(string username, string firstName, string lastName, string city, string zip)
        {
            Referer res = null;

            try
            {
                MySqlCommand q = new MySqlCommand("select * from Referer " +
                    "where Username = @username " +
                    "and FirstName = @firstName " +
                    "and LastName = @lastName " +
                    "and City = @city " +
                    "and Zip = @zip");
                q.Parameters.Add("@username", MySqlDbType.VarChar).Value = username;
                q.Parameters.Add("@firstName", MySqlDbType.VarChar).Value = firstName;
                q.Parameters.Add("@lastName", MySqlDbType.VarChar).Value = lastName;
                q.Parameters.Add("@city", MySqlDbType.VarChar).Value = city;
                q.Parameters.Add("@zip", MySqlDbType.VarChar).Value = zip;

                res = dao.Load<Referer>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public bool RecoverRefererPasswordEcigs(string username)
        {
            bool res = false;
            Referer referer = null;
            try
            {
                dao.BeginTransaction();

                referer = GetByLogin(username);
                referer.Password = Utility.Password(new Random(), 6);
                dao.Save<Referer>(referer);
                res = true;

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res = false;
            }

            if (res)
            {
                emailService.SendRefererPassword(referer, 10);
            }

            return res;
        }

        public Referer GetByCode(string refererCode)
        {
            if (string.IsNullOrEmpty(refererCode))
            {
                return null;
            }

            Referer res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select r.* from Referer r " +
                    "where r.RefererCode = @refererCode");
                q.Parameters.Add("@refererCode", MySqlDbType.VarChar).Value = refererCode;

                res = dao.Load<Referer>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<Referer> GetRefererList()
        {
            IList<Referer> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select r.* from Referer r");
                res = dao.Load<Referer>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        private IList<SaleView> GetOwnSales(int refererID)
        {
            IList<SaleView> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select i.InventoryID, i.Product as InventoryProduct, (ifnull(pci.Quantity, 1) * ifnull(bsale.Quantity, 1)) as InventoryQuantity, s.SaleID, s.CreateDT as SaleCreateDT, ch.Amount as SaleChargeAmount, null as SaleShippedDT, s.TrackingNumber as SaleTrackingNumber, null as SaleReturnDT, b.BillingID, b.FirstName as BillingFirstName, b.LastName as BillingLastName from Sale s " +
                    "inner join BillingSale bsale on bsale.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = bsale.ChargeHistoryID " +
                    "inner join ChargeDetails cd on cd.ChargeHistoryID = ch.ChargeHistoryID " +
                    "inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "inner join Billing b on b.BillingID = bs.BillingID " +
                    "inner join RefererBilling rb on rb.BillingID = b.BillingID " +
                    "inner join ProductCode pc on pc.ProductCode = cd.SKU " +
                    "inner join ProductCodeInventory pci on pci.ProductCodeID = pc.ProductCodeID " +
                    "inner join Inventory i on i.InventoryID = pci.InventoryID " +
                    "where rb.RefererID = @refererID " +
                    "union " +
                    "select i.InventoryID, i.Product as InventoryProduct, (ifnull(pci.Quantity, 1) * ifnull(u.Quantity, 1)) as InventoryQuantity, s.SaleID, s.CreateDT as SaleCreateDT, ch.Amount as SaleChargeAmount, null as SaleShippedDT, s.TrackingNumber as SaleTrackingNumber, null as SaleReturnDT, b.BillingID, b.FirstName as BillingFirstName, b.LastName as BillingLastName from Sale s " +
                    "inner join UpsellSale usale on usale.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = usale.ChargeHistoryID " +
                    "inner join Upsell u on u.UpsellID = usale.UpsellID " +
                    "inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "inner join Billing b on b.BillingID = bs.BillingID " +
                    "inner join RefererBilling rb on rb.BillingID = b.BillingID " +
                    "inner join ProductCode pc on pc.ProductCode = u.ProductCode " +
                    "inner join ProductCodeInventory pci on pci.ProductCodeID = pc.ProductCodeID " +
                    "inner join Inventory i on i.InventoryID = pci.InventoryID " +
                    "where rb.RefererID = @refererID " +
                    "union " +
                    "select i.InventoryID, i.Product as InventoryProduct, (ifnull(pci.Quantity, 1) * ifnull(e.Quantity, 1)) as InventoryQuantity, s.SaleID, s.CreateDT as SaleCreateDT, 0.00 as SaleChargeAmount, null as SaleShippedDT, s.TrackingNumber as SaleTrackingNumber, null as SaleReturnDT, b.BillingID, b.FirstName as BillingFirstName, b.LastName as BillingLastName from Sale s " +
                    "inner join ExtraTrialShipSale esale on esale.SaleID = s.SaleID " +
                    "inner join ExtraTrialShip e on e.ExtraTrialShipID = esale.ExtraTrialShipID " +
                    "inner join Billing b on b.BillingID = e.BillingID " +
                    "inner join RefererBilling rb on rb.BillingID = b.BillingID " +
                    "inner join ProductCode pc on pc.ProductCode = e.ProductCode " +
                    "inner join ProductCodeInventory pci on pci.ProductCodeID = pc.ProductCodeID " +
                    "inner join Inventory i on i.InventoryID = pci.InventoryID " +
                    "where rb.RefererID = @refererID");
                q.Parameters.Add("@refererID", MySqlDbType.Int32).Value = refererID;

                IList<InventorySaleView> tempRes = dao.Load<InventorySaleView>(q);

                res = GetSaleListByInventoryList(tempRes);
                res = res.OrderByDescending(i => i.CreateDT).ToList();
                foreach (SaleView i in res)
                {
                    i.ShippedDT = shipperService.GetShippedDate(i.SaleID.Value);
                    i.ReturnDT = shipperService.GetReturnDate(i.SaleID.Value);
                    i.TrackingNumber = shipperService.ExtractTrackingNumber(i.TrackingNumber);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        private IList<SaleView> GetSaleListByInventoryList(IEnumerable<InventorySaleView> inventoryList)
        {
            IList<SaleView> res = null;
            try
            {
                res = new List<SaleView>();

                foreach (var sale in inventoryList.GroupBy(i => i.SaleID))
                {
                    res.Add(new SaleView()
                    {
                        SaleID = sale.First().SaleID,
                        CreateDT = sale.First().SaleCreateDT,
                        ChargeAmount = sale.First().SaleChargeAmount,
                        ShippedDT = sale.First().SaleShippedDT,
                        TrackingNumber = sale.First().SaleTrackingNumber,
                        ReturnDT = sale.First().SaleReturnDT,
                        Billing = new Billing()
                        {
                            BillingID = sale.First().BillingID,
                            FirstName = sale.First().BillingFirstName,
                            LastName = sale.First().BillingLastName,
                        },
                        InventoryList = sale.Select(i => new InventoryView()
                        {
                            InventoryID = i.InventoryID,
                            Product = i.InventoryProduct,
                            Quantity = i.InventoryQuantity
                        }).ToList()
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<SaleView> GetOwnSalesExcludeReturns(int refererID)
        {
            IList<SaleView> res = null;
            try
            {
                res = GetOwnSales(refererID).Where(i => i.ReturnDT == null).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<SaleView> GetPrimaryReferalsSalesExcludeReturns(int refererID)
        {
            IList<SaleView> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select i.InventoryID, i.Product as InventoryProduct, (ifnull(pci.Quantity, 1) * ifnull(bsale.Quantity, 1)) as InventoryQuantity, s.SaleID, s.CreateDT as SaleCreateDT, ch.Amount as SaleChargeAmount, null as SaleShippedDT, s.TrackingNumber as SaleTrackingNumber, null as SaleReturnDT, b.BillingID, b.FirstName as BillingFirstName, b.LastName as BillingLastName from Sale s " +
                    "inner join SaleReferer sr on sr.SaleID = s.SaleID " +
                    "inner join BillingSale bsale on bsale.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = bsale.ChargeHistoryID " +
                    "inner join ChargeDetails cd on cd.ChargeHistoryID = ch.ChargeHistoryID " +
                    "inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "inner join Billing b on b.BillingID = bs.BillingID " +
                    "inner join ProductCode pc on pc.ProductCode = cd.SKU " +
                    "inner join ProductCodeInventory pci on pci.ProductCodeID = pc.ProductCodeID " +
                    "inner join Inventory i on i.InventoryID = pci.InventoryID " +
                    "where sr.RefererID = @refererID " +
                    "union " +
                    "select i.InventoryID, i.Product as InventoryProduct, (ifnull(pci.Quantity, 1) * ifnull(u.Quantity, 1)) as InventoryQuantity, s.SaleID, s.CreateDT as SaleCreateDT, ch.Amount as SaleChargeAmount, null as SaleShippedDT, s.TrackingNumber as SaleTrackingNumber, null as SaleReturnDT, b.BillingID, b.FirstName as BillingFirstName, b.LastName as BillingLastName from Sale s " +
                    "inner join SaleReferer sr on sr.SaleID = s.SaleID " +
                    "inner join UpsellSale usale on usale.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = usale.ChargeHistoryID " +
                    "inner join Upsell u on u.UpsellID = usale.UpsellID " +
                    "inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "inner join Billing b on b.BillingID = bs.BillingID " +
                    "inner join ProductCode pc on pc.ProductCode = u.ProductCode " +
                    "inner join ProductCodeInventory pci on pci.ProductCodeID = pc.ProductCodeID " +
                    "inner join Inventory i on i.InventoryID = pci.InventoryID " +
                    "where sr.RefererID = @refererID");
                q.Parameters.Add("@refererID", MySqlDbType.Int32).Value = refererID;

                IList<InventorySaleView> tempRes = dao.Load<InventorySaleView>(q);

                res = GetSaleListByInventoryList(tempRes);
                res = res.OrderByDescending(i => i.CreateDT).ToList();
                foreach (SaleView i in res)
                {
                    i.ShippedDT = shipperService.GetShippedDate(i.SaleID.Value);
                    i.ReturnDT = shipperService.GetReturnDate(i.SaleID.Value);
                    i.TrackingNumber = shipperService.ExtractTrackingNumber(i.TrackingNumber);
                }

                res = res.Where(i => i.ReturnDT == null).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<SaleView> GetSecondaryReferalsSalesExcludeReturns(int refererID)
        {
            IList<SaleView> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select i.InventoryID, i.Product as InventoryProduct, (ifnull(pci.Quantity, 1) * ifnull(bsale.Quantity, 1)) as InventoryQuantity, s.SaleID, s.CreateDT as SaleCreateDT, ch.Amount as SaleChargeAmount, null as SaleShippedDT, s.TrackingNumber as SaleTrackingNumber, null as SaleReturnDT, b.BillingID, b.FirstName as BillingFirstName, b.LastName as BillingLastName from Sale s " +
                    "inner join SaleReferer sr on sr.SaleID = s.SaleID " +
                    "inner join Referer r on r.RefererID = sr.RefererID " +
                    "inner join BillingSale bsale on bsale.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = bsale.ChargeHistoryID " +
                    "inner join ChargeDetails cd on cd.ChargeHistoryID = ch.ChargeHistoryID " +
                    "inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "inner join Billing b on b.BillingID = bs.BillingID " +
                    "inner join ProductCode pc on pc.ProductCode = cd.SKU " +
                    "inner join ProductCodeInventory pci on pci.ProductCodeID = pc.ProductCodeID " +
                    "inner join Inventory i on i.InventoryID = pci.InventoryID " +
                    "where r.ParentRefererID = @refererID " +
                    "union " +
                    "select i.InventoryID, i.Product as InventoryProduct, (ifnull(pci.Quantity, 1) * ifnull(u.Quantity, 1)) as InventoryQuantity, s.SaleID, s.CreateDT as SaleCreateDT, ch.Amount as SaleChargeAmount, null as SaleShippedDT, s.TrackingNumber as SaleTrackingNumber, null as SaleReturnDT, b.BillingID, b.FirstName as BillingFirstName, b.LastName as BillingLastName from Sale s " +
                    "inner join SaleReferer sr on sr.SaleID = s.SaleID " +
                    "inner join Referer r on r.RefererID = sr.RefererID " +
                    "inner join UpsellSale usale on usale.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = usale.ChargeHistoryID " +
                    "inner join Upsell u on u.UpsellID = usale.UpsellID " +
                    "inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "inner join Billing b on b.BillingID = bs.BillingID " +
                    "inner join ProductCode pc on pc.ProductCode = u.ProductCode " +
                    "inner join ProductCodeInventory pci on pci.ProductCodeID = pc.ProductCodeID " +
                    "inner join Inventory i on i.InventoryID = pci.InventoryID " +
                    "where r.ParentRefererID = @refererID");
                q.Parameters.Add("@refererID", MySqlDbType.Int32).Value = refererID;

                IList<InventorySaleView> tempRes = dao.Load<InventorySaleView>(q);

                res = GetSaleListByInventoryList(tempRes);
                res = res.OrderByDescending(i => i.CreateDT).ToList();
                foreach (SaleView i in res)
                {
                    i.ShippedDT = shipperService.GetShippedDate(i.SaleID.Value);
                    i.ReturnDT = shipperService.GetReturnDate(i.SaleID.Value);
                    i.TrackingNumber = shipperService.ExtractTrackingNumber(i.TrackingNumber);
                }

                res = res.Where(i => i.ReturnDT == null).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<RefererView> GetPrimaryReferalsSalesExcludeReturns(DateTime startDate, DateTime endDate)
        {
            IList<RefererView> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select r.*, count(s.SaleID) as SalesCount, sum(ch.Amount) as SalesAmount from Sale s " +
                    "inner join SaleReferer sr on sr.SaleID = s.SaleID " +
                    "inner join Referer r on r.RefererID = sr.RefererID " +
                    "inner join BillingSale bsale on bsale.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = bsale.ChargeHistoryID " +
                    "left join ReturnedSale rets on rets.SaleID = s.SaleID " +
                    "where s.CreateDT between @startDate and @endDate " +
                    "and rets.SaleID is null " +
                    "group by r.RefererID " +
                    "union all " +
                    "select r.*, count(s.SaleID) as SalesCount, sum(ch.Amount) as SalesAmount from Sale s " +
                    "inner join SaleReferer sr on sr.SaleID = s.SaleID " +
                    "inner join Referer r on r.RefererID = sr.RefererID " +
                    "inner join UpsellSale usale on usale.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = usale.ChargeHistoryID " +
                    "left join ReturnedSale rets on rets.SaleID = s.SaleID " +
                    "where s.CreateDT between @startDate and @endDate " +
                    "and rets.SaleID is null " +
                    "group by r.RefererID ");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate;

                res = dao.Load<RefererView>(q);

                res = res.GroupBy(i => i.Referer.RefererID).
                    Select(i => new RefererView()
                    {
                        Referer = i.First().Referer,
                        SalesCount = i.Sum(r => r.SalesCount),
                        SalesAmount = i.Sum(r => r.SalesAmount)
                    }).
                    ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<RefererView> GetSecondaryReferalsSalesExcludeReturns(DateTime startDate, DateTime endDate)
        {
            IList<RefererView> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select r.*, count(s.SaleID) as SalesCount, sum(ch.Amount) as SalesAmount from Sale s " +
                    "inner join SaleReferer sr on sr.SaleID = s.SaleID " +
                    "inner join Referer childr on childr.RefererID = sr.RefererID " +
                    "inner join Referer r on r.RefererID = childr.ParentRefererID " +
                    "inner join BillingSale bsale on bsale.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = bsale.ChargeHistoryID " +
                    "left join ReturnedSale rets on rets.SaleID = s.SaleID " +
                    "where s.CreateDT between @startDate and @endDate " +
                    "and rets.SaleID is null " +
                    "group by r.RefererID " +
                    "union all " +
                    "select r.*, count(s.SaleID) as SalesCount, sum(ch.Amount) as SalesAmount from Sale s " +
                    "inner join SaleReferer sr on sr.SaleID = s.SaleID " +
                    "inner join Referer childr on childr.RefererID = sr.RefererID " +
                    "inner join Referer r on r.RefererID = childr.ParentRefererID " +
                    "inner join UpsellSale usale on usale.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = usale.ChargeHistoryID " +
                    "left join ReturnedSale rets on rets.SaleID = s.SaleID " +
                    "where s.CreateDT between @startDate and @endDate " +
                    "and rets.SaleID is null " +
                    "group by r.RefererID ");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate;

                res = dao.Load<RefererView>(q);

                res = res.GroupBy(i => i.Referer.RefererID).
                    Select(i => new RefererView()
                    {
                        Referer = i.First().Referer,
                        SalesCount = i.Sum(r => r.SalesCount),
                        SalesAmount = i.Sum(r => r.SalesAmount)
                    }).
                    ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public decimal CalculateRefererPrimaryCommission(decimal amount)
        {
            return amount * REFERER_PRIMARY_SALE_COMMISSION;
        }

        public decimal CalculateRefererSecondaryCommission(decimal amount)
        {
            return amount * REFERER_SECONDARY_SALE_COMMISSION;
        }

        public decimal CalculateRefererPrimaryCommissionInEcigsDollars(decimal amount)
        {
            return ConvertToEcigsDollars(CalculateRefererPrimaryCommission(amount));
        }

        public decimal CalculateRefererSecondaryCommissionInEcigsDollars(decimal amount)
        {
            return ConvertToEcigsDollars(CalculateRefererSecondaryCommission(amount));
        }

        public decimal ConvertToEcigsDollars(decimal amount)
        {
            return amount / ECIGS_DOLLAR_TO_USD_CONVERSION_RATE;
        }

        public decimal ConvertToUSD(decimal amount)
        {
            return amount * ECIGS_DOLLAR_TO_USD_CONVERSION_RATE;
        }

        public DateTime CommissionIncubationPeriodStart
        {
            get
            {
                //Get last month in incubation period
                DateTime res = DateTime.Today.AddDays(1 - REFERER_COMMISSION_INCUBATION_PERIOD);
                //First day of last month in incubation period
                return res.AddDays(1 - res.Day);
            }
        }

        public IList<RefererCommission> GetCommissions(int refererID)
        {
            IList<RefererCommission> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select * from RefererCommission " +
                    "where RefererID = @refererID " +
                    "order by CreateDT desc");
                q.Parameters.Add("@refererID", MySqlDbType.Int32).Value = refererID;
                res = dao.Load<RefererCommission>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public decimal GetAvailableCommission(int refererID)
        {
            decimal res = 0M;
            try
            {
                IList<SaleView> primarySaleList = GetPrimaryReferalsSalesExcludeReturns(refererID);
                IList<SaleView> secondarySaleList = GetSecondaryReferalsSalesExcludeReturns(refererID);

                //All sales before incubation period start
                decimal salesAmount =
                    CalculateRefererPrimaryCommission(
                        primarySaleList.Where(i => i.CreateDT < CommissionIncubationPeriodStart).Sum(i => i.ChargeAmount.Value)) +
                    CalculateRefererSecondaryCommission(
                        secondarySaleList.Where(i => i.CreateDT < CommissionIncubationPeriodStart).Sum(i => i.ChargeAmount.Value));

                //All commited commissions
                decimal commissionsAmount = GetCommissions(refererID).Sum(i => i.Amount.Value);

                res = salesAmount - commissionsAmount;
                res = Math.Round(res, 2);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = 0M;
            }
            return res;
        }

        public BusinessError<RefererCommission> CommitAvailableCommission(int refererID, int refererCommissionTypeID)
        {
            BusinessError<RefererCommission> res = new BusinessError<RefererCommission>(null, BusinessErrorState.Error, null);
            try
            {
                dao.BeginTransaction();

                decimal amount = GetAvailableCommission(refererID);
                if (amount <= 0M || (refererCommissionTypeID == RefererCommissionTypeEnum.ConvertToCash && amount < REFERER_COMMISSION_MIN_FOR_CASH))
                {
                    res.ErrorMessage = "Insufficient amount.";
                }
                else
                {
                    RefererCommission comm = new RefererCommission();
                    comm.RefererID = refererID;
                    comm.Amount = amount;
                    comm.RemainingAmount = amount;
                    comm.RefererCommissionTID = refererCommissionTypeID;
                    //Autocomplete for Use In Store
                    comm.Completed = (refererCommissionTypeID == RefererCommissionTypeEnum.UseInStore);
                    comm.CreateDT = DateTime.Now;
                    dao.Save<RefererCommission>(comm);

                    res.State = BusinessErrorState.Success;
                    res.ReturnValue = comm;
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();

                res.State = BusinessErrorState.Error;
                res.ReturnValue = null;
                res.ErrorMessage = "Unknown error occured.";
            }
            return res;
        }

        public decimal GetAvailableAmountToRedeem(int refererID)
        {
            decimal res = 0M;

            IList<RefererCommission> comms = GetCommissions(refererID);
            if (comms != null)
            {
                comms = comms.Where(c => c.RefererCommissionTID == TrimFuel.Model.Enums.RefererCommissionTypeEnum.UseInStore && c.RemainingAmount != null).ToList();
                if (comms.Count > 0)
                {
                    res = comms.Sum(c => c.RemainingAmount.Value);
                }
            }

            return res;
        }

        public decimal GetAvailableAmountToRedeemInEcigsDollars(int refereID)
        {
            return ConvertToEcigsDollars(GetAvailableAmountToRedeem(refereID));
        }

        public bool CreateOrderReferer(Order order, int? refererID, long billingID)
        {
            bool res = false;
            try
            {
                dao.BeginTransaction();

                var ownReferer = GetOwnRefererByBilling(billingID);
                if (ownReferer != null)
                {
                    if (ownReferer.ParentRefererID == null)
                    {
                        ownReferer.ParentRefererID = refererID;
                        dao.Save<Referer>(ownReferer);
                    }
                }
                order.RefererID = refererID;
                dao.Save<Order>(order);
                res = true;

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
                res = false;
            }
            return res;
        }
    }
}
