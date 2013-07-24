using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Web.Admin.Logic;

namespace TrimFuel.Web.Admin
{
    public partial class product_manager : PageX
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public override string HeaderString
        {
            get { return "Product Group Manager"; }
        }
    }
}
