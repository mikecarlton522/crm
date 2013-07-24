using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using TrimFuel.Business;
using System.Net.Mail;
using System.Web.Script.Services;
using TrimFuel.Model;
using TrimFuel.WebServices.ShippingAPI.Logic;

namespace TrimFuel.WebServices.ShippingAPI
{
    /// <summary>
    /// Summary description for emails
    /// </summary>
    [WebService(Namespace = "http://trianglecrm.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class emails : System.Web.Services.WebService
    {
        JobService jobService = new JobService();
        EmailService emailService = new EmailService();

        [WebMethod]
        public void SendAbandonEmails()
        {
            jobService.SendAbandonEmails();
        }

        [WebMethod]
        public void SendNewsletterEmails()
        {
            jobService.SendNewsletterEmails();
        }

        [WebMethod]
        public void SendRMAEmail(int billingID, string rma)
        {

            emailService.SendRMAEmail(billingID, rma);

        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string SendEmailUsingCustomSMTP(string fromName, string fromAddress, string toName, string toAddress, string body, string subject)
        {

            var setting = new SMTPSettingService().GetSMTPSetting();
            if (setting != null)
            {
                try
                {
                    if (setting.EnableSSL != true)
                    {
                        System.Net.Mail.MailMessage mMailMessage = new System.Net.Mail.MailMessage();
                        mMailMessage.From = new MailAddress(fromAddress, fromName);
                        mMailMessage.To.Add(new MailAddress(toAddress, toName));
                        mMailMessage.Subject = subject;
                        mMailMessage.Body = body;
                        mMailMessage.IsBodyHtml = true;

                        SmtpClient mSmtpClient = new SmtpClient();
                        mSmtpClient.Host = setting.Server;
                        mSmtpClient.Port = setting.Port ?? 25;
                        mSmtpClient.EnableSsl = false;
                        mSmtpClient.UseDefaultCredentials = false;
                        mSmtpClient.Credentials = new System.Net.NetworkCredential(setting.UserName, setting.Password);

                        mSmtpClient.Send(mMailMessage);
                    }
                    else
                        MailSenderSSL.SendEmail(toAddress, toName, subject, body, fromAddress, fromName, setting);
                    return string.Format("OK. Message sent.");
                }
                catch (Exception ex)
                {
                    return string.Format("Error. {0}", "Message can not be sent using that settings");
                }
            }
            return string.Format("Error. {0}", "No SMTP settings");
        }

        [WebMethod]
        public void ProcessEmailQueue(long billingID)
        {
            new EmailService().ProcessEmailQueue(billingID);
        }

        [WebMethod]
        public void ProcessAllEmailQueue()
        {
            new EmailService().ProcessEmailQueue();
        }
    }
}
