using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gateways
{
    public partial class Assertigy : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public bool IsSuccess 
        {
            get 
            {
                Random rnd = new Random();
                if (rnd.NextDouble() <= 1)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
