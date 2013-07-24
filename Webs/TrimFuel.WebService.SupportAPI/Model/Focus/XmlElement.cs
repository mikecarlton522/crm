using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrimFuel.WebService.SupportAPI.Model.Focus
{
    public class XmlElement
    {
        protected DateTime? ConvertToDate(string date)
        {
            if (!string.IsNullOrEmpty(date))
            {
                DateTime temp = DateTime.Now;
                if (DateTime.TryParse(date, out temp)) return temp;
            }
            return null;
        }

        protected int? ConvertToInt(string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                int temp = 0;
                if (int.TryParse(val, out temp)) return temp;
            }
            return null;
        }

        protected long? ConvertToLong(string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                long temp = 0;
                if (long.TryParse(val, out temp)) return temp;
            }
            return null;
        }
    }
}
