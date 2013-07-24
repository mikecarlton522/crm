using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fitdiet.Store1.Logic;
using FitDiet.Store1.Logic;

namespace FitDiet.Store1.Controls
{
    public partial class CartMenu : System.Web.UI.UserControl
    {
        public event EventHandler<StepChangeEventArgs> StepChanged;
        protected void OnStepChanged(PageCartExt.CartState toState)
        {
            if (StepChanged != null)
            {
                StepChanged(this, new StepChangeEventArgs(toState));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                DataBind();
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            phShoppingCart.Controls.Clear();
            phShippingDet.Controls.Clear();
            phPlaceMyOrder.Controls.Clear();
            phBillingDet.Controls.Clear();

            if (((PageCartExt)Page).CurrentState == PageCartExt.CartState.Cart)
                phShoppingCart.Controls.Add(GetLabel("MY SHOPPING CART"));
            else
                phShoppingCart.Controls.Add(GetLinkButton("MY SHOPPING CART", 1));

            if (((PageCartExt)Page).CurrentState == PageCartExt.CartState.ShippingDetails)
                phShippingDet.Controls.Add(GetLabel("SHIPPING DETAILS")); 
            else
                phShippingDet.Controls.Add(GetLinkButton("SHIPPING DETAILS", 2));

            if (((PageCartExt)Page).CurrentState == PageCartExt.CartState.BillingDetails)
                phBillingDet.Controls.Add(GetLabel("BILLING DETAILS")); 
            else
                phBillingDet.Controls.Add(GetLinkButton("BILLING DETAILS", 3));

            if (((PageCartExt)Page).CurrentState == PageCartExt.CartState.PlaceMyOrder)
                phPlaceMyOrder.Controls.Add(GetLabel("PLACE MY ORDER")); 
            else
                phPlaceMyOrder.Controls.Add(GetLinkButton("PLACE MY ORDER", 4));
        }

        protected void Href_Click(object sender, EventArgs e)
        {
            int stateToGo = Convert.ToInt16(((LinkButton)sender).CommandArgument);
            //if (stateToGo <= (int)((PageCartExt)Page).CurrentState)
            {
                OnStepChanged((PageCartExt.CartState)stateToGo);
            }
        }

        private LinkButton GetLinkButton(string title, int state)
        {
            LinkButton lnk = new LinkButton();
            lnk.CssClass = "numbering_nav";
            lnk.CommandArgument = state.ToString();
            lnk.Click += new EventHandler(Href_Click);
            lnk.Text = title;
            lnk.EnableViewState = false;
            lnk.ID = "lnl_" + state.ToString();

            return lnk;
        }

        private Label GetLabel(string title)
        {
            Label lbl = new Label();
            lbl.CssClass = "numbering_nav";
            lbl.Style.Add("color", "#9C3");
            lbl.Text = title;
            lbl.EnableViewState = false;
            lbl.ID = "lbl_curr";

            return lbl;
        }
    }
}