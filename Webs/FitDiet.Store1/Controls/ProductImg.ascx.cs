using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fitdiet.Store1.Logic;

namespace Fitdiet.Store1.Controls
{
    public partial class ProductImg : System.Web.UI.UserControl
    {
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
    }
}