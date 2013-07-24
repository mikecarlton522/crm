using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using TrimFuel.Business.Dao;
using MySql.Data.MySqlClient;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Business.Gateways;
using TrimFuel.Business.Gateways.DefaultEmail;
using TrimFuel.Business.Utils;
using log4net;
using System.Configuration;

namespace CoAction_InventoryEmail
{
    class Program
    {
        static void Main(string[] args)
        {
            ILog logger = LogManager.GetLogger(typeof(Program));
            log4net.Config.XmlConfigurator.Configure();
            IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);

            try
            {
                string FROM_ADDRESS = ConfigurationSettings.AppSettings["From"];
                string FROM_NAME = ConfigurationSettings.AppSettings["FromName"];
                string SUBJECT = ConfigurationSettings.AppSettings["Subject"];
                string[] TO = ConfigurationSettings.AppSettings["To"].Split(',');

                MySqlCommand q = new MySqlCommand("select * from Inventory");
                IList<Inventory> inventoryList = dao.Load<Inventory>(q);

                StringBuilder csv = new StringBuilder();

                foreach (Inventory inventory in inventoryList)
                {
                    csv.AppendFormat("{0},{1},{2}", inventory.SKU, inventory.Product, inventory.InStock);
                    csv.AppendLine();
                }

                MemoryStream stream = new MemoryStream(UTF32Encoding.Default.GetBytes(csv.ToString()));
                stream.Position = 0;

                string date = DateTime.Today.ToString("yyyy_MM_dd");

                Attachment att = new Attachment(stream, "daily_inventory_report_" + date + ".csv");

                MailMessage message = new MailMessage();
                message.Attachments.Add(att);
                message.Body = "Report attached";
                message.From = new MailAddress(FROM_ADDRESS, FROM_NAME);
                message.Subject = SUBJECT;
                foreach (string item in TO)
                {
                    message.To.Add(new MailAddress(item));
                }                

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "relay.jangosmtp.net";
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                logger.Error("Fatal Error", ex);
            }
        }
    }
}
