using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business
{
    public class SMTPSettingService : BaseService
    {
        static string CUSTOM_SMTP_URL = "https://" + Config.Current.APPLICATION_ID + "/dotNetJobs/Emails.asmx/SendEmailUsingCustomSMTP";

        public SMTPSetting GetSMTPSetting()
        {
            SMTPSetting res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("Select * from SMTPSetting Limit 1");
                res = dao.Load<SMTPSetting>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public SMTPSetting GetSMTPSetting(int? id)
        {
            SMTPSetting res = null;
            try
            {
                res = dao.Load<SMTPSetting>(id);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public void SaveSMTPSetting(SMTPSetting setting)
        {
            try
            {
                dao.BeginTransaction();
                var current = GetSMTPSetting();
                if (current != null)
                    setting.SMTPSettingID = current.SMTPSettingID;

                setting.URL = CUSTOM_SMTP_URL;

                dao.Save<SMTPSetting>(setting);
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
            }
        }

        public void DeleteSMTPSetting()
        {
            try
            {
                MySqlCommand q = new MySqlCommand("Delete from SMTPSetting");
                dao.ExecuteNonQuery(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }
    }
}
