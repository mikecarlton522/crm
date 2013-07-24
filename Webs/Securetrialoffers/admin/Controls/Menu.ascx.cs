using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Securetrialoffers.admin.Controls
{
    public partial class Menu : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public bool Adrel 
        {
            get
            {
                return false;
            }
        }

        public string MenuWidth 
        {
            get 
            {
                return (Adrel) ? " width: 321px;" : "";
            }
        }
    }
}