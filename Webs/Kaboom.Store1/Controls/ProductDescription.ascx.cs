using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Kaboom.Store1.Logic;

namespace Kaboom.Store1.Controls
{
    public partial class ProductDescription : System.Web.UI.UserControl
    {
        public enum DescriptionType
        {
            Promotion = 1,
            Cart = 2
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public ShoppingCartProduct Product { get; set; }

        protected KnownProduct ProductNumber
        {
            get
            {
                return ShoppingCart.GetProductNumber(Product);
            }
        }

        public DescriptionType Type { get; set; }
    }
}