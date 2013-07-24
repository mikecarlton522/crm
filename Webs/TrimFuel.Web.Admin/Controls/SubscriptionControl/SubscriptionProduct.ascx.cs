using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model.Views;
using TrimFuel.Business;

namespace TrimFuel.Web.Admin.Controls.SubscriptionControl
{
    public partial class SubscriptionProduct : System.Web.UI.UserControl
    {
        private SubscriptionNewService service = new SubscriptionNewService();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public int? ProductID { get; set; }
        public string SelectedGroupProductSKU { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            if (ProductID != null)
            {
                IList<GroupProductSKU> values = new List<GroupProductSKU>();
                IList<GroupProductSKU> loaded = service.GetGroupedProductList(ProductID.Value);
                foreach (var item in loaded)
                {
                    if (values.Count == 0 || values.Last().GroupVolume != item.GroupVolume)
                    {
                        values.Add(new GroupProductSKU() { GroupProductSKU_ = "", GroupProductName = item.GroupVolume.ToString() + "x Volume" });
                    }
                    item.GroupProductName = FixProductName(item.GroupProductName);
                    values.Add(item);
                }
                rProducts.DataSource = values;
            }
        }

        protected string FixProductName(string productName)
        {
            return productName
                .Replace("E-Cigarette ", "")
                .Replace("Refill - ", "")
                .Replace(" Flavour", "")
                .Replace(" Flavor", "")
                .Replace(" Flavo", "")
                .Replace("Strength", "Nicotine");
        }
    }
}