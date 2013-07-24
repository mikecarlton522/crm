using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using Ecigsbrand.Store2.Logic;

namespace Ecigsbrand.Store2.Controls
{
    public partial class LoginToken : System.Web.UI.UserControl
    {
        public Referer Referer { get { return Membership.CurrentReferer; } }
    }        
}