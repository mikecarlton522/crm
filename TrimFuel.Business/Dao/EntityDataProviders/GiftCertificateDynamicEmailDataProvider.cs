using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class GiftCertificateDynamicEmailDataProvider : EntityDataProvider<GiftCertificateDynamicEmail>
    {
        private DynamicEmailDataProvider dynamicEmailDataProvider = new DynamicEmailDataProvider();

        private const string INSERT_COMMAND = "INSERT INTO GiftCertificateDynamicEmail(DynamicEmailID, StoreID) VALUES(@DynamicEmailID, @StoreID);";
        private const string UPDATE_COMMAND = "UPDATE GiftCertificateDynamicEmail SET StoreID=@StoreID WHERE DynamicEmailID=@DynamicEmailID;";
        private const string SELECT_COMMAND = "SELECT * FROM DynamicEmail de INNER JOIN GiftCertificateDynamicEmail gsde ON de.DynamicEmailID = gsde.DynamicEmailID WHERE de.DynamicEmailID = @DynamicEmailID;";

        public override void Save(GiftCertificateDynamicEmail entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.DynamicEmailID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
            }

            dynamicEmailDataProvider.Save(entity, cmdCreater);

            cmd.Parameters.Add("@DynamicEmailID", MySqlDbType.Int32).Value = entity.DynamicEmailID;
            cmd.Parameters.Add("@StoreID", MySqlDbType.Int32).Value = entity.StoreID;

            if (cmd.ExecuteNonQuery() == 0)
            {
                throw new Exception(string.Format("GiftCertificateDynamicEmail({0}) was not found in database.", entity.DynamicEmailID));
            }
        }

        public override GiftCertificateDynamicEmail Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@DynamicEmailID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override GiftCertificateDynamicEmail Load(DataRow row)
        {
            DynamicEmail dynamicEmail = (new DynamicEmailDataProvider()).Load(row);

            GiftCertificateDynamicEmail res = new GiftCertificateDynamicEmail();
            res.FillFromDynamicEmail(dynamicEmail);

            if (!(row["StoreID"] is DBNull))
                res.StoreID = Convert.ToInt32(row["StoreID"]);

            return res;
        }
    }
}
