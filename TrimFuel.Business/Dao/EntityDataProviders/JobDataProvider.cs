using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class JobDataProvider : EntityDataProvider<Job>
    {
        private const string INSERT_COMMAND = "INSERT INTO Job(JobKey, StartDT, EndDT, Finished, ProgressPercent, CustomData, BackControlFlagStop) VALUES(@JobKey, @StartDT, @EndDT, @Finished, @ProgressPercent, @CustomData, @BackControlFlagStop); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE Job SET JobKey=@JobKey, StartDT=@StartDT, EndDT=@EndDT, Finished=@Finished, ProgressPercent=@ProgressPercent, CustomData=@CustomData, BackControlFlagStop=@BackControlFlagStop WHERE JobID=@JobID;";
        private const string SELECT_COMMAND = "SELECT * FROM Job WHERE JobID=@JobID;";

        public override void Save(Job entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.JobID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@JobID", MySqlDbType.Int32).Value = entity.JobID;
            }

            cmd.Parameters.Add("@JobKey", MySqlDbType.VarChar).Value = entity.JobKey;
            cmd.Parameters.Add("@StartDT", MySqlDbType.Timestamp).Value = entity.StartDT;
            cmd.Parameters.Add("@EndDT", MySqlDbType.Timestamp).Value = entity.EndDT;
            cmd.Parameters.Add("@Finished", MySqlDbType.Bit).Value = entity.Finished;
            cmd.Parameters.Add("@ProgressPercent", MySqlDbType.Decimal).Value = entity.ProgressPercent;
            cmd.Parameters.Add("@CustomData", MySqlDbType.VarChar).Value = entity.CustomData;
            cmd.Parameters.Add("@BackControlFlagStop", MySqlDbType.Bit).Value = entity.BackControlFlagStop;


            if (entity.JobID == null)
            {
                entity.JobID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Job({0}) was not found in database.", entity.JobID));
                }
            }
        }

        public override Job Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@JobID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Job Load(DataRow row)
        {
            Job res = new Job();

            if (!(row["JobID"] is DBNull))
                res.JobID = Convert.ToInt32(row["JobID"]);
            if (!(row["JobKey"] is DBNull))
                res.JobKey = Convert.ToString(row["JobKey"]);
            if (!(row["StartDT"] is DBNull))
                res.StartDT = Convert.ToDateTime(row["StartDT"]);
            if (!(row["EndDT"] is DBNull))
                res.EndDT = Convert.ToDateTime(row["EndDT"]);
            if (!(row["Finished"] is DBNull))
                res.Finished = Convert.ToBoolean(row["Finished"]);
            if (!(row["ProgressPercent"] is DBNull))
                res.ProgressPercent = Convert.ToDecimal(row["ProgressPercent"]);
            if (!(row["CustomData"] is DBNull))
                res.CustomData = Convert.ToString(row["CustomData"]);
            if (!(row["BackControlFlagStop"] is DBNull))
                res.BackControlFlagStop = Convert.ToBoolean(row["BackControlFlagStop"]);

            return res;
        }
    }
}
