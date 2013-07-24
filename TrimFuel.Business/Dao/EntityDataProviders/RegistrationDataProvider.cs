using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class RegistrationDataProvider : EntityDataProvider<Registration>
    {
        private const string INSERT_COMMAND = "INSERT INTO Registration(CampaignID, FirstName, LastName, Address1, Address2, City, State, Zip, Email, Phone, CreateDT, Affiliate, SubAffiliate, IP, URL) VALUES(@CampaignID, @FirstName, @LastName, @Address1, @Address2, @City, @State, @Zip, @Email, @Phone, @CreateDT, @Affiliate, @SubAffiliate, @IP, @URL); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE Registration SET CampaignID=@CampaignID, FirstName=@FirstName, LastName=@LastName, Address1=@Address1, Address2=@Address2, City=@City, State=@State, Zip=@Zip, Email=@Email, Phone=@Phone, CreateDT=@CreateDT, Affiliate=@Affiliate, SubAffiliate=@SubAffiliate, IP=@IP, URL=@URL WHERE RegistrationID=@RegistrationID;";
        private const string SELECT_COMMAND = "SELECT * FROM Registration WHERE RegistrationID=@RegistrationID;";

        public override void Save(Registration entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();
            
            if (entity.RegistrationID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@RegistrationID", MySqlDbType.Int64).Value = entity.RegistrationID;
            }

            cmd.Parameters.Add("@CampaignID", MySqlDbType.Int32).Value = entity.CampaignID;
            cmd.Parameters.Add("@FirstName", MySqlDbType.VarChar).Value = entity.FirstName;
            cmd.Parameters.Add("@LastName", MySqlDbType.VarChar).Value = entity.LastName;
            cmd.Parameters.Add("@Address1", MySqlDbType.VarChar).Value = entity.Address1;
            cmd.Parameters.Add("@Address2", MySqlDbType.VarChar).Value = entity.Address2;
            cmd.Parameters.Add("@City", MySqlDbType.VarChar).Value = entity.City;
            cmd.Parameters.Add("@State", MySqlDbType.VarChar).Value = entity.State;
            cmd.Parameters.Add("@Zip", MySqlDbType.VarChar).Value = entity.Zip;
            cmd.Parameters.Add("@Email", MySqlDbType.VarChar).Value = entity.Email;
            cmd.Parameters.Add("@Phone", MySqlDbType.VarChar).Value = entity.Phone;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;
            cmd.Parameters.Add("@Affiliate", MySqlDbType.VarChar).Value = entity.Affiliate;
            cmd.Parameters.Add("@SubAffiliate", MySqlDbType.VarChar).Value = entity.SubAffiliate;
            cmd.Parameters.Add("@IP", MySqlDbType.VarChar).Value = entity.IP;
            cmd.Parameters.Add("@URL", MySqlDbType.VarChar).Value = entity.URL;

            if (entity.RegistrationID == null)
            {
                entity.RegistrationID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Registration({0}) was not found in database.", entity.RegistrationID));
                }
            }
        }

        public override Registration Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@RegistrationID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Registration Load(DataRow row)
        {
            Registration res = new Registration();

            if (!(row["RegistrationID"] is DBNull))
                res.RegistrationID = Convert.ToInt64(row["RegistrationID"]);
            if (!(row["CampaignID"] is DBNull))
                res.CampaignID = Convert.ToInt32(row["CampaignID"]);
            if (!(row["FirstName"] is DBNull))
                res.FirstName = Convert.ToString(row["FirstName"]);
            if (!(row["LastName"] is DBNull))
                res.LastName = Convert.ToString(row["LastName"]);
            if (!(row["Address1"] is DBNull))
                res.Address1 = Convert.ToString(row["Address1"]);
            if (!(row["Address2"] is DBNull))
                res.Address2 = Convert.ToString(row["Address2"]);
            if (!(row["City"] is DBNull))
                res.City = Convert.ToString(row["City"]);
            if (!(row["State"] is DBNull))
                res.State = Convert.ToString(row["State"]);
            if (!(row["Zip"] is DBNull))
                res.Zip = Convert.ToString(row["Zip"]);
            if (!(row["Email"] is DBNull))
                res.Email = Convert.ToString(row["Email"]);
            if (!(row["Phone"] is DBNull))
                res.Phone = Convert.ToString(row["Phone"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["Affiliate"] is DBNull))
                res.Affiliate = Convert.ToString(row["Affiliate"]);
            if (!(row["SubAffiliate"] is DBNull))
                res.SubAffiliate = Convert.ToString(row["SubAffiliate"]);
            if (!(row["IP"] is DBNull))
                res.IP = Convert.ToString(row["IP"]);
            if (!(row["URL"] is DBNull))
                res.URL = Convert.ToString(row["URL"]);

            return res;
        }
    }
}
