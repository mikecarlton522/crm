using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Web.Admin.Logic;
using TrimFuel.Model;
using TrimFuel.Business.Gateways.DefaultEmail;

namespace TrimFuel.Web.Admin
{
    public partial class management_smtp : PageX
    {
        SMTPSettingService service = new SMTPSettingService();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            phCurrent.Visible = false;
            phDefault.Visible = true;

            IList<SMTPSetting> settings = new List<SMTPSetting>();
            var currSetting = service.GetSMTPSetting();
            if (currSetting != null)
            {
                settings.Add(currSetting);
                phCurrent.Visible = true;
                phDefault.Visible = false;
            }
            rSMTP.DataSource = settings;
        }

        public override string HeaderString
        {
            get { return "SMTP Configuration"; }
        }
    }
}
