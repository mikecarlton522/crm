using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kaboom.Store1.Logic
{
    public enum ShoppingCartProductType
    {
        Subscription = 1,
        Upsell = 2
    }

    public class ShoppingCartProduct
    {
        public ShoppingCartProductType ProductType { get; set; }
        public int ProductID { get; set; }
        public decimal Price { get; set; }

        //public override string ToString()
        //{
        //    string prefix = string.Empty;
        //    switch (ProductType)
        //    {
        //        case ShoppingCartProductType.Subscription:
        //            prefix = "s";
        //            break;
        //        case ShoppingCartProductType.Upsell:
        //            prefix = "u";
        //            break;
        //        default:
        //            throw new Exception("ProductType is undefined");
        //            break;
        //    }
        //    return string.Format("{0}_{1}", prefix, ProductType);
        //}

        //public static ShoppingCartProduct Create(string productString)
        //{
        //    string[] productDesc = productString.Split('_');
        //    if (productDesc.Length != 2)
        //    {
        //        throw new ArgumentException("Parameter productString is not well formatted");
        //    }
        //    ShoppingCartProductType productType = ShoppingCartProductType.Subscription;
        //    switch (productDesc[0])
        //    {
        //        case "s":
        //            productType = ShoppingCartProductType.Subscription;
        //            break;
        //        case "u":
        //            productType = ShoppingCartProductType.Upsell;
        //            break;
        //        default:
        //            throw new ArgumentException(string.Format("Can not determine ProductType({0})", productDesc[0]));
        //            break;
        //    }
        //    int productID = 0;
        //    if (!int.TryParse(productDesc[1], out productID))
        //    {
        //        throw new ArgumentException(string.Format("Can not determine ProductID({0})", productDesc[1]));
        //    }
        //    return new ShoppingCartProduct() { ProductType = productType, ProductID = productID };
        //}
    }
}
