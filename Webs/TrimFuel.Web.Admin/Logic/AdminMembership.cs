using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrimFuel.Model;
using TrimFuel.Business;

namespace TrimFuel.Web.Admin.Logic
{
    public class AdminMembership
    {
        public static TrimFuel.Model.Admin CurrentAdmin
        {
            get
            {
                string adminName = CurrentAdminName;
                if (adminName != null)
                {
                    TrimFuel.Business.DashboardService srv = new TrimFuel.Business.DashboardService();
                    return srv.GetAdminByName(adminName);
                }
                return null;
                //return new TrimFuel.Business.DashboardService().Load<Admin>(334);
            }
        }

        public static string CurrentAdminRole
        {
            get
            {
                TrimFuel.Model.Admin admin = CurrentAdmin;
                if (CurrentAdmin != null)
                {
                    TrimFuel.Business.DashboardService srv = new TrimFuel.Business.DashboardService();
                    RestrictLevel l = srv.GetRestrictLevelByID(admin.RestrictLevel ?? 0);
                    if (l != null)
                    {
                        return l.DisplayName;
                    }
                }
                return null;
            }
        }

        private static string CurrentAdminName
        {
            get 
            {
                if (HttpContext.Current.Request.Cookies["admName4Net"] != null)
                {
                    return HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies["admName4Net"].Value);
                }
                return null;
            }
        }

        public static bool IsAuthenticated
        {
            get
            {
                return CurrentAdminName != null;
            }
        }

        public static int? CurrentCampaignID
        {
            get
            {
                if (Config.Current.APPLICATION_ID == TrimFuel.Model.Enums.ApplicationEnum.TriangleCRM ||
                    Config.Current.APPLICATION_ID == TrimFuel.Model.Enums.ApplicationEnum.LocalhostTriangleCRM)
                {
                    return 1020;
                }
                TrimFuel.Business.DashboardService srv = new TrimFuel.Business.DashboardService();
                Campaign cmp = srv.GetOrCreateCRMCampaign();
                if (cmp != null)
                {
                    return cmp.CampaignID;
                }
                return null;
            }
        }
    }
}
