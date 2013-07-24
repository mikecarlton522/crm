using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace TrimFuel.Business.Controls
{
    public class DDLPaymentType : DropDownList
    {
        [Flags]
        public enum RenderPaymentTypes
        {
            Visa = 0x01,
            MasterCard = 0x02,
            Amex = 0x04,
            Discover = 0x08
        }

        RenderPaymentTypes _paymentTypes = 0;
        public RenderPaymentTypes PaymentTypes 
        { 
            get
            {
                if ((int)_paymentTypes == 0)
                    _paymentTypes = RenderPaymentTypes.Visa | RenderPaymentTypes.MasterCard;
                return _paymentTypes;
            }
            set
            {
                _paymentTypes = value;
            }
        }
        
        private Dictionary<string, string> values = new Dictionary<string, string>()
        {
            {"Amex", "1"},
            {"Visa", "2"},
            {"MasterCard", "3"},
            {"Discover", "4"}
        };

        protected override void OnDataBinding(EventArgs e)
        {
            int selectedIndex = SelectedIndex;
            Items.Clear();

            RenderType(RenderPaymentTypes.Amex);
            RenderType(RenderPaymentTypes.Visa);
            RenderType(RenderPaymentTypes.MasterCard);
            RenderType(RenderPaymentTypes.Discover);

            SelectedIndex = selectedIndex;
            base.OnDataBinding(e);
        }

        private void RenderType(RenderPaymentTypes renderType)
        {
            if ((PaymentTypes & renderType) > 0)
                Items.Add(new ListItem(renderType.ToString(), values[renderType.ToString()]));
        }
    }
}
