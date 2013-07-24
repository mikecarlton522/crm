using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fitdiet.Store1.Logic;
using TrimFuel.Model;

namespace Fitdiet.Store1.Controls
{
    public partial class LoginToken : System.Web.UI.UserControl
    {
        public Referer Referer { get { return Membership.CurrentReferer; } }
    }        
}