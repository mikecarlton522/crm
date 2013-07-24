using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class BillingDataProvider : EntityDataProvider<Billing>
    {
        private const string INSERT_COMMAND = "INSERT INTO Billing(CampaignID, RegistrationID, FirstName, LastName, CreditCard, CVV, PaymentTypeID, ExpMonth, ExpYear, Address1, Address2, City, State, Zip, Country, Email, Phone, CreateDT, Affiliate, SubAffiliate, IP, URL) VALUES(@CampaignID, @RegistrationID, @FirstName, @LastName, @CreditCard, @CVV, @PaymentTypeID, @ExpMonth, @ExpYear, @Address1, @Address2, @City, @State, @Zip, @Country, @Email, @Phone, @CreateDT, @Affiliate, @SubAffiliate, @IP, @URL); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE Billing SET CampaignID=@CampaignID, RegistrationID=@RegistrationID, FirstName=@FirstName, LastName=@LastName, CreditCard=@CreditCard, CVV=@CVV, PaymentTypeID=@PaymentTypeID, ExpMonth=@ExpMonth, ExpYear=@ExpYear, Address1=@Address1, Address2=@Address2, City=@City, State=@State, Zip=@Zip, Country=@Country, Email=@Email, Phone=@Phone, CreateDT=@CreateDT, Affiliate=@Affiliate, SubAffiliate=@SubAffiliate, IP=@IP, URL=@URL WHERE BillingID=@BillingID;";
        private const string SELECT_COMMAND = "SELECT * FROM Billing WHERE BillingID=@BillingID;";

        public override void Save(Billing entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.BillingID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;
            }

            cmd.Parameters.Add("@CampaignID", MySqlDbType.Int32).Value = entity.CampaignID;
            cmd.Parameters.Add("@RegistrationID", MySqlDbType.Int64).Value = entity.RegistrationID;
            cmd.Parameters.Add("@FirstName", MySqlDbType.VarChar).Value = entity.FirstName;
            cmd.Parameters.Add("@LastName", MySqlDbType.VarChar).Value = entity.LastName;
            cmd.Parameters.Add("@CreditCard", MySqlDbType.VarChar).Value = entity.CreditCard;
            cmd.Parameters.Add("@CVV", MySqlDbType.VarChar).Value = entity.CVV;
            cmd.Parameters.Add("@PaymentTypeID", MySqlDbType.Int32).Value = entity.PaymentTypeID;
            cmd.Parameters.Add("@ExpMonth", MySqlDbType.Int32).Value = entity.ExpMonth;
            cmd.Parameters.Add("@ExpYear", MySqlDbType.Int32).Value = entity.ExpYear;
            cmd.Parameters.Add("@Address1", MySqlDbType.VarChar).Value = entity.Address1;
            cmd.Parameters.Add("@Address2", MySqlDbType.VarChar).Value = entity.Address2;
            cmd.Parameters.Add("@City", MySqlDbType.VarChar).Value = entity.City;
            cmd.Parameters.Add("@State", MySqlDbType.VarChar).Value = entity.State;
            cmd.Parameters.Add("@Zip", MySqlDbType.VarChar).Value = entity.Zip;
            cmd.Parameters.Add("@Country", MySqlDbType.VarChar).Value = entity.Country;
            cmd.Parameters.Add("@Email", MySqlDbType.VarChar).Value = entity.Email;
            cmd.Parameters.Add("@Phone", MySqlDbType.VarChar).Value = entity.Phone;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;
            cmd.Parameters.Add("@Affiliate", MySqlDbType.VarChar).Value = entity.Affiliate;
            cmd.Parameters.Add("@SubAffiliate", MySqlDbType.VarChar).Value = entity.SubAffiliate;
            cmd.Parameters.Add("@IP", MySqlDbType.VarChar).Value = entity.IP;
            cmd.Parameters.Add("@URL", MySqlDbType.VarChar).Value = entity.URL;

            if (entity.BillingID == null)
            {
                entity.BillingID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Billing({0}) was not found in database.", entity.BillingID));
                }
            }
        }

        public override Billing Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Billing Load(DataRow row)
        {
            Billing res = new Billing();

            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt64(row["BillingID"]);
            if (!(row["CampaignID"] is DBNull))
                res.CampaignID = Convert.ToInt32(row["CampaignID"]);
            if (!(row["RegistrationID"] is DBNull))
                res.RegistrationID = Convert.ToInt64(row["RegistrationID"]);
            if (!(row["FirstName"] is DBNull))
                res.FirstName = Convert.ToString(row["FirstName"]);
            if (!(row["LastName"] is DBNull))
                res.LastName = Convert.ToString(row["LastName"]);
            if (!(row["CreditCard"] is DBNull))
                res.CreditCard = Convert.ToString(row["CreditCard"]);
            if (!(row["CVV"] is DBNull))
                res.CVV = Convert.ToString(row["CVV"]);
            if (!(row["PaymentTypeID"] is DBNull))
                res.PaymentTypeID = Convert.ToInt32(row["PaymentTypeID"]);
            if (!(row["ExpMonth"] is DBNull))
                res.ExpMonth = Convert.ToInt32(row["ExpMonth"]);
            if (!(row["ExpYear"] is DBNull))
                res.ExpYear = Convert.ToInt32(row["ExpYear"]);
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
            if (!(row["Country"] is DBNull))
                res.Country = Convert.ToString(row["Country"]);
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
