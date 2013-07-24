using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using TrimFuel.Business;
using BeautyTruth.Store1.Logic;
using TrimFuel.Business.Utils;

namespace BeautyTruth.Store1
{
    public partial class _Default : PageX
    {
        protected List<ProductCodeInfo> _products;
        protected List<ProductCodeInfo> Products
        {
            get
            {
                if (_products == null)
                {
                    _products = new WebStoreService().GetRandomProductCodeInfoByCampaignID(GeneralInfo.CampaignID)
                        .Take(4)
                        .ToList();
                }
                return _products;

            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}
