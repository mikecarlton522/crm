using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class SMTPSetting : Entity
    {
        public int? SMTPSettingID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public string URL { get; set; }
        public int? Port { get; set; }
        public bool? EnableSSL { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("UserName", UserName, 100);
            v.AssertString("Password", Password, 100);
            v.AssertString("Server", Server, 100);
            v.AssertString("URL", URL, 250);
        }
    }
}
