using System;
using System.Web;
using System.Collections.Generic;
using System.Configuration;

namespace TrimFuel.Web.Module
{
    public class IpBlockingModule : IHttpModule
    {
        private const string BLOCK_IP_KEY = "BlockIP";
        private const string REDIRECT_URL = "http://www.google.com";

        #region IHttpModule Members

        public void Dispose()
        {
            //clean-up code here.
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
        }

        private void context_BeginRequest(object sender, EventArgs e)
        {
            string ip = HttpContext.Current.Request.UserHostAddress;
            if (IpAddresses.Contains(ip))
            {
                HttpContext.Current.Response.Redirect(REDIRECT_URL);
                HttpContext.Current.Response.End();
            }
        }

        #endregion

        private IList<string> ipAdresses = null;
        public IList<string> IpAddresses 
        {
            get 
            {
                if (ipAdresses == null)
                {
                    ipAdresses = new List<string>();

                    string raw = ConfigurationManager.AppSettings.Get(BLOCK_IP_KEY);
                    if (!string.IsNullOrEmpty(raw))
                    {
                        raw = raw.Replace(",", ";");
                        raw = raw.Replace(" ", ";");

                        foreach (string ip in raw.Split(';'))
                        {
                            ipAdresses.Add(ip.Trim());
                        }
                    }
                }
                return ipAdresses;
            }
        }
    }
}
