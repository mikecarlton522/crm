using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ExtraTrialShipTypeDataProvider : EntityDataProvider<ExtraTrialShipType>
    {
        private const string SELECT_COMMAND = "SELECT * FROM ExtraTrialShipType WHERE ID=@ExtraTrialShipTypeID;";

        public override void Save(ExtraTrialShipType entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override ExtraTrialShipType Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ExtraTrialShipTypeID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ExtraTrialShipType Load(DataRow row)
        {
            ExtraTrialShipType res = new ExtraTrialShipType();

            if (!(row["ID"] is DBNull))
                res.ExtraTrialShipTypeID = Convert.ToInt32(row["ID"]);
            if (!(row["ProductCode"] is DBNull))
                res.ProductCode = Convert.ToString(row["ProductCode"]);
            if (!(row["DisplayName"] is DBNull))
                res.DisplayName = Convert.ToString(row["DisplayName"]);
            if (!(row["Active"] is DBNull))
                res.Active = Convert.ToBoolean(row["Active"]);

            return res;
        }
    }
}
