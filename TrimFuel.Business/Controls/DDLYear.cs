using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace TrimFuel.Business.Controls
{
    public class DDLYear : DropDownList
    {
        //public enum ModeEnum
        //{
        //    FourDigits = 1,
        //    TwoDigits = 2
        //}

        private const int DDL_YEAR_DEFAULT_END = 2025;

        public DDLYear()
        {
            StartYear = DateTime.Today.Year;
            //Mode = ModeEnum.FourDigits;
        }

        public int StartYear { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            int selectedIndex = SelectedIndex;
            Items.Clear();

            foreach (KeyValuePair<string, string> v in GetValues(StartYear, DDL_YEAR_DEFAULT_END))
            {
                Items.Add(new ListItem(v.Value, v.Key));
            }

            SelectedIndex = selectedIndex;

            base.OnDataBinding(e);
        }

        private Dictionary<string, string> GetValues(int startYear, int endYear)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            for (int i = startYear; i < endYear + 1; i++)
            {
                values.Add(i.ToString(), i.ToString());
            }
            return values;
        }
    }
}
