using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using TrimFuel.Business.Utils;

namespace Securetrialoffers.admin.Logic.UI
{
    public abstract class ReportBasePage : Page
    {
        protected string strStartDate = null;
        protected string strEndDate = null;

        protected DateTime? StartDate { get; set; }
        protected DateTime? EndDate { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            LoadInput();
            ResolveDates();

            base.OnLoad(e);
        }

        private void LoadInput()
        {
            strStartDate = Utility.TryGetStr(Request.Params["date1"]);
            strEndDate = Utility.TryGetStr(Request.Params["date2"]);
        }

        private void ResolveDates()
        {
            StartDate = Utility.TryGetDate(Request.Params["date1"]);
            EndDate = Utility.TryGetDate(Request.Params["date2"]);
        }

        protected string ReportTitle
        {
            get
            {
                if (StartDate != null && EndDate != null)
                {
                    return string.Format("{0}: {1} to {2}",
                        ReportName,
                        StartDate.Value.ToShortDateString(),
                        EndDate.Value.ToShortDateString());
                }
                else if (StartDate != null)
                {
                    return string.Format("{0}: {1} to present",
                        ReportName,
                        StartDate.Value.ToShortDateString());
                }
                else if (EndDate != null)
                {
                    return string.Format("{0}: All dates to {1}",
                        ReportName,
                        EndDate.Value.ToShortDateString());
                }

                return string.Format("{0}: All dates", ReportName);
            }
        }

        public abstract string ReportName { get; }
    }
}
