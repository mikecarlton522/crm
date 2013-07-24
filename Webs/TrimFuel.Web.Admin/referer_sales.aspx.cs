using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Business.Utils;
using TrimFuel.Web.Admin.Logic;

namespace TrimFuel.Web.Admin
{
    public partial class referer_sales : PageX
    {
        RefererService refererService = new RefererService();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private Referer referer = null;
        public Referer Referer
        {
            get
            {
                if (referer == null)
                {
                    int? refererID = Utility.TryGetInt(Request["refererId"]);
                    if (refererID != null)
                    {
                        referer = refererService.Load<Referer>(refererID);
                    }
                }
                return referer;
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            RefererSales1.Referer = Referer;
        }

        public override string HeaderString
        {
            get { return Referer.FullName; }
        }
    }
}
