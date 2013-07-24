using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    class TPClientSettingDataProvider : EntityDataProvider<TPClientSetting>
    {
        private const string SELECT_COMMAND = "SELECT * FROM TPClientSettings WHERE TPClientSettingID=@TPClientSettingID;";
        private const string UPDATE_COMMAND = @"UPDATE TPClientSettings SET id_tpclient=@TPClientID, lbn=@LegalBusinessName, dba=@DBAName, contact_name=@ContactName, contact_phone=@ContactPhone, username=@Username, password=@Password, secondary_name=@SecondaryContactName, last_login_date=@LastLoginDate,
                product_offered=@ProductOffered, billing_model_recurring=@BillingModel, customer_service_phone=@CustomerServicePhoneNumber, page_url=@MarketingPageURL, open_sales_regions=@OpenSalesRegions WHERE TPClientSettingID=@TPClientSettingID;";
        private const string INSERT_COMMAND = @"INSERT INTO TPClientSettings(id_tpclient, lbn, dba, contact_name, contact_phone, username, password, secondary_name, last_login_date, product_offered, billing_model_recurring, customer_service_phone, page_url, open_sales_regions)
                VALUES(@TPClientID, @LegalBusinessName, @DBAName, @ContactName, @ContactPhone, @Username, @Password, @SecondaryContactName, @LastLoginDate, @ProductOffered, @BillingModel, @CustomerServicePhoneNumber, @MarketingPageURL, @OpenSalesRegions); SELECT @@IDENTITY;";

        public override void Save(TPClientSetting entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.TPClientSettingID == 0)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@TPClientSettingID", MySqlDbType.Int32).Value = entity.TPClientSettingID;
            }

            cmd.Parameters.Add("@TPClientID", MySqlDbType.Int32).Value = entity.TPClientID;
            cmd.Parameters.Add("@LegalBusinessName", MySqlDbType.VarChar).Value = entity.LegalBusinessName;
            cmd.Parameters.Add("@DBAName", MySqlDbType.VarChar).Value = entity.DBAName;
            cmd.Parameters.Add("@ContactName", MySqlDbType.VarChar).Value = entity.ContactName;
            cmd.Parameters.Add("@ContactPhone", MySqlDbType.VarChar).Value = entity.ContactPhone;
            cmd.Parameters.Add("@Username", MySqlDbType.VarChar).Value = entity.Username;
            cmd.Parameters.Add("@Password", MySqlDbType.VarChar).Value = entity.Password;
            cmd.Parameters.Add("@SecondaryContactName", MySqlDbType.VarChar).Value = entity.SecondaryContactName;
            cmd.Parameters.Add("@LastLoginDate", MySqlDbType.Datetime).Value = entity.LastLoginDate;
            cmd.Parameters.Add("@ProductOffered", MySqlDbType.VarChar).Value = entity.ProductOffered;
            cmd.Parameters.Add("@BillingModel", MySqlDbType.Bit).Value = entity.BillingModel;
            cmd.Parameters.Add("@CustomerServicePhoneNumber", MySqlDbType.VarChar).Value = entity.CustomerServicePhoneNumber;
            cmd.Parameters.Add("@MarketingPageURL", MySqlDbType.VarChar).Value = entity.MarketingPageURL;
            cmd.Parameters.Add("@OpenSalesRegions", MySqlDbType.VarChar).Value = entity.OpenSalesRegions;

            if (entity.TPClientSettingID == 0)
            {
                entity.TPClientSettingID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("TPClientSettings({0}) was not found in database.", entity.TPClientSettingID));
                }
            }
        }

        public override TPClientSetting Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@TPClientSettingID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override TPClientSetting Load(System.Data.DataRow row)
        {
            TPClientSetting res = new TPClientSetting();

            if (!(row["TPClientSettingID"] is DBNull))
                res.TPClientSettingID = Convert.ToInt32(row["TPClientSettingID"]);
            if (!(row["id_tpclient"] is DBNull))
                res.TPClientID = Convert.ToInt32(row["id_tpclient"]);
            if (!(row["lbn"] is DBNull))
                res.LegalBusinessName = Convert.ToString(row["lbn"]);
            if (!(row["dba"] is DBNull))
                res.DBAName = Convert.ToString(row["dba"]);
            if (!(row["contact_name"] is DBNull))
                res.ContactName = Convert.ToString(row["contact_name"]);
            if (!(row["contact_phone"] is DBNull))
                res.ContactPhone = Convert.ToString(row["contact_phone"]);
            if (!(row["username"] is DBNull))
                res.Username = Convert.ToString(row["username"]);
            if (!(row["password"] is DBNull))
                res.Password = Convert.ToString(row["password"]);
            if (!(row["secondary_name"] is DBNull))
                res.SecondaryContactName = Convert.ToString(row["secondary_name"]);
            if (!(row["last_login_date"] is DBNull))
                res.LastLoginDate = Convert.ToDateTime(row["last_login_date"]);
            if (!(row["product_offered"] is DBNull))
                res.ProductOffered = Convert.ToString(row["product_offered"]);
            if (!(row["billing_model_recurring"] is DBNull))
                res.BillingModel = Convert.ToBoolean(row["billing_model_recurring"]);
            if (!(row["customer_service_phone"] is DBNull))
                res.CustomerServicePhoneNumber = Convert.ToString(row["customer_service_phone"]);
            if (!(row["page_url"] is DBNull))
                res.MarketingPageURL = Convert.ToString(row["page_url"]);
            if (!(row["open_sales_regions"] is DBNull))
                res.OpenSalesRegions = Convert.ToString(row["open_sales_regions"]);

            return res;
        }
    }
}
