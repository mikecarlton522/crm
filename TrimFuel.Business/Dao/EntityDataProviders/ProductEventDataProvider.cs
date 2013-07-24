using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ProductEventDataProvider : EntityDataProvider<ProductEvent>
    {
        private const string INSERT_COMMAND = "INSERT INTO ProductEvents(ProductID, EventTypeID, URL) VALUES(@ProductID, @EventTypeID, @URL); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE ProductEvents SET ProductID=@ProductID, EventTypeID=@EventTypeID, URL=@URL WHERE ProductEventID=@ProductEventID;";
        private const string SELECT_COMMAND = "SELECT * FROM ProductEvents WHERE ProductEventID=@ProductEventID;";

        public override void Save(ProductEvent entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            int? id = null;
            if (entity.ProductEventID == null)
                cmd.CommandText = INSERT_COMMAND;
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ProductEventID", MySqlDbType.Int32).Value = entity.ProductEventID;
            }

            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = entity.ProductID;
            cmd.Parameters.Add("@EventTypeID", MySqlDbType.Int32).Value = entity.EventTypeID;
            cmd.Parameters.Add("@URL", MySqlDbType.VarChar).Value = entity.URl;

            if (entity.ProductEventID == null)
            {
                entity.ProductEventID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("ProductEvent ({0}) was not found in database.", entity.ProductEventID));
                }
            }
        }

        public override ProductEvent Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ProductEventID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ProductEvent Load(System.Data.DataRow row)
        {
            ProductEvent res = new ProductEvent();

            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);
            if (!(row["EventTypeID"] is DBNull))
                res.EventTypeID = Convert.ToInt32(row["EventTypeID"]);
            if (!(row["ProductEventID"] is DBNull))
                res.ProductEventID = Convert.ToInt32(row["ProductEventID"]);
            if (!(row["URL"] is DBNull))
                res.URl = Convert.ToString(row["URL"]);

            return res;
        }
    }
}
