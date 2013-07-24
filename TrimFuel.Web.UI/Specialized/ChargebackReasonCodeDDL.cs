using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;

namespace TrimFuel.Web.UI.Specialized
{
    public class ChargebackReasonCodeDDL : DropDownList
    {
        protected override void OnInit(EventArgs e)
        {
            PaymentTypeID = TrimFuel.Model.Enums.PaymentTypeEnum.Visa;

            base.OnInit(e);
        }

        DashboardService service = new DashboardService();

        public int PaymentTypeID { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            string selectedValue = SelectedValue;
            Items.Clear();

            Items.Add(new ListItem("-- Select --", string.Empty));
            foreach (ChargebackReasonCode i in service.GetChargebackReasonCodeList(PaymentTypeID))
            {
                Items.Add(new ListItem(i.ReasonCode.ToString() + " " + i.Description, i.ChargebackReasonCodeID.ToString()));
            }

            try
            {
                SelectedValue = selectedValue;
            }
            catch { }            

            base.OnDataBinding(e);
        }
    }
}
