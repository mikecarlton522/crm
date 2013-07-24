using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class FraudScoreDataProvider : EntityDataProvider<FraudScore>
    {
        private const string INSERT_COMMAND = "INSERT INTO FraudScore(SaleID, Request, Response, Error, FraudScore, CreateDT, BillingID, ResponseBinName) VALUES(@SaleID, @Request, @Response, @Error, @FraudScore, @CreateDT, @BillingID, @ResponseBinName); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE FraudScore SET SaleID=@SaleID, Request=@Request, Response=@Response, Error=@Error, FraudScore=@FraudScore, CreateDT=@CreateDT, BillingID=@BillingID, ResponseBinName=@ResponseBinName WHERE ID=@FraudScoreID;";
        private const string SELECT_COMMAND = "SELECT * FROM FraudScore WHERE ID=@FraudScoreID;";

        public override void Save(FraudScore entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.FraudScoreID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@FraudScoreID", MySqlDbType.Int32).Value = entity.FraudScoreID;
            }

            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@Request", MySqlDbType.VarChar).Value = entity.Request;
            cmd.Parameters.Add("@Response", MySqlDbType.VarChar).Value = entity.Response;
            cmd.Parameters.Add("@Error", MySqlDbType.Bit).Value = entity.Error;
            cmd.Parameters.Add("@FraudScore", MySqlDbType.Int16).Value = entity.FraudScore_;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;
            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;
            cmd.Parameters.Add("@ResponseBinName", MySqlDbType.VarChar).Value = entity.ResponseBinName;

            if (entity.FraudScoreID == null)
            {
                entity.FraudScoreID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("FraudScore({0}) was not found in database.", entity.FraudScoreID));
                }
            }
        }

        public override FraudScore Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@FraudScoreID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override FraudScore Load(DataRow row)
        {
            FraudScore res = new FraudScore();

            if (!(row["ID"] is DBNull))
                res.FraudScoreID = Convert.ToInt32(row["ID"]);
            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["Request"] is DBNull))
                res.Request = Convert.ToString(row["Request"]);
            if (!(row["Response"] is DBNull))
                res.Response = Convert.ToString(row["Response"]);
            if (!(row["Error"] is DBNull))
                res.Error = Convert.ToBoolean(row["Error"]);
            if (!(row["FraudScore"] is DBNull))
                res.FraudScore_ = Convert.ToInt16(row["FraudScore"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt64(row["BillingID"]);
            if (!(row["ResponseBinName"] is DBNull))
                res.ResponseBinName = Convert.ToString(row["ResponseBinName"]);

            return res;
        }
    }
}
