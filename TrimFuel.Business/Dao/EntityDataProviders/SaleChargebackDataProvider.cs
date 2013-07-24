using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class SaleChargebackDataProvider : EntityDataProvider<SaleChargeback>
    {
        private const string INSERT_COMMAND = "INSERT INTO SaleChargeback(SaleID, BillingID, ChargebackStatusTID, ChargebackReasonCodeID, CaseNumber, ARN, CreateDT, PostDT, DisputeSentDT) VALUES(@SaleID, @BillingID, @ChargebackStatusTID, @ChargebackReasonCodeID, @CaseNumber, @ARN, @CreateDT, @PostDT, @DisputeSentDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE SaleChargeback SET SaleID=@SaleID, BillingID=@BillingID, ChargebackStatusTID=@ChargebackStatusTID, ChargebackReasonCodeID=@ChargebackReasonCodeID, CaseNumber=@CaseNumber, ARN=@ARN, CreateDT=@CreateDT, PostDT=@PostDT, DisputeSentDT=@DisputeSentDT WHERE SaleChargebackID=@SaleChargebackID;";
        private const string SELECT_COMMAND = "SELECT * FROM SaleChargeback WHERE SaleChargebackID=@SaleChargebackID;";

        public override void Save(SaleChargeback entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.SaleChargebackID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@SaleChargebackID", MySqlDbType.Int32).Value = entity.SaleChargebackID;
            }

            cmd.Parameters.Add("@SaleID", MySqlDbType.Int32).Value = entity.SaleID;
            cmd.Parameters.Add("@BillingID", MySqlDbType.Int32).Value = entity.BillingID;
            cmd.Parameters.Add("@ChargebackStatusTID", MySqlDbType.Int32).Value = entity.ChargebackStatusTID;
            cmd.Parameters.Add("@ChargebackReasonCodeID", MySqlDbType.Int32).Value = entity.ChargebackReasonCodeID;
            cmd.Parameters.Add("@CaseNumber", MySqlDbType.VarChar).Value = entity.CaseNumber;
            cmd.Parameters.Add("@ARN", MySqlDbType.VarChar).Value = entity.ARN;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Date).Value = entity.CreateDT;
            cmd.Parameters.Add("@PostDT", MySqlDbType.Timestamp).Value = entity.PostDT;
            cmd.Parameters.Add("@DisputeSentDT", MySqlDbType.Timestamp).Value = entity.DisputeSentDT;


            if (entity.SaleChargebackID == null)
            {
                entity.SaleChargebackID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("SaleChargeback({0}) was not found in database.", entity.SaleChargebackID));
                }
            }
        }

        public override SaleChargeback Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@SaleChargebackID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override SaleChargeback Load(DataRow row)
        {
            SaleChargeback res = new SaleChargeback();

            if (!(row["SaleChargebackID"] is DBNull))
                res.SaleChargebackID = Convert.ToInt32(row["SaleChargebackID"]);
            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt32(row["SaleID"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt32(row["BillingID"]);
            if (!(row["ChargebackStatusTID"] is DBNull))
                res.ChargebackStatusTID = Convert.ToInt32(row["ChargebackStatusTID"]);
            if (!(row["ChargebackReasonCodeID"] is DBNull))
                res.ChargebackReasonCodeID = Convert.ToInt32(row["ChargebackReasonCodeID"]);
            if (!(row["CaseNumber"] is DBNull))
                res.CaseNumber = Convert.ToString(row["CaseNumber"]);
            if (!(row["ARN"] is DBNull))
                res.ARN = Convert.ToString(row["ARN"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["PostDT"] is DBNull))
                res.PostDT = Convert.ToDateTime(row["PostDT"]);
            if (!(row["DisputeSentDT"] is DBNull))
                res.DisputeSentDT = Convert.ToDateTime(row["DisputeSentDT"]);

            return res;
        }
    }
}
