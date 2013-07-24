using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrimFuel.Web.Admin.Controls
{
    public partial class DateFilter : System.Web.UI.UserControl
    {
        public enum PredefinedMode
        {
            Backward = 1,
            Forward = 2
        }

        public PredefinedMode Mode
        {
            get 
            {
                if (ViewState["Mode"] == null)
                {
                    ViewState["Mode"] = PredefinedMode.Backward;
                }
                return (PredefinedMode)ViewState["Mode"]; 
            }
            set { ViewState["Mode"] = value; }
        }
        public DateTime? Date1 { get; set; }
        public DateTime? Date2 { get; set; }

        public DateTime Date1WithTime 
        {
            get
            {
                DateTime dt = DateTime.Today;
                if (Date1 != null)
                {
                    dt = Date1.Value;
                }
                return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            }
        }
        public DateTime Date2WithTime
        {
            get
            {
                DateTime dt = DateTime.Today;
                if (Date2 != null)
                {
                    dt = Date2.Value;
                }
                return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DateTime tempDt;
                if (DateTime.TryParse(Request["date1"], out tempDt))
                {
                    Date1 = tempDt;
                }
                if (DateTime.TryParse(Request["date2"], out tempDt))
                {
                    Date2 = tempDt;
                }
            }
            else
            {
                DateTime tempDt;
                if (DateTime.TryParse(tbDate1.Text.Trim(), out tempDt))
                {
                    Date1 = tempDt;
                }
                if (DateTime.TryParse(tbDate2.Text.Trim(), out tempDt))
                {
                    Date2 = tempDt;
                }
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            if (Date1 == null)
            {
                Date1 = DateTime.Today;
            }
            if (Date2 == null)
            {
                Date2 = DateTime.Today;
            }

            if (Date1 != null)
            {
                tbDate1.Text = Date1.Value.ToShortDateString();
            }
            if (Date2 != null)
            {
                tbDate2.Text = Date2.Value.ToShortDateString();
            }
        }

        protected IList<KeyValuePair<DateTime, DateTime>> DateValues
        {
            get
            {
                IList<KeyValuePair<DateTime, DateTime>> res = new List<KeyValuePair<DateTime, DateTime>>();

                if (Mode == PredefinedMode.Backward)
                {
                    //Yesterday
                    res.Add(new KeyValuePair<DateTime, DateTime>(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-1)));
                    //Today
                    res.Add(new KeyValuePair<DateTime, DateTime>(DateTime.Today, DateTime.Today));
                    //This Week
                    res.Add(new KeyValuePair<DateTime, DateTime>(DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek), DateTime.Today));
                    //Last Week
                    res.Add(new KeyValuePair<DateTime, DateTime>(DateTime.Today.AddDays(-(8 + (int)DateTime.Today.DayOfWeek)), DateTime.Today.AddDays(-(1 + (int)DateTime.Today.DayOfWeek))));
                    //This Month
                    res.Add(new KeyValuePair<DateTime, DateTime>(DateTime.Today.AddDays(1 - DateTime.Today.Day), DateTime.Today));
                    //Last Month
                    res.Add(new KeyValuePair<DateTime, DateTime>(DateTime.Today.AddDays(1 - DateTime.Today.Day).AddMonths(-1), DateTime.Today.AddDays(-DateTime.Today.Day)));
                }
                else if (Mode == PredefinedMode.Forward)
                {
                    //Tomorrow
                    res.Add(new KeyValuePair<DateTime, DateTime>(DateTime.Today.AddDays(1), DateTime.Today.AddDays(1)));
                    //Next 7 Days
                    res.Add(new KeyValuePair<DateTime, DateTime>(DateTime.Today, DateTime.Today.AddDays(7)));
                    //Next 30 Days
                    res.Add(new KeyValuePair<DateTime, DateTime>(DateTime.Today, DateTime.Today.AddDays(30)));
                    //Next 3 Months
                    res.Add(new KeyValuePair<DateTime, DateTime>(DateTime.Today, DateTime.Today.AddMonths(3)));
                    //Next 6 Months
                    res.Add(new KeyValuePair<DateTime, DateTime>(DateTime.Today, DateTime.Today.AddMonths(6)));
                }

                return res;
            }
        }

        protected string Date1Values 
        {
            get 
            {
                return string.Join(", ", DateValues.Select(item => string.Format("'{0}'", item.Key.ToShortDateString())).ToArray());
            }
        }

        protected string Date2Values
        {
            get
            {
                return string.Join(", ", DateValues.Select(item => string.Format("'{0}'", item.Value.ToShortDateString())).ToArray());
            }
        }
    }
}