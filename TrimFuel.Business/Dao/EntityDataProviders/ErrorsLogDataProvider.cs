using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ErrorsLogDataProvider : EntityDataProvider<ErrorsLog>
    {
        private const string SELECT_COMMAND = "SELECT * FROM ErrorsLog WHERE ErrorsLogID=@ErrorsLogID;";

        public override void Save(ErrorsLog entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override ErrorsLog Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ErrorsLogID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ErrorsLog Load(DataRow row)
        {
            ErrorsLog res = new ErrorsLog();

            if (!(row["ErrorsLogID"] is DBNull))
                res.ErrorsLogID = Convert.ToInt32(row["ErrorsLogID"]);
            if (!(row["Application"] is DBNull))
                res.Application = Convert.ToString(row["Application"]);
            if (!(row["ApplicationID"] is DBNull))
                res.ApplicationID = Convert.ToString(row["ApplicationID"]);
            if (!(row["ErrorDate"] is DBNull))
                res.ErrorDate = Convert.ToDateTime(row["ErrorDate"]);
            if (!(row["ClassName"] is DBNull))
                res.ClassName = Convert.ToString(row["ClassName"]);
            if (!(row["BriefErrorText"] is DBNull))
                res.BriefErrorText = Convert.ToString(row["BriefErrorText"]);
            if (!(row["ErrorText"] is DBNull))
                res.ErrorText = Convert.ToString(row["ErrorText"]);
            if (!(row["Category"] is DBNull))
                res.Category = Convert.ToString(row["Category"]);

            return res;
        }
    }
}
