using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrimFuel.Business;
using System.Text;
using TrimFuel.Business.Utils;

namespace TrimFuel.WebServices.BillingAPI.Logic
{
    public class LoggingModule : IHttpModule
    {
        public LoggingModule()
        {
        }

        #region IHttpModule Members

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
            context.EndRequest += new EventHandler(context_EndRequest);
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            try
            {
                RequestString = new StringBuilder();
                ResponseString = new StringBuilder();

                if (HttpContext.Current.Request.InputStream != null)
                {
                    HttpContext.Current.Request.InputStream.Seek(0, System.IO.SeekOrigin.Begin);
                    RequestString.Append(Utility.Stream2String(HttpContext.Current.Request.InputStream, HttpContext.Current.Request.ContentEncoding));
                    HttpContext.Current.Request.InputStream.Seek(0, System.IO.SeekOrigin.Begin);

                    if (HttpContext.Current != null && HttpContext.Current.Response != null)
                    {
                        HttpContext.Current.Response.Filter = new LoggingOutputFilter(HttpContext.Current.Response.Filter, ResponseString);
                    }
                }
            }
            catch { }
        }

        public StringBuilder RequestString 
        {
            get 
            {
                return (StringBuilder)HttpContext.Current.Items["Logging_RequestString"];
            }
            set 
            {
                HttpContext.Current.Items["Logging_RequestString"] = value;
            }
        }

        public StringBuilder ResponseString
        {
            get
            {
                return (StringBuilder)HttpContext.Current.Items["Logging_ResponseString"];
            }
            set
            {
                HttpContext.Current.Items["Logging_ResponseString"] = value;
            }
        }

        void context_EndRequest(object sender, EventArgs e)
        {
            try
            {
                if (HttpContext.Current != null)
                {
                    BillingService serv = new BillingService();
                    if (HttpContext.Current != null)
                    {
                        serv.LogBillingAPIRequest(HttpContext.Current.Request, RequestString, ResponseString);
                    }
                }
            }
            catch { }
        }

        #endregion
    }
}
