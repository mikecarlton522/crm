using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class RefererSales : System.Web.UI.Page
    {
        RefererService refererService = new RefererService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
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
    }
}
