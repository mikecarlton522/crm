using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Securetrialoffers.admin
{
    public partial class report : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string StartDate
        {
            get
            {
                return TodayDate;
            }
        }

        public string EndDate
        {
            get
            {
                return TodayDate;
            }
        }

        public string TodayDate
        {
            get
            {
                return DateTime.Today.ToShortDateString();
            }
        }

        public string YesterdayDate
        {
            get
            {
                return DateTime.Today.AddDays(-1).ToShortDateString();
            }
        }

        public string WeekStartDate
        {
            get
            {
                return DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek).ToShortDateString();
            }
        }

        public string MonthStartDate
        {
            get
            {
                return DateTime.Today.AddDays(1 - DateTime.Today.Day).ToShortDateString();
            }
        }
    }
}
