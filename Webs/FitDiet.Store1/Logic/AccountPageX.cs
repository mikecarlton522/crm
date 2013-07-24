using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using TrimFuel.Model;

namespace Fitdiet.Store1.Logic
{
    public class AccountPageX : PageX
    {
        public Referer Referer 
        {
            get { return Membership.CurrentReferer; }
        }
    }
}
