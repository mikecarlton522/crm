using System;
using System.Net;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class ApexSound : System.Web.UI.Page
    {
        private const string SOUND_URL = "http://203.153.53.212:2014/download.aspx?phone={0}";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string phone = Request["phone"];                

                if (string.IsNullOrEmpty(phone))
                {
                    Response.End();
                    
                    return;
                }

                try
                {
                    using (WebClientPlus client = new WebClientPlus())
                    {
                        client.HeadOnly = true;

                        string url = string.Format(SOUND_URL, phone);

                        byte[] body = client.DownloadData(url); //length should be 0

                        string type = client.ResponseHeaders["content-type"];

                        if (type.StartsWith("audio") || type.StartsWith("application"))
                            soundButton.Visible = true;
                    }
                }
                catch
                {
                    soundButton.Visible = false;
                }
            }
        }
    }

    class WebClientPlus : WebClient
    {
        public bool HeadOnly { get; set; }
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest req = base.GetWebRequest(address);
            if (HeadOnly && req.Method == "GET")
            {
                req.Method = "HEAD";
            }
            return req;
        }
    }
}