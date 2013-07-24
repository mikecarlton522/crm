using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Business.Utils;
using System.Text;

namespace TrimFuel.Web.Admin.Controls
{
    public partial class DropDownFreeItem : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                ExtraTrialShipType = Utility.TryGetInt(Request["ddlFreeItems"]);
            }
        }

        public int? ExtraTrialShipType { get; set; }

        public string ProductGroupID { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            FillScriptLiteral();
            base.OnDataBinding(e);
        }

        private void FillScriptLiteral()
        {
            //adding productFreeList to js
            ProductService prService = new ProductService();
            var productProductCodeList = prService.GetProductProductCodeList();
            var productCodeList = prService.GetProductCodeList();

            StringBuilder script = new StringBuilder();
            script.AppendLine("var productFreeList = new Array();");
            int loop = 0;
            foreach (var item in (new DashboardService()).GetActiveFreeItems())
            {
                foreach (var prCode in productCodeList.Where(u => u.ProductCode_ == item.ProductCode))
                {
                    if (prCode != null)
                    {
                        foreach (var prProduct in productProductCodeList.Where(u => u.ProductCodeID == prCode.ProductCodeID))
                        {
                            script.Append(string.Format("productFreeList[{0}] = ", loop));
                            script.Append("{");
                            script.Append("ProductID : \"");
                            script.Append(prProduct == null ? 0 : prProduct.ProductID);
                            script.Append("\", ProductName : \"");
                            script.Append(item.DisplayName);
                            script.Append("\", ExtraTrialShipID : \"");
                            script.Append(item.ExtraTrialShipTypeID);
                            script.AppendLine("\"};");
                            loop++;
                        }
                    }
                }
            }
            litProductProductCodeList.Text = script.ToString();
            //adding productFreeList to js
        }
    }
}