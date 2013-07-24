using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ProductWikiDataProvider : EntityDataProvider<ProductWiki>
    {
        private const string INSERT_COMMAND = "INSERT INTO ProductWiki(ProductID, Path) VALUES(@ProductID, @Path);";
        private const string UPDATE_COMMAND = "UPDATE ProductWiki SET Path=@Path WHERE ProductID=@ProductID;";
        private const string SELECT_COMMAND = "SELECT * FROM ProductWiki WHERE ProductID=@ProductID;";

        public override void Save(ProductWiki entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            var esistsEntity = Load(entity.ProductID, cmdCreater);

            if (esistsEntity == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
            }

            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = entity.ProductID;
            cmd.Parameters.Add("@Path", MySqlDbType.VarChar).Value = entity.Path;

            if (cmd.ExecuteNonQuery() == 0)
            {
                throw new Exception(string.Format("ProductWiki({0}) was not found in database.", entity.ProductID));
            }
        }

        public override ProductWiki Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ProductWiki Load(System.Data.DataRow row)
        {
            ProductWiki res = new ProductWiki();

            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);
            if (!(row["Path"] is DBNull))
                res.Path = Convert.ToString(row["Path"]);

            return res;
        }
    }
}
