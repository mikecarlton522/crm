using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Ecigsbrand.Store1.Logic
{
    public class PageX : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                ShoppingCart.Load();
                ShoppingCart.Save();
                DataBind();
            }
        }

        protected string FormatPrice(decimal price)
        {
            return price.ToString("$0.00");
        }

        public ShoppingCart ShoppingCart 
        {
            get 
            {
                return ShoppingCart.Current;
            }
        }
    }
}
