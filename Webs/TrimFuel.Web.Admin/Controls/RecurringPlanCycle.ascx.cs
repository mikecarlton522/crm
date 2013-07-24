using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using TrimFuel.Model.Enums;
using TrimFuel.Business;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.Controls
{
    public partial class RecurringPlanCycle_ : UserControl
    {
        public ViewModeEnum ViewMode { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        public bool LastOne { get; set; }
        public RecurringPlanCycleView PlanCycle { get; set; }

        public int? ProposedProductID { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            If1.Condition = (ViewMode == ViewModeEnum.View);
            If2.Condition = (ViewMode == ViewModeEnum.Edit);

            If3.Condition = (PlanCycle.Constraint != null);
        }

        protected string ShowBilling()
        {
            if (PlanCycle.Constraint == null)
                return "None";
            if (PlanCycle.Constraint.ChargeTypeID == ChargeTypeEnum.Charge)
                return "Charge";
            if (PlanCycle.Constraint.ChargeTypeID == ChargeTypeEnum.AuthOnly)
                return "Authorize";
            return "Undefined";
        }

        protected string ShowShipping()
        {
            if (PlanCycle.ShipmentList.Count == 0)
                return "No";
            return "Yes";
        }

        protected string ShowAmount(decimal? amount)
        {
            string htmlCurrencySymbol = string.Empty;
            ProductService ps = new ProductService();
            Currency cur = null;
            if (ProposedProductID != null)
                cur = ps.GetProductCurrency(ProposedProductID.Value);
            if (cur != null)
                htmlCurrencySymbol = ps.GetProductCurrency(ProposedProductID.Value).HtmlSymbol;
            htmlCurrencySymbol = string.IsNullOrEmpty(htmlCurrencySymbol) ? "$" : htmlCurrencySymbol;
            return Utility.FormatCurrency(amount, htmlCurrencySymbol);
        }
    }
}