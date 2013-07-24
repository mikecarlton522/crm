using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fitdiet.Store1.Logic;
using TrimFuel.Business.Utils;
using TrimFuel.Model;
using TrimFuel.Business;

namespace Fitdiet.Store1
{
    public partial class referer_code : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string refererCode = Utility.TryGetStr(Request["code"]);
            if (!string.IsNullOrEmpty(refererCode))
            {
                Referer referer = (new RefererService()).GetByCode(refererCode.Trim());
                if (referer != null)
                {
                    ShoppingCart.Current.RefererCode = refererCode.Trim();
                    ShoppingCart.Current.Save();
                }
            }
            Response.Redirect("~/default.aspx");
        }
    }
}
