using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class DailyBillingResultsDataProvider : EntityDataProvider<DailyBillingResults>
    {
        private const string INSERT_COMMAND = "INSERT INTO DailyBillingResults(ChargeDT, SubscriptionID, ReattemptSuccessCount, RebillFailCount, RebillSuccessCount, ReattemptFailCount, Amount, Affiliate, MID) VALUES(@ChargeDT, @SubscriptionID, @ReattemptSuccessCount, @RebillFailCount, @RebillSuccessCount, @ReattemptFailCount, @Amount, @Affiliate, @MID);";
        private const string UPDATE_COMMAND = "UPDATE DailyBillingResults SET ChargeDT=@ChargeDT, SubscriptionID=@SubscriptionID, ReattemptSuccessCount=@ReattemptSuccessCount, RebillFailCount=@RebillFailCount, RebillSuccessCount=@RebillSuccessCount, ReattemptFailCount=@ReattemptFailCount, Amount=@Amount, Affiliate=@Affiliate, MID=@MID WHERE DailyBillingResultsID=@DailyBillingResultsID;";
        private const string SELECT_COMMAND = "SELECT * FROM DailyBillingResults WHERE DailyBillingResultsID = @DailyBillingResultsID;";

        public override void Save(DailyBillingResults entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.DailyBillingResultsID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@DailyBillingResultsID", MySqlDbType.Int32).Value = entity.DailyBillingResultsID;
            }

            cmd.Parameters.Add("@Affiliate", MySqlDbType.VarChar).Value = entity.Affiliate;
            cmd.Parameters.Add("@Amount", MySqlDbType.Decimal).Value = entity.Amount;
            cmd.Parameters.Add("@ChargeDT", MySqlDbType.DateTime).Value = entity.ChargeDT;
            cmd.Parameters.Add("@MID", MySqlDbType.VarChar).Value = entity.MID;
            cmd.Parameters.Add("@ReattemptFailCount", MySqlDbType.Int32).Value = entity.ReattemptFailCount;
            cmd.Parameters.Add("@ReattemptSuccessCount", MySqlDbType.Int32).Value = entity.ReattemptSuccessCount;
            cmd.Parameters.Add("@RebillFailCount", MySqlDbType.Int32).Value = entity.RebillFailCount;
            cmd.Parameters.Add("@RebillSuccessCount", MySqlDbType.Int32).Value = entity.RebillSuccessCount;
            cmd.Parameters.Add("@SubscriptionID", MySqlDbType.Int32).Value = entity.SubscriptionID;

            if (entity.DailyBillingResultsID == null)
            {
                entity.DailyBillingResultsID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("DailyBillingResults({0}) was not found in database.", entity.DailyBillingResultsID));
                }
            }
        }

        public override DailyBillingResults Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@DailyBillingResultsID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override DailyBillingResults Load(System.Data.DataRow row)
        {
            DailyBillingResults res = new DailyBillingResults();

            if (!(row["Affiliate"] is DBNull))
                res.Affiliate = Convert.ToString(row["Affiliate"]);
            if (!(row["Amount"] is DBNull))
                res.Amount = Convert.ToDecimal(row["Amount"]);
            if (!(row["ChargeDT"] is DBNull))
                res.ChargeDT = Convert.ToDateTime(row["ChargeDT"]);
            if (!(row["DailyBillingResultsID"] is DBNull))
                res.DailyBillingResultsID = Convert.ToInt32(row["DailyBillingResultsID"]);
            if (!(row["MID"] is DBNull))
                res.MID = Convert.ToString(row["MID"]);
            if (!(row["ReattemptFailCount"] is DBNull))
                res.ReattemptFailCount = Convert.ToInt32(row["ReattemptFailCount"]);
            if (!(row["ReattemptSuccessCount"] is DBNull))
                res.ReattemptSuccessCount = Convert.ToInt32(row["ReattemptSuccessCount"]);
            if (!(row["RebillFailCount"] is DBNull))
                res.RebillFailCount = Convert.ToInt32(row["RebillFailCount"]);
            if (!(row["RebillSuccessCount"] is DBNull))
                res.RebillSuccessCount = Convert.ToInt32(row["RebillSuccessCount"]);
            if (!(row["SubscriptionID"] is DBNull))
                res.SubscriptionID = Convert.ToInt32(row["SubscriptionID"]);

            return res;
        }
    }
}
