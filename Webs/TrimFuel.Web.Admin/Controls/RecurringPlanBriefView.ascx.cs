using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model.Views;
using TrimFuel.Business.Utils;
using TrimFuel.Business;
using TrimFuel.Model;

namespace TrimFuel.Web.Admin.Controls
{
    public partial class RecurrinPlanBriefView : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public RecurringPlanView RecurringPlan { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            if (RecurringPlan != null)
            {
                rCycles.DataSource = RecurringPlan.CycleList;
            }
        }

        protected string ShowAmount(decimal? amount, int? productID)
        {
            string htmlCurrencySymbol = string.Empty;
            ProductService ps = new ProductService();
            Currency cur = ps.GetProductCurrency(productID.Value);
            if(cur != null)
                htmlCurrencySymbol = ps.GetProductCurrency(productID.Value).HtmlSymbol;
            htmlCurrencySymbol = string.IsNullOrEmpty(htmlCurrencySymbol) ? "$" : htmlCurrencySymbol;
            return Utility.FormatCurrency(amount, htmlCurrencySymbol);
        }
    }
}