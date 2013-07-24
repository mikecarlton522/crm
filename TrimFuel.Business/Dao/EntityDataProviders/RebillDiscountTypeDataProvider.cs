using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class RebillDiscountTypeDataProvider : EntityDataProvider<RebillDiscountType>
    {
        private const string INSERT_COMMAND = "INSERT INTO DiscountType(Name) VALUES(@Name); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE DiscountType SET Name=@Name WHERE DiscountTypeID=@DiscountTypeID;";
        private const string SELECT_COMMAND = "SELECT * FROM DiscountType WHERE DiscountTypeID=@DiscountTypeID;";

        public override void Save(RebillDiscountType entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.DiscountTypeID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@DiscountTypeID", MySqlDbType.Int32).Value = entity.DiscountTypeID;
            }

            cmd.Parameters.Add("@Name", MySqlDbType.VarChar).Value = entity.Name;


            if (entity.DiscountTypeID == null)
            {
                entity.DiscountTypeID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("DiscountType({0}) was not found in database.", entity.DiscountTypeID));
                }
            }
        }

        public override RebillDiscountType Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@DiscountTypeID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override RebillDiscountType Load(DataRow row)
        {
            RebillDiscountType res = new RebillDiscountType();

            if (!(row["DiscountTypeID"] is DBNull))
                res.DiscountTypeID = Convert.ToInt32(row["DiscountTypeID"]);
            if (!(row["Name"] is DBNull))
                res.Name = Convert.ToString(row["Name"]);

            return res;
        }
    }
}