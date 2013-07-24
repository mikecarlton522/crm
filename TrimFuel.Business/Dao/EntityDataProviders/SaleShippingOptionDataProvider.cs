using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class SaleShippingOptionDataProvider : EntityDataProvider<SaleShippingOption>
    {
        private const string INSERT_COMMAND = "INSERT INTO SaleShippingOption(SaleID, ShippingOptionID) VALUES(@SaleID, @ShippingOptionID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE SaleShippingOption SET SaleID=@SaleID, ShippingOptionID=@ShippingOptionID WHERE SaleID=@IDSaleID;";
        private const string SELECT_COMMAND = "SELECT * FROM SaleShippingOption WHERE SaleID=@IDSaleID;";

        public override void Save(SaleShippingOption entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.SaleShippingOptionID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@IDSaleID", MySqlDbType.Int64).Value = entity.SaleShippingOptionID.Value.SaleID;
            }

            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@ShippingOptionID", MySqlDbType.Int32).Value = entity.ShippingOptionID;


            if (entity.SaleShippingOptionID == null)
            {
                cmd.ExecuteNonQuery();
                entity.SaleShippingOptionID = new SaleShippingOption.ID() { SaleID = entity.SaleID.Value };
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("SaleShippingOption({0}) was not found in database.", entity.SaleShippingOptionID));
                }
                else
                {
                    entity.SaleShippingOptionID = new SaleShippingOption.ID() { SaleID = entity.SaleID.Value };
                }
            }
        }

        public override SaleShippingOption Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@IDSaleID", MySqlDbType.Int64).Value = ((SaleShippingOption.ID?)key).Value.SaleID;

            return Load(cmd).FirstOrDefault();
        }

        public override SaleShippingOption Load(DataRow row)
        {
            SaleShippingOption res = new SaleShippingOption();

            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["ShippingOptionID"] is DBNull))
                res.ShippingOptionID = Convert.ToInt32(row["ShippingOptionID"]);
            if (res.SaleID != null)
            {
                res.SaleShippingOptionID = new SaleShippingOption.ID() { SaleID = res.SaleID.Value };
            }

            return res;
        }
    }
}
