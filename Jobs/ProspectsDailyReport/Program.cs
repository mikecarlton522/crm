using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using log4net;
using System.Net.Mail;
using System.IO;
using TrimFuel.Business.Dao;
using TrimFuel.Model.Views;
using MySql.Data.MySqlClient;
using TrimFuel.Model;
using TrimFuel.Business;

namespace ProspectsDailyReport
{
    class Program
    {
        static DateTime ENDDATE = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        static DateTime STARTDATE = ENDDATE.AddDays(-1);
        static string _stoConnectionString = ConfigurationManager.ConnectionStrings["TrimFuel"].ConnectionString;
        static ILog logger = null;
        static string SUBJECT_TEMPLATE = "{0} : Prospects report";
        static string BODY = string.Format("Hi,<br/>Please find attached the prospects report for date {0}/{1}/{2}.", STARTDATE.Day, STARTDATE.Month, STARTDATE.Year);

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            log4net.ThreadContext.Properties["ApplicationID"] = "All clients";
            logger = LogManager.GetLogger(typeof(Program));

            ProcessProspectsClients();
        }

        static void ProcessProspectsClients()
        {
            IDao dao = new MySqlDao(_stoConnectionString);
            MySqlCommand q = new MySqlCommand("SELECT * FROM TPClient WHERE ProcessProspects=1");
            var clients = dao.Load<TPClient>(q);
            foreach (var client in clients)
            {
                //create Client DAO
                string[] emails = ConfigurationManager.AppSettings[client.DomainName + "_prospects_emails"].Split(',');
                dao = new MySqlDao(client.ConnectionString);
                ProcessClient(dao, client.Name, emails);
            }
        }

        private static void ProcessClient(IDao dao, string clientName, string[] emails)
        {
            var content = ProcessProspects(dao);
            SendEmail(content, clientName, emails);
        }

        private static string ProcessProspects(IDao dao)
        {
            StringBuilder content = new StringBuilder();
            content.AppendLine("\"RegistrationID\",\"Flow\",\"Date\",\"First Name\",\"Last Name\",\"Address\",\"City\",\"State\",\"Zip\",\"Phone\",\"Email\",\"Affiliate\",\"SubAffiliate\",\"IP\"");
            try
            {
                var q = new MySqlCommand(@"Select distinct c.DisplayName as CampaignName, r.* from Registration r 
                                                left join Billing b on b.RegistrationID=r.RegistrationID
                                                left join BillingSubscription bs on bs.BillingID=b.BillingID
                                                left join Campaign c on c.CampaignID=r.CampaignID
                                                Where r.CreateDT Between @StartDate and @EndDate
                                                and bs.BillingSubscriptionID is null
                                                and coalesce(r.Email,'') <> '' 
                                                and lower(r.FirstName) <> 'test'
                                                group by r.RegistrationID");
                q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = STARTDATE;
                q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = ENDDATE;
                var report = dao.Load<ProspectView>(q);
                foreach (var res in report)
                {
                    content.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\"",
                                       res.Registration.RegistrationID, res.CampaignName, res.Registration.CreateDT, res.Registration.FirstName, res.Registration.LastName,
                                       res.Registration.Address1, res.Registration.City, res.Registration.State, res.Registration.Zip,
                                       res.Registration.Phone, res.Registration.Email, res.Registration.Affiliate, res.Registration.SubAffiliate, res.Registration.IP));
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return content.ToString();
        }

        private static void SendEmail(string content, string clientName, string[] emails)
        {
            MailMessage mail = new MailMessage();
            mail.Body = BODY;
            mail.Subject = string.Format(SUBJECT_TEMPLATE, clientName);
            mail.IsBodyHtml = true;
            mail.From = new MailAddress("donotreply@trianglecrm.com", "TriangleCRM Automated Reporting");

            using (var ms = new MemoryStream(ASCIIEncoding.Default.GetBytes(content)))
            {
                mail.Attachments.Add(new Attachment(ms, "Daily Report.csv", "text/csv"));
                foreach (var email in emails)
                {
                    mail.To.Add(email);
                    SmtpClient smtp = new SmtpClient("relay.jangosmtp.net");
                    smtp.Send(mail);
                }
            }
        }
    }
}
