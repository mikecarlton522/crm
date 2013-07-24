using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BeautyTruth.Store1.Logic;
using TrimFuel.Model;

namespace BeautyTruth.Store1.Controls
{
    public partial class Front : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public int TotalProductCount
        {
            get
            {
                return (int)ShoppingCart.Current.Products.Sum(item => item.Value);
            }
        }

        public Referer Referer
        {
            get { return Membership.CurrentReferer; }
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
