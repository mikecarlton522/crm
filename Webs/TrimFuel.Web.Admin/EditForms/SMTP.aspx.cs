using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.EditForms
{
    public partial class SMTP : System.Web.UI.Page
    {
        const string DEFAULT_SERVER = "relay.jangosmtp.net";

        SMTPSettingService service = new SMTPSettingService();

        SMTPSetting _setting = null;
        protected SMTPSetting Setting
        {
            get
            {
                if (_setting == null)
                    _setting = service.GetSMTPSetting();

                if (_setting == null)
                {
                    _setting = new SMTPSetting();
                    _setting.EnableSSL = false;
                    _setting.Server = DEFAULT_SERVER;
                }
                return _setting;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!string.IsNullOrEmpty(Request["action"]))
            //{
            //    Save(null, null);
            //}
        }

        protected void Save(object sender, EventArgs e)
        {
            if (Utility.TryGetStr(Request["server"]).Trim() == DEFAULT_SERVER)
                service.DeleteSMTPSetting();
            else
                service.SaveSMTPSetting(new SMTPSetting()
                {
                    UserName = Utility.TryGetStr(Request["userName"]),
                    Password = Utility.TryGetStr(Request["password"]),
                    Server = Utility.TryGetStr(Request["server"]),
                    Port = Utility.TryGetInt(Request["port"]),
                    EnableSSL = string.IsNullOrEmpty(Utility.TryGetStr(Request["ssl"])) ? false : true
                });
            Note.Text = "SMTP Settings successfully saved";
            DataBind();
        }

        protected void Reset_Click(object sender, EventArgs e)
        {
            service.DeleteSMTPSetting();
            Note.Text = "SMTP Settings successfully saved";
            DataBind();
        }
    }
}
