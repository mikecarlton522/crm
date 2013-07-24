using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace TrimFuel.Business.Controls
{
    public class DDLMonth : DropDownList
    {
        public DDLMonth()
        {
            Mode = ModeEnum.OneOrTwoDigits;
        }

        public enum ModeEnum
        {
            OneOrTwoDigits = 1,
            TwoDigits = 2
        }

        private Dictionary<string, string> values = new Dictionary<string, string>()
        {
            {"1", "Jan (1)"},
            {"2", "Feb (2)"},
            {"3", "Mar (3)"},
            {"4", "Apr (4)"},
            {"5", "May (5)"},
            {"6", "Jun (6)"},
            {"7", "Jul (7)"},
            {"8", "Aug (8)"},
            {"9", "Sep (9)"},
            {"10", "Oct (10)"},
            {"11", "Nov (11)"},
            {"12", "Dec (12)"}
        };

        public ModeEnum Mode { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            int selectedIndex = SelectedIndex;
            Items.Clear();

            foreach (KeyValuePair<string, string> v in values)
            {
                string val = v.Key;
                if (Mode == ModeEnum.TwoDigits && val.Length < 2)
                {
                    val = "0" + val;
                }
                Items.Add(new ListItem(v.Value, val));
            }

            SelectedIndex = selectedIndex;

            base.OnDataBinding(e);
        }
    }
}
