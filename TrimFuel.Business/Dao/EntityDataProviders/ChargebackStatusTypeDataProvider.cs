using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ChargebackStatusTypeDataProvider : EntityDataProvider<ChargebackStatusType>
    {
        private const string SELECT_COMMAND = "SELECT * FROM ChargebackStatusType WHERE ChargebackStatusTypeID = @ChargebackStatusTypeID;";

        public override void Save(ChargebackStatusType entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override ChargebackStatusType Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ChargebackStatusTypeID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ChargebackStatusType Load(DataRow row)
        {
            ChargebackStatusType res = new ChargebackStatusType();

            if (!(row["ChargebackStatusTypeID"] is DBNull))
                res.ChargebackStatusTypeID = Convert.ToInt32(row["ChargebackStatusTypeID"]);
            if (!(row["DisplayName"] is DBNull))
                res.DisplayName = Convert.ToString(row["DisplayName"]);

            return res;
        }
    }
}
