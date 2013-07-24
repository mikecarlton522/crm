using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class RefererDataProvider : EntityDataProvider<Referer>
    {
        private const string INSERT_COMMAND = "INSERT INTO Referer(FirstName, LastName, Company, Address1, Address2, City, State, Zip, Country, RefererCode, ParentRefererID, CreateDT, Username, Password) VALUES(@FirstName, @LastName, @Company, @Address1, @Address2, @City, @State, @Zip, @Country, @RefererCode, @ParentRefererID, @CreateDT, @Username, @Password); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE Referer SET FirstName=@FirstName, LastName=@LastName, Company=@Company, Address1=@Address1, Address2=@Address2, City=@City, State=@State, Zip=@Zip, Country=@Country, RefererCode=@RefererCode, ParentRefererID=@ParentRefererID, CreateDT=@CreateDT, Username=@Username, Password=@Password WHERE RefererID=@RefererID;";
        private const string SELECT_COMMAND = "SELECT * FROM Referer WHERE RefererID=@RefererID;";

        public override void Save(Referer entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.RefererID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@RefererID", MySqlDbType.Int32).Value = entity.RefererID;
            }

            cmd.Parameters.Add("@FirstName", MySqlDbType.VarChar).Value = entity.FirstName;
            cmd.Parameters.Add("@LastName", MySqlDbType.VarChar).Value = entity.LastName;
            cmd.Parameters.Add("@Company", MySqlDbType.VarChar).Value = entity.Company;
            cmd.Parameters.Add("@Address1", MySqlDbType.VarChar).Value = entity.Address1;
            cmd.Parameters.Add("@Address2", MySqlDbType.VarChar).Value = entity.Address2;
            cmd.Parameters.Add("@City", MySqlDbType.VarChar).Value = entity.City;
            cmd.Parameters.Add("@State", MySqlDbType.VarChar).Value = entity.State;
            cmd.Parameters.Add("@Zip", MySqlDbType.VarChar).Value = entity.Zip;
            cmd.Parameters.Add("@Country", MySqlDbType.VarChar).Value = entity.Country;
            cmd.Parameters.Add("@RefererCode", MySqlDbType.VarChar).Value = entity.RefererCode;
            cmd.Parameters.Add("@ParentRefererID", MySqlDbType.Int32).Value = entity.ParentRefererID;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;
            cmd.Parameters.Add("@Username", MySqlDbType.VarChar).Value = entity.Username;
            cmd.Parameters.Add("@Password", MySqlDbType.VarChar).Value = entity.Password;

            if (entity.RefererID == null)
            {
                entity.RefererID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Referer({0}) was not found in database.", entity.RefererID));
                }
            }
        }

        public override Referer Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@RefererID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Referer Load(DataRow row)
        {
            Referer res = new Referer();

            if (!(row["RefererID"] is DBNull))
                res.RefererID = Convert.ToInt32(row["RefererID"]);
            if (!(row["FirstName"] is DBNull))
                res.FirstName = Convert.ToString(row["FirstName"]);
            if (!(row["LastName"] is DBNull))
                res.LastName = Convert.ToString(row["LastName"]);
            if (!(row["Company"] is DBNull))
                res.Company = Convert.ToString(row["Company"]);
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
            if (!(row["RefererCode"] is DBNull))
                res.RefererCode = Convert.ToString(row["RefererCode"]);
            if (!(row["ParentRefererID"] is DBNull))
                res.ParentRefererID = Convert.ToInt32(row["ParentRefererID"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["Username"] is DBNull))
                res.Username = Convert.ToString(row["Username"]);
            if (!(row["Password"] is DBNull))
                res.Password = Convert.ToString(row["Password"]);

            return res;
        }
    }
}
