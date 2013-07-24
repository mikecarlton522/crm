using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fitdiet.Store1.Controls
{
    public partial class MenuControlPanel : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected string IsSelected(string pageName)
        {
            return (Request.Url.AbsolutePath.Contains(pageName)) ? " class=\"selected\"" : "";
        }
    }
}