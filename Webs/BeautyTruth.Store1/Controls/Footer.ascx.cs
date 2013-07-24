using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using BeautyTruth.Store1.Logic;

namespace BeautyTruth.Store1.Controls
{
    public partial class Footer : System.Web.UI.UserControl
    {
        public bool WithSignIn { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            phSignIn.Visible = WithSignIn;
            phTextColumns.Visible = WithSignIn;
            txtEmailAddress.Text = "Enter your email for exclusive member-only deals...";
        }

        protected void lbAddEmail_Click(object sender, EventArgs e)
        {
            string email = txtEmailAddress.Text;
            if (new WebStoreService().AddNewEmail(email, GeneralInfo.CampaignID))
            {
                //success
                lblResult.Text = "Your email successfully added.";
            }
            else
            {
                //error
                lblResult.Text = "This email already exists.";
            }
            DataBind();
        }
    }
}