using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Enums;
using TrimFuel.Model.Views;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Flow
{
    public class OrderBuilder : BaseService
    {
        public class Product
        {
            public ProductSKU ProductSKU { get; set; }
            public int Quantity { get; set; }
        }

        public struct Plan
        {
            public int RecurringPlanID { get; set; }
            public int TrialInterim { get; set; }
        }

        public OrderView Order { get; set; }

        public OrderBuilder LoadByID(long? orderID)
        {
            Order = new OrderService().LoadOrder(orderID);
            return this;
        }

        public OrderBuilder Create(long? billingID, int? campaignID, string orderAuthor, string ip, string url,
            string affiliate, string subAffiliate, int? productID)
        {
            Order = new OrderView()
            {
                Billing = EnsureLoad<Billing>(billingID),
                Order = new Order() { 
                    BillingID = billingID,
                    CampaignID = campaignID,
                    OrderAuthor = orderAuthor,
                    IP = ip,
                    URL = url,
                    Affiliate = affiliate,
                    SubAffiliate = subAffiliate,
                    ProductID = productID,
                    Scrub = false,
                    OrderStatus = OrderStatusEnum.New
                },
                SaleList = new List<OrderSaleView>()
            };
            return this;
        }

        /// <summary>
        /// Intended for back compatibility to support UpsellTypes and ExtraTrialShipTypes
        /// Gets Inventory List and replaces SKUs by ProductSKU
        /// If appropriate ProductSKU is not found throws exception
        /// </summary>
        /// <param name="productCode"></param>
        /// <returns></returns>
        public IList<Product> GetProductSKU_By_ProductCode(string productCode)
        {
            IList<Product> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select i.InventoryID, sku.ProductSKU as SKU, sku.ProductName as Product, pci.Quantity from Inventory i
                    inner join ProductCodeInventory pci on pci.InventoryID = i.InventoryID
                    inner join ProductCode pc on pc.ProductCodeID = pci.ProductCodeID
                    inner join ProductSKU sku on i.SKU = sku.ProductSKU
                    where pc.ProductCode = @productCode
                    group by i.InventoryID
                ");
                q.Parameters.Add("@productCode", MySqlDbType.VarChar).Value = productCode;

                IList<InventoryView> res2 = dao.Load<InventoryView>(q);
                res = new List<Product>();
                foreach (var item in res2)
	            {
                    if (item.SKU == null)
                    {
                        throw new Exception(string.Format("Can't find ProductSKU for Inventory({0})", item.InventoryID));
                    }
                    else
                    {
                        res.Add(new Product(){
                            Quantity = item.Quantity.Value,
                            ProductSKU = new ProductSKU(){
                                ProductSKU_ = item.SKU,
                                ProductName = item.Product
                            }
                        });
                    }
	            }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                res = null;
            }
            return res;
        }

        public OrderBuilder AppendExtraTrialShipType(int extraTrialShipTypeID, int quantity)
        {
            try
            {
                ExtraTrialShipType type = EnsureLoad<ExtraTrialShipType>(extraTrialShipTypeID);
                IList<Product> productList = GetProductSKU_By_ProductCode(type.ProductCode);
                if (productList != null)
                {
                    AppendSale(null, 0M, quantity, productList, null);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return this;
        }

        public OrderBuilder AppendUpsellType(int upsellTypeID, int quantity)
        {
            try
            {
                UpsellType type = EnsureLoad<UpsellType>(upsellTypeID);
                IList<Product> productList = GetProductSKU_By_ProductCode(type.ProductCode);
                if (productList != null)
                {
                    AppendSale(null, type.Price.Value, quantity, productList, null);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return this;
        }

        public OrderBuilder AppendSale(string name, decimal price, int quantity, IEnumerable<Product> productList, Plan? recurringPlan)
        {
            OrderSaleView saleView = new OrderSaleView()
            {
                Order = Order,
                Invoice = null,
                OrderSale = new OrderSale()
                {
                    InvoiceID = null,
                    OrderID = Order.Order.OrderID,
                    SaleType = (recurringPlan != null ? OrderSaleTypeEnum.Trial : OrderSaleTypeEnum.Upsell),
                    Quantity = quantity,
                    PurePrice = price,
                    SaleID = null,
                    SaleName = name,
                    SaleStatus = SaleStatusEnum.New
                }
            };

            IList<OrderRecurringPlan> planList = new List<OrderRecurringPlan>();
            if (recurringPlan != null)
            {
                planList.Add(new OrderRecurringPlan()
                {
                    SaleID = null,
                    RecurringStatus = RecurringStatusEnum.New,
                    RecurringPlanID = recurringPlan.Value.RecurringPlanID,
                    TrialInterim = recurringPlan.Value.TrialInterim,
                    OrderRecurringPlanID = null,
                    NextCycleDT = null                    
                });
            }

            IList<OrderProductView> orderProductList = new List<OrderProductView>();
            if (productList != null && productList.Count() > 0)
            {
                foreach (Product item in productList)
                {
                    orderProductList.Add(new OrderProductView()
                    {
                        ProductSKU = item.ProductSKU,
                        OrderProduct = new OrderProduct() { 
                            OrderProductID = null,
                            SaleID = null,
                            ProductSKU = item.ProductSKU.ProductSKU_,
                            Quantity = item.Quantity
                        },
                        Sale = saleView
                    });
                }
            }

            saleView.PlanList = planList;
            saleView.ProductList = orderProductList;

            Order.SaleList.Add(saleView);
            return this;
        }

        public OrderBuilder AppendProductCode(string productCode, int quantity, decimal price)
        {
            try
            {
                List<Product> productList = GetProductSKU_By_ProductCode(productCode).ToList();

                for (int i = 0; i < productList.Count; i++ )
                    productList[i].Quantity = productList[i].Quantity * quantity;

                if (productList != null)
                {
                    AppendSale(null, price, 1, productList, null);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return this;
        }

        public OrderBuilder AppendProductCode(int productCodeID, int quantity, decimal price)
        {
            try
            {
                var prCode = dao.Load<ProductCode>(productCodeID);
                if (prCode != null)
                {
                    IList<Product> productList = GetProductSKU_By_ProductCode(prCode.ProductCode_);

                    for (int i = 0; i < productList.Count; i++)
                        productList[i].Quantity = productList[i].Quantity * quantity;

                    if (productList != null)
                    {
                        AppendSale(null, price, 1, productList, null);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return this;
        }

        public virtual BusinessError<IList<string>> Validate()
        {
            BusinessError<IList<string>> res = new BusinessError<IList<string>>(null, BusinessErrorState.Success, "");
            //res.ErrorMessage = "One or more products are invalid"
            //res.ReturnValue = new List<string>();
            //res.ReturnValue.Add("... has invalid quantity");
            //...
            return res;
        }

        public BusinessError<OrderView> Save()
        {
            BusinessError<OrderView> res = new BusinessError<OrderView>(Order, BusinessErrorState.Success, string.Empty);

            BusinessError<IList<string>> isValid = Validate();
            if (isValid.State == BusinessErrorState.Error)
            {
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = isValid.ErrorMessage + string.Join("\n", isValid.ReturnValue.ToArray());
            }
            else
            {
                try
                {
                    dao.BeginTransaction();

                    if (Order.Order.OrderID == null)
                    {
                        Order.Order.CreateDT = DateTime.Now;
                        dao.Save(Order.Order);
                    }
                    foreach (OrderSaleView sale in Order.SaleList)
                    {
                        if (sale.OrderSale.SaleID == null)
                        {
                            sale.OrderSale.OrderID = Order.Order.OrderID;
                            sale.OrderSale.CreateDT = DateTime.Now;
                            dao.Save(sale.OrderSale);

                            foreach (OrderProductView product in sale.ProductList)
                            {
                                product.OrderProduct.SaleID = sale.OrderSale.SaleID;
                                dao.Save(product.OrderProduct);
                            }

                            foreach (OrderRecurringPlan plan in sale.PlanList)
                            {
                                plan.SaleID = sale.OrderSale.SaleID;
                                dao.Save(plan);
                            }
                        }
                    }

                    dao.CommitTransaction();
                }
                catch (Exception ex)
                {
                    dao.RollbackTransaction();
                    logger.Error(ex);
                    res.ErrorMessage = "Unknown error occurred.";
                    res.State = BusinessErrorState.Error;
                    res.ReturnValue = null;
                    Order = null;
                }
            }

            return res;
        }
    }
}
