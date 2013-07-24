using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Fitdiet.Store1.Controls
{
    public partial class Menu : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected string ImgPrefix(string pageName)
        {
            return (Request.Url.AbsolutePath.Contains(pageName)) ? "-sel" : "";
        }

        protected string ImgHoverPrefix(string pageName)
        {
            return (Request.Url.AbsolutePath.Contains(pageName)) ? "-sel" : "-hl";
        }

        private string realativePathPrefix = null;
        protected string RealativePathPrefix 
        {
            get 
            {
                if (realativePathPrefix == null)
                {
                    realativePathPrefix = ResolveClientUrl("../");
                    if (realativePathPrefix == "./")
                        realativePathPrefix = "";
                }
                return realativePathPrefix;
            }
        }
    }
}