using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class BillingExternalInfoDataProvider : EntityDataProvider<BillingExternalInfo>
    {
        private const string INSERT_COMMAND = "INSERT INTO BillingExternalInfo(BillingID, InternalID, CustomField1, CustomField2, CustomField3, CustomField4, CustomField5) VALUES(@BillingID, @InternalID, @CustomField1, @CustomField2, @CustomField3, @CustomField4, @CustomField5);";
        private const string UPDATE_COMMAND = "UPDATE BillingExternalInfo SET InternalID=@InternalID, CustomField1=@CustomField1, CustomField2=@CustomField2, CustomField3=@CustomField3, CustomField4=@CustomField4, CustomField5=@CustomField5 WHERE BillingID=@BillingID;";
        private const string SELECT_COMMAND = "SELECT * FROM BillingExternalInfo WHERE BillingID=@BillingID;";

        public override void Save(BillingExternalInfo entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            //Billing 1 <-> 0..1 BillingExternalInfo association
            //Try to update first
            cmd.CommandText = UPDATE_COMMAND;

            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;
            cmd.Parameters.Add("@InternalID", MySqlDbType.VarChar).Value = entity.InternalID;
            cmd.Parameters.Add("@CustomField1", MySqlDbType.VarChar).Value = entity.CustomField1;
            cmd.Parameters.Add("@CustomField2", MySqlDbType.VarChar).Value = entity.CustomField2;
            cmd.Parameters.Add("@CustomField3", MySqlDbType.VarChar).Value = entity.CustomField3;
            cmd.Parameters.Add("@CustomField4", MySqlDbType.VarChar).Value = entity.CustomField4;
            cmd.Parameters.Add("@CustomField5", MySqlDbType.VarChar).Value = entity.CustomField5;

            //Billing 1 <-> 0..1 BillingExternalInfo association
            if (cmd.ExecuteNonQuery() == 0)
            {
                //Billing 1 <-> 0..1 BillingExternalInfo association
                //If update failed try to insert
                cmd.CommandText = INSERT_COMMAND;
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Foreign Key BillingID({0}) was not found in database.", entity.BillingID));
                }
            }
        }

        public override BillingExternalInfo Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override BillingExternalInfo Load(DataRow row)
        {
            BillingExternalInfo res = new BillingExternalInfo();

            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt64(row["BillingID"]);
            if (!(row["InternalID"] is DBNull))
                res.InternalID = Convert.ToString(row["InternalID"]);
            if (!(row["CustomField1"] is DBNull))
                res.CustomField1 = Convert.ToString(row["CustomField1"]);
            if (!(row["CustomField2"] is DBNull))
                res.CustomField2 = Convert.ToString(row["CustomField2"]);
            if (!(row["CustomField3"] is DBNull))
                res.CustomField3 = Convert.ToString(row["CustomField3"]);
            if (!(row["CustomField4"] is DBNull))
                res.CustomField4 = Convert.ToString(row["CustomField4"]);
            if (!(row["CustomField5"] is DBNull))
                res.CustomField5 = Convert.ToString(row["CustomField5"]);

            return res;
        }
    }
}
