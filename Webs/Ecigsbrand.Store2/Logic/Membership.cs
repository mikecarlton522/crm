using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrimFuel.Model;
using TrimFuel.Business;
using System.Web.Security;
using System.Security.Principal;

namespace Ecigsbrand.Store2.Logic
{
    public class Membership
    {
        public static void LoginReferer(Referer referer)
        {
            FormsAuthentication.SetAuthCookie(referer.Username, false);
        }

        public static void LogoutReferer()
        {
            FormsAuthentication.SignOut();
        }

        public static Referer ValidateReferer(string username, string password)
        {
            return (new RefererService()).GetByLogin(username, password);
        }

        private const string CURRENT_REFERER_KEY = "CurrentReferer";
        public static Referer CurrentReferer
        {
            get 
            {       
                IIdentity id = HttpContext.Current.User.Identity;
                if (id.IsAuthenticated)
                {
                    if (HttpContext.Current.Items[CURRENT_REFERER_KEY] == null)
                    {
                        HttpContext.Current.Items[CURRENT_REFERER_KEY] = (new RefererService()).GetByLogin(id.Name);
                    }
                    return (Referer)HttpContext.Current.Items[CURRENT_REFERER_KEY];
                }
                return null;
            }
        }
    }
}
