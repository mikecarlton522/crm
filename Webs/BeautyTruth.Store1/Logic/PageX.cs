using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace BeautyTruth.Store1.Logic
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

        protected string GetAdditionStringByIndex(int index)
        {
            if (index % 3 == 0)
                return "alpha";
            else
                if ((index + 1) % 3 == 0)
                    return "omega";
                else return "";
        }
    }
}
