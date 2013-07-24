using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace TrimFuel.Business.Gateways
{
    /// <summary>
    /// this class would be used for storing some additional params for BPag and Pagador payment gateways.
    /// </summary>
    public class PaymentExVars
    {
        public static int Installments
        {
            get
            {
                if (HttpContext.Current.Items["Installments"] == null)
                    return 1;
                else
                    return Convert.ToInt32(HttpContext.Current.Items["Installments"]);
            }

            set
            {
                HttpContext.Current.Items["Installments"] = value;
            }
        }
    }

    public class RequestResponseObject
    {
        public string Request { get; set; }
        public string Response { get; set; }
    }
}
