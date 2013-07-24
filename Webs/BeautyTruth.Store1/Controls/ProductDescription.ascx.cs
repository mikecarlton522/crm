using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BeautyTruth.Store1.Logic;
using TrimFuel.Business;
using TrimFuel.Model;

namespace Fitdiet.Store1.Controls
{
    public partial class ProductDescription : System.Web.UI.UserControl
    {
        public string Description 
        {
            get
            {
                if (Product.ProductType == ShoppingCartProductType.Upsell)
                {
                    var pr = new ProductService().GetProductCodeInfo(Product.ProductID);
                    if (pr != null)
                        return pr.Title;
                }
                if (Product.ProductType == ShoppingCartProductType.Subscription)
                {
                    var s = new ProductService().Load<Subscription>(Product.ProductID);
                    if (s != null)
                        return s.DisplayName;
                }
                return "Unknown";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
        
        public ShoppingCartProduct Product { get; set; }
    }
}