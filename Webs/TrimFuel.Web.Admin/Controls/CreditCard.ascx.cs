using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.Controls
{
    public partial class CreditCard_ : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                ExpMonth = Utility.TryGetInt(ddlMonth.SelectedValue);
                ExpYear = Utility.TryGetInt(ddlYear.SelectedValue);
                CVV = Utility.TryGetStr(tbCreditCardCVV.Text);
                CreditCard = Utility.TryGetStr(tbCreditCardNumber.Text);
            }
        }

        public int? ExpMonth { get; set; }
        public int? ExpYear { get; set; }
        public string CreditCard { get; set; }
        public string CVV { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            try
            {
                TrimFuel.Model.Containers.CreditCard cc = new TrimFuel.Model.Containers.CreditCard(CreditCard);
                tbCreditCardNumber.Text = "************" + cc.DecryptedCreditCardRight4;
            }
            catch {}
            try
            {
                ddlMonth.SelectedValue = (ExpMonth != null ? ExpMonth.ToString() : "1");
            }
            catch { }
            try
            {
                ddlYear.SelectedValue = (ExpYear != null ? ExpYear > 100 ? ExpYear.ToString() : (ExpYear + 2000).ToString() : DateTime.Today.Year.ToString());
            }
            catch { }
            tbCreditCardCVV.Text = CVV;
        }
    }
}