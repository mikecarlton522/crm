using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fitdiet.Store1.Logic;
using FitDiet.Store1.Controls;

namespace FitDiet.Store1.Logic
{
    public class PageCartExt : PageX
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            if (!Request.IsSecureConnection)
            {
                string redirectUrl = Request.Url.ToString().Replace("http:", "https:");
                Response.Redirect(redirectUrl);
            }
        }

        public enum CartState
        {
            Cart = 1,
            ShippingDetails = 2,
            BillingDetails = 3,
            PlaceMyOrder = 4
        }

        public CartState CurrentState
        {
            get
            {
                if (ViewState["CurrentState"] == null)
                {
                    ViewState["CurrentState"] = CartState.Cart;
                }
                return (CartState)ViewState["CurrentState"];
            }
            set { ViewState["CurrentState"] = value; }
        }
    }
}
