using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Enums;
using TrimFuel.Model.Views;
using TrimFuel.Business.Gateways;
using TrimFuel.Business.Flow;

namespace TrimFuel.Business
{
    public class OrderShipmentService : BaseService
    {
        public IList<ShippingEventView> GetShippingHistory(long saleID)
        {
            IList<ShippingEventView> res = null;

            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select distinct sr.CreateDT, sr.ResponseShipmentStatus as ResultShipmentStatus, sr.ShipperID as ShipperID, sr.Response as EventText from ShipperRequest sr
                    inner join ShipmentShipperRequest ssr on ssr.ShipperRequestID = sr.ShipperRequestID
                    inner join Shipment sh on sh.ShipmentID = ssr.ShipmentID
                    where sh.SaleID = @saleID
                    union all
                    select distinct sn.CreateDT, sn.NoteShipmentStatus as ResultShipmentStatus, null as ShipperID, sn.Note as EventText from ShippingNote sn
                    inner join ShipmentShippingNote ssn on ssn.ShippingNoteID = sn.ShippingNoteID
                    inner join Shipment sh on sh.ShipmentID = ssn.ShipmentID
                    where sh.SaleID = @saleID
                ");
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;

                res = dao.Load<ShippingEventView>(q).OrderBy(i => i.CreateDT).ToList();

                q = new MySqlCommand(@"
                    select * from Shipper
                ");
                IList<Shipper> res2 = dao.Load<Shipper>(q);

                foreach (var item in res)
                {
                    item.Shipper = res2.FirstOrDefault(j => j.ShipperID == item.ShipperID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                res = null;
            }

            return res;
        }

        private IShipmentGateway GetGateway(Shipper shipper)
        {
            if (shipper == null)
            {
                throw new Exception("Can't determine Gateway. MID is null.");
            }
            if (shipper.ShipperID == ShipperEnum.TSN)
            {
                return new Gateways.TSN.TSNGateway();
            }
            else if (shipper.ShipperID == ShipperEnum.ABF)
            {
                return new Gateways.ABF.ABFGateway();
            }
            else if (shipper.ShipperID == ShipperEnum.AtLastFulfillment)
            {
                return new Gateways.AtLastFulfillment.AtLastFulfillmentGateway2();
            }
            else if (shipper.ShipperID == ShipperEnum.GoFulfillment)
            {
                return new Gateways.GoFulfillmentGateway.GoFulfillmentGateway2();
            }
            else if (shipper.ShipperID == ShipperEnum.NPF)
            {
                return new Gateways.NPFGateway.NPFGateway2();
            }
            else if (shipper.ShipperID == ShipperEnum.MB)
            {
                return new Gateways.MoldingBox.MoldingBoxGateway2();
            }
            else if (shipper.ShipperID == ShipperEnum.TF)
            {
                return new Gateways.ABF.TriangleFulfillmentGateway();
            }
            throw new Exception(string.Format("Can't determine Gateway for Shipment({0},{1}).", shipper.ShipperID, shipper.Name));
        }

        public Shipper GetShipperByProductID(int productID)
        {
            Shipper res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select sh.* from Shipper sh
                    inner join ShipperProduct shp on shp.ShipperID = sh.ShipperID
                    where shp.ProductID = @productID
                ");
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                res = dao.Load<Shipper>(q).SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        public void SubmitPendingShipments()
        {
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select distinct p.* from Shipment sh
                    inner join OrderSale sl on sl.SaleID = sh.SaleID
                    inner join Orders o on o.OrderID = sl.OrderID
                    inner join Product p on p.ProductID = o.ProductID
                    where sh.ShipmentStatus >= @new and sh.ShipmentStatus < @submitted
                ");
                q.Parameters.Add("@new", MySqlDbType.Int32).Value = ShipmentStatusEnum.New;
                q.Parameters.Add("@submitted", MySqlDbType.Int32).Value = ShipmentStatusEnum.Submitted;
                IList<Product> productList = dao.Load<Product>(q);
                foreach (var product in productList)
                {
                    SubmitPendingShipments(product);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void SubmitPendingShipments(Product product)
        {
            try
            {
                Shipper shipper = GetShipperByProductID(product.ProductID.Value);
                if (shipper == null)
                {
                    throw new Exception(string.Format("Can't determine Shipper for Product({0},{1})", product.ProductID, product.ProductName));
                }
                IList<ShipmentPackageView> packageList = GetPendingShipments(product.ProductID.Value);

                IList<ShipmentPackageView> errorPackageList = (
                    from pck in packageList
                    from sl in pck.SaleList
                    from sh in sl.ShipmentList
                    where string.IsNullOrEmpty(sh.InventorySKU) || string.IsNullOrEmpty(sh.InventoryName)
                    select pck).Distinct().ToList();

                packageList = packageList.Except(errorPackageList).ToList();
                IList<Shipment> packageShipmentList = packageList.SelectMany(i => i.GetShipmentList()).Select(i => i.Shipment).ToList();

                IList<string> errorSKUList = (
                    from pck in errorPackageList
                    from sl in pck.SaleList
                    from sh in sl.ShipmentList
                    where string.IsNullOrEmpty(sh.InventorySKU) || string.IsNullOrEmpty(sh.InventoryName)
                    select sh.Shipment.ProductSKU).Distinct().ToList();

                IList<KeyValuePair<string, IList<Shipment>>> errorShipmentList = (
                    from sku in errorSKUList
                    join sh in (
                        from pck in errorPackageList
                        from sl in pck.SaleList
                        from sh in sl.ShipmentList
                        where string.IsNullOrEmpty(sh.InventorySKU) || string.IsNullOrEmpty(sh.InventoryName)
                        select sh.Shipment) on sku equals sh.ProductSKU into shGrouping
                    select new KeyValuePair<string, IList<Shipment>>(
                        sku,
                        shGrouping.ToList())
                    ).ToList();

                //Log errors into log file
                if (errorSKUList.Count > 0)
                {
                    try
                    {
                        throw new Exception(string.Format("Can't find InventorySKU for ProductSKU({0}) for Shipper({1})", string.Join(",", errorSKUList.ToArray()), shipper.Name));
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }

                IShipmentGateway gateway = GetGateway(shipper);
                IDictionary<ShipperConfig.ID, ShipperConfig> config = new ShipperConfigService().GetShipperConfig(shipper.ShipperID);
                new ShipperConfigService().CheckShipperConfig(shipper, config);
                using (JobProcess job = new JobProcess("Shipments Submit ProductID = " + product.ProductID.Value.ToString()))
                {
                    bool canContinue = true;
                    string currentState = string.Empty;
                    decimal currentPercent = 0.00M;
                    StringBuilder processState = new StringBuilder();
                    var shService = new ShipmentFlow();

                    currentState = string.Format("Updating shipments with invalid SKUs: 0 from {0} packages processed", errorPackageList.Count());
                    canContinue = job.CheckAvailabilityAndUpdateProgress(currentPercent, processState.ToString() + currentState);
                    if (canContinue)
                    {                        
                        foreach (var item in errorShipmentList)
                        {
                            shService.SubmitErrorShipments(item.Value, string.Format("Can't find InventorySKU for ProductSKU({0})", item.Key));
                        }
                        currentState = string.Format("Updating shipments with invalid SKUs: {0} from {0} packages processed", errorPackageList.Count());
                        currentPercent = 5M; // 5% for invalid SKUs processing
                    }

                    processState.AppendLine(currentState);
                    currentState = string.Format("Submitting valid shipments: 0 from {0} packages processed", packageList.Count());
                    canContinue = job.CheckAvailabilityAndUpdateProgress(currentPercent, processState.ToString() + currentState);
                    if (canContinue)
                    {
                        IList<ShipmentGatewayResult<ShipmentPackageSubmitResult>> requestList = 
                            gateway.SubmitShipments(packageList, config, Config.Current.SHIPPING_TEST_MODE, delegate(decimal progress, int processed) {
                                //total 90% for submitting
                                currentPercent = 5M + 90M * progress;
                                currentState = string.Format("Submitting valid shipments: {0} from {1} packages processed", processed, packageList.Count());
                                canContinue = job.CheckAvailabilityAndUpdateProgress(currentPercent, processState.ToString() + currentState);
                                return canContinue;
                            });

                        int submittedCount = requestList.SelectMany(i => i.PackageList).Count();
                        processState.AppendLine(currentState);
                        currentState = string.Format("Updating submitted shipments: 0 from {0} packages processed", submittedCount);
                        canContinue = job.CheckAvailabilityAndUpdateProgress(currentPercent, processState.ToString() + currentState);

                        //Update submitted shipments independent on if Job can continue
                        int acceptedByShipper = 0;

                        //TODO: move whole cycle to ShipmentFlow
                        foreach (var request in requestList)
                        {
                            try 
	                        {
                                acceptedByShipper += request.PackageList.Where(i => !string.IsNullOrEmpty(i.ShipperRegID)).Count();

                                ShipperRequest shipperRequest = new ShipperRequest();
                                shipperRequest.CreateDT = DateTime.Now;
                                shipperRequest.Request = request.Request;
                                shipperRequest.Response = request.Response;
                                shipperRequest.ResponseShipmentStatus = (request.PackageList.Where(i => !string.IsNullOrEmpty(i.ShipperRegID)).Count() > 0 ? ShipmentStatusEnum.Submitted : ShipmentStatusEnum.SubmitError);
                                shipperRequest.ShipperID = (short)shipper.ShipperID;
                                dao.Save(shipperRequest);

                                foreach (var package in request.PackageList)
	                            {
		                            IList<Shipment> shipmentList = (
                                        from shID in package.ShipmentIDList
                                        join sh in packageShipmentList on shID equals sh.ShipmentID.Value
                                        select sh
                                        ).ToList();
                                    foreach (var shipment in shipmentList)
                                    {
                                        ShipmentShipperRequest link = new ShipmentShipperRequest();
                                        link.ShipmentID = shipment.ShipmentID;
                                        link.ShipperRequestID = shipperRequest.ShipperRequestID;
                                        dao.Save(link);
                                    }
                                    shService.SubmitShipments(shipmentList, shipper.ShipperID, package.ShipperRegID);
	                            }
	                        }
	                        catch (Exception ex)
	                        {
                                logger.Error(ex);
	                        }
                        }
                        currentPercent += 5M;
                        currentState = string.Format("Updating submitted shipments: {0} from {0} packages processed", requestList.SelectMany(i => i.PackageList).Count());

                        processState.AppendLine(currentState);
                        processState.AppendLine(string.Format("Total: To be Submitted = {0}, Submitted = {1}, Accepted by Shipper = {2}", 
                            packageList.Count + errorPackageList.Count,
                            submittedCount,
                            acceptedByShipper));
                        processState.AppendLine(string.Format("Total errors: Queued = {0}, Declined by Shipper = {1}", 
                            packageList.Count + errorPackageList.Count - submittedCount,
                            submittedCount - acceptedByShipper));
                        job.CheckAvailabilityAndUpdateProgress(currentPercent, processState.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public IList<ShipmentPackageView> GetPendingShipments(int productID)
        {
            IList<ShipmentPackageView> res = null;
            try
            {
                Product product = EnsureLoad<Product>(productID);
                Shipper shipper = GetShipperByProductID(product.ProductID.Value);

                MySqlCommand q = new MySqlCommand(@"
                    select sh.* from Shipment sh
                    inner join OrderSale sl on sl.SaleID = sh.SaleID
                    inner join Orders o on o.OrderID = sl.OrderID
                    where sh.ShipmentStatus >= @new and sh.ShipmentStatus < @submitted
                    and o.ProductID = @productID
                ");
                q.Parameters.Add("@new", MySqlDbType.Int32).Value = ShipmentStatusEnum.New;
                q.Parameters.Add("@submitted", MySqlDbType.Int32).Value = ShipmentStatusEnum.Submitted;
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                IList<Shipment> shipmentList = dao.Load<Shipment>(q);

                q = new MySqlCommand(@"
                    select distinct sl.*, o.BillingID from Shipment sh
                    inner join OrderSale sl on sl.SaleID = sh.SaleID
                    inner join Orders o on o.OrderID = sl.OrderID
                    where sh.ShipmentStatus >= @new and sh.ShipmentStatus < @submitted
                    and o.ProductID = @productID
                ");
                q.Parameters.Add("@new", MySqlDbType.Int32).Value = ShipmentStatusEnum.New;
                q.Parameters.Add("@submitted", MySqlDbType.Int32).Value = ShipmentStatusEnum.Submitted;
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                IList<OrderSaleBillingView> saleList = dao.Load<OrderSaleBillingView>(q);

                q = new MySqlCommand(@"
                    select distinct shOption.* from Shipment sh
                    inner join OrderSale sl on sl.SaleID = sh.SaleID
                    inner join SaleShippingOption shOption on shOption.SaleID = sl.SaleID
                    inner join Orders o on o.OrderID = sl.OrderID
                    where sh.ShipmentStatus >= @new and sh.ShipmentStatus < @submitted
                    and o.ProductID = @productID
                ");
                q.Parameters.Add("@new", MySqlDbType.Int32).Value = ShipmentStatusEnum.New;
                q.Parameters.Add("@submitted", MySqlDbType.Int32).Value = ShipmentStatusEnum.Submitted;
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                IList<SaleShippingOption> saleShippingOptionList = dao.Load<SaleShippingOption>(q);

                //LIMIT 5 to control script
                q = new MySqlCommand(@"
                    select distinct r.* from Shipment sh
                    inner join OrderSale sl on sl.SaleID = sh.SaleID
                    inner join Orders o on o.OrderID = sl.OrderID
                    inner join Billing b on b.BillingID = o.BillingID
                    inner join Registration r on r.RegistrationID = b.RegistrationID
                    where sh.ShipmentStatus >= @new and sh.ShipmentStatus < @submitted
                    and o.ProductID = @productID
                    limit 1000
                ");
                q.Parameters.Add("@new", MySqlDbType.Int32).Value = ShipmentStatusEnum.New;
                q.Parameters.Add("@submitted", MySqlDbType.Int32).Value = ShipmentStatusEnum.Submitted;
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                IList<Registration> registrationList = dao.Load<Registration>(q);

                q = new MySqlCommand(@"
                    select distinct rInfo.* from Shipment sh
                    inner join OrderSale sl on sl.SaleID = sh.SaleID
                    inner join Orders o on o.OrderID = sl.OrderID
                    inner join Billing b on b.BillingID = o.BillingID
                    inner join Registration r on r.RegistrationID = b.RegistrationID
                    inner join RegistrationInfo rInfo on rInfo.RegistrationID = r.RegistrationID
                    where sh.ShipmentStatus >= @new and sh.ShipmentStatus < @submitted
                    and o.ProductID = @productID
                ");
                q.Parameters.Add("@new", MySqlDbType.Int32).Value = ShipmentStatusEnum.New;
                q.Parameters.Add("@submitted", MySqlDbType.Int32).Value = ShipmentStatusEnum.Submitted;
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                IList<RegistrationInfo> registrationInfoList = dao.Load<RegistrationInfo>(q);

                q = new MySqlCommand(@"
                    select distinct b.* from Shipment sh
                    inner join OrderSale sl on sl.SaleID = sh.SaleID
                    inner join Orders o on o.OrderID = sl.OrderID
                    inner join Billing b on b.BillingID = o.BillingID
                    where sh.ShipmentStatus >= @new and sh.ShipmentStatus < @submitted
                    and o.ProductID = @productID
                ");
                q.Parameters.Add("@new", MySqlDbType.Int32).Value = ShipmentStatusEnum.New;
                q.Parameters.Add("@submitted", MySqlDbType.Int32).Value = ShipmentStatusEnum.Submitted;
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                IList<Billing> billingList = dao.Load<Billing>(q);
                
                IList<Set<ProductSKU, InventorySKU>> inventoryList = new List<Set<ProductSKU, InventorySKU>>();
                if (shipper != null)
                {
                    inventoryList = GetInventoryList(shipper.ShipperID);
                }

                res = (
                    from r in registrationList
                    join rInfo in registrationInfoList on r.RegistrationID.Value equals rInfo.RegistrationID.Value into rInfoGrouping
                    join b in billingList on r.RegistrationID.Value equals b.RegistrationID.Value
                    join sl in
                        (
                            from sl in saleList
                            join shOption in saleShippingOptionList on sl.Sale.SaleID.Value equals shOption.SaleID.Value into shOptionGrouping
                            join sh in
                                (
                                    from sh in shipmentList
                                    join i in inventoryList on sh.ProductSKU equals i.Value1.ProductSKU_ into iGrouping
                                    select new ShipmentShipperView()
                                    {
                                        Shipment = sh,
                                        InventoryName = iGrouping.Select(i => i.Value1.ProductName).FirstOrDefault(),
                                        InventorySKU = iGrouping.Select(i => (i.Value2 != null && !string.IsNullOrEmpty(i.Value2.InventorySKU_) ? i.Value2.InventorySKU_ : i.Value1.ProductSKU_)).FirstOrDefault()
                                    }) on sl.Sale.SaleID.Value equals sh.Shipment.SaleID.Value into shGrouping
                            where shGrouping.Count() > 0
                            select new OrderSaleShipmentView()
                            {
                                BillingID = sl.BillingID,
                                Sale = sl.Sale,
                                ShippingOption = shOptionGrouping.FirstOrDefault(),
                                ShipmentList = shGrouping.ToList()
                            }) on b.BillingID.Value equals sl.BillingID.Value into slGrouping
                    where slGrouping.Count() > 0
                    select new ShipmentPackageView()
                    {
                        Billing = b,
                        Registration = r,
                        RegistrationInfo = rInfoGrouping.FirstOrDefault(),
                        Product = product,
                        SaleList = slGrouping.ToList()
                    }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        private DateTime CHECKSHIPPEDSHIPMENTS_FROMDATE { get { return DateTime.Today.AddMonths(-2); } }
        public void CheckShippedShipments()
        {
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select distinct shp.* from Shipment sh
                    inner join Shipper shp on shp.ShipperID = sh.ShipperID
                    where sh.ShipmentStatus >= @submitted and sh.ShipmentStatus < @shipped
                    and sh.CreateDT >= @fromDate
                ");
                q.Parameters.Add("@submitted", MySqlDbType.Int32).Value = ShipmentStatusEnum.Submitted;
                q.Parameters.Add("@shipped", MySqlDbType.Int32).Value = ShipmentStatusEnum.Shipped;
                q.Parameters.Add("@fromDate", MySqlDbType.Timestamp).Value = CHECKSHIPPEDSHIPMENTS_FROMDATE;

                IList<Shipper> shipperList = dao.Load<Shipper>(q);
                foreach (var shipper in shipperList)
                {
                    CheckShippedShipments(shipper, CHECKSHIPPEDSHIPMENTS_FROMDATE);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void CheckShippedShipments(Shipper shipper, DateTime fromDate)
        {
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select distinct sh.ShipperRegID as Value from Shipment sh
                    where sh.ShipmentStatus >= @submitted and sh.ShipmentStatus < @shipped
                    and sh.CreateDT >= @fromDate and sh.ShipperID = @shipperID and sh.ShipperRegID is not null
                ");
                q.Parameters.Add("@submitted", MySqlDbType.Int32).Value = ShipmentStatusEnum.Submitted;
                q.Parameters.Add("@shipped", MySqlDbType.Int32).Value = ShipmentStatusEnum.Shipped;
                q.Parameters.Add("@fromDate", MySqlDbType.Timestamp).Value = fromDate;
                q.Parameters.Add("@shipperID", MySqlDbType.Int16).Value = shipper.ShipperID;

                IList<string> shipperRegIDList = dao.Load<StringView>(q).Select(i => i.Value).ToList();

                IShipmentGateway gateway = GetGateway(shipper);

                IDictionary<ShipperConfig.ID, ShipperConfig> config = new ShipperConfigService().GetShipperConfig(shipper.ShipperID);
                new ShipperConfigService().CheckShipperConfig(shipper, config);

                var shService = new ShipmentFlow();
                IList<ProductSKU> productSKUList = new SubscriptionNewService().GetProductList();

                if (gateway.IsCheckShippedImplemented)
                {
                    //Implement with JobProcess
                    var res = gateway.CheckShipped(shipperRegIDList, config, Config.Current.SHIPPING_TEST_MODE, null);

                    //TODO: move whole cycle to ShipmentFlow
                    foreach (var request in res)
                    {
                        try
                        {
                            ShipperRequest shipperRequest = new ShipperRequest();
                            shipperRequest.CreateDT = DateTime.Now;
                            shipperRequest.Request = request.Request;
                            shipperRequest.Response = request.Response;
                            shipperRequest.ResponseShipmentStatus = ShipmentStatusEnum.Shipped;
                            shipperRequest.ShipperID = (short)shipper.ShipperID;
                            dao.Save(shipperRequest);

                            foreach (var package in request.PackageList.Where(i => !string.IsNullOrEmpty(i.TrackingNumber)))
                            {
                                if (shipperRegIDList.Contains(package.ShipperRegID))
                                {
                                    bool shipped = new ShipmentFlow().ShipShipments(package.ShipperRegID, shipperRequest.ShipperRequestID.Value,
                                        package.TrackingNumber, package.ShipDT ?? DateTime.Now, productSKUList);
                                    if (shipped)
                                        shipperRegIDList.Remove(package.ShipperRegID);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public IList<Shipment> GetShipmentListByShipperRegID(string shipperRegID)
        {
            IList<Shipment> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select sh.* from Shipment sh
                    where sh.ShipperRegID = @shipperRegID
                ");
                q.Parameters.Add("@shipperRegID", MySqlDbType.VarChar).Value = shipperRegID;
                res = dao.Load<Shipment>(q);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

//        public void CheckReturnedShipments()
//        {
//        }

        public IList<Set<ProductSKU, InventorySKU>> GetInventoryList(int shipperID)
        {
            IList<Set<ProductSKU, InventorySKU>> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select p.* from ProductSKU p
                    inner join InventorySKU i on i.ProductSKU = p.ProductSKU
                    where i.ShipperID = @shipperID
                ");
                q.Parameters.Add("@shipperID", MySqlDbType.Int32).Value = shipperID;
                IList<ProductSKU> productsSKUList = dao.Load<ProductSKU>(q);

                q = new MySqlCommand(@"
                    select i.* from InventorySKU i
                    where i.ShipperID = @shipperID
                ");
                q.Parameters.Add("@shipperID", MySqlDbType.Int32).Value = shipperID;
                IList<InventorySKU> inventorySKUList = dao.Load<InventorySKU>(q);

                res = (
                    from p in productsSKUList
                    join i in inventorySKUList on p.ProductSKU_ equals i.ProductSKU into iGrouping
                    select new Set<ProductSKU, InventorySKU>()
                    {
                        Value1 = p,
                        Value2 = iGrouping.FirstOrDefault()
                    }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }
    }
}
