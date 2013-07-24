using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mail;
using TrimFuel.Model;

namespace TrimFuel.WebServices.ShippingAPI.Logic
{
    public class MailSenderSSL
    {
        public static bool SendEmail(
            string toAddress,
            string toName,
            string pSubject,
            string pBody,
            string fromAddress,
            string fromName,
            SMTPSetting setting)
        {
            try
            {
                System.Web.Mail.MailMessage myMail = new System.Web.Mail.MailMessage();
                myMail.Fields.Add
                    ("http://schemas.microsoft.com/cdo/configuration/smtpserver",
                                  setting.Server);
                myMail.Fields.Add
                    ("http://schemas.microsoft.com/cdo/configuration/smtpserverport",
                                  setting.Port);
                myMail.Fields.Add
                    ("http://schemas.microsoft.com/cdo/configuration/sendusing",
                                  "2");
                myMail.Fields.Add
                ("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1");
                //Use 0 for anonymous
                myMail.Fields.Add
                ("http://schemas.microsoft.com/cdo/configuration/sendusername",
                    setting.UserName);
                myMail.Fields.Add
                ("http://schemas.microsoft.com/cdo/configuration/sendpassword",
                     setting.Password);
                myMail.Fields.Add
                ("http://schemas.microsoft.com/cdo/configuration/smtpusessl",
                     "true");

                myMail.From = fromAddress;
                myMail.To = toAddress;
                myMail.Subject = pSubject;
                myMail.BodyFormat = MailFormat.Html;
                myMail.Body = pBody;

                System.Web.Mail.SmtpMail.SmtpServer = string.Format("{0}:{1}", setting.Server, setting.Port);
                System.Web.Mail.SmtpMail.Send(myMail);
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}