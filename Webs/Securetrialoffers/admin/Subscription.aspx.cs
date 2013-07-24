using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;

namespace Securetrialoffers.admin
{
    public partial class Subscription_ : System.Web.UI.Page
    {
        BaseService service = new BaseService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
            phSuccess.Visible = false;
            phError.Visible = false;
        }

        protected void bSave_Click(object sender, EventArgs e)
        {
            Subscription s = Subscription1.Subscription;
            if (service.Save<Subscription>(s))
            {
                lSubscriptionID.Text = s.SubscriptionID.ToString();
                ShowSuccess();
            }
            else
            {
                ShowError();
            }            
        }

        private void ShowSuccess()
        {
            phSuccess.Visible = true;
        }

        private void ShowError()
        {
            phError.Visible = true;
        }

    }
}
