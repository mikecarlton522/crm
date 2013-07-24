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
    public partial class AlsoBought : System.Web.UI.UserControl
    {
        public int CountToShow { get; set; }
        public string ProductCode { get; set; }
        public string GridClass { get; set; }
        int? CategoryID
        {
            get
            {
                var cat = new WebStoreService().GetCategoryByProductCode(ProductCode);
                return cat == null ? 0 : cat.ProductCategoryID;
            }
        }

        List<KeyValuePair<string, string>> _products;
        protected List<KeyValuePair<string, string>> Products
        {
            get
            {
                if (_products == null)
                {
                    var prods = new WebStoreService().GetRandomProductCodeInfoByCampaignID(GeneralInfo.CampaignID).Where(u => u.ProductCode_ != ProductCode && u.FeaturedProduct == true).Take(CountToShow).ToList();
                    _products = prods.Select((u, i) => new KeyValuePair<string, string>(u.ProductCode_, GetAdditionStringByIndex(i))).ToList();
                }
                return _products;

            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                rProducts.DataBind();
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
        }

        private string GetAdditionStringByIndex(int index)
        {
            if (index % CountToShow == 0)
                return "alpha";
            else
                if ((index + 1) % CountToShow == 0)
                    return "omega";
                else return "";
        }
    }
}