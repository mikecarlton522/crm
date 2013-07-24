using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using TrimFuel.Model;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    class MagentoProductCategoryDataProvider : EntityDataProvider<MagentoProductCategory>
    {
        private const string INSERT_COMMAND = "INSERT INTO `MagentoProductCategory`(InventoryID, CategoryName) VALUES(@InventoryID, @CategoryName);";
        private const string UPDATE_COMMAND = "UPDATE `MagentoProductCategory` SET InventoryID=@InventoryID, CategoryName=@CategoryName;";
        private const string SELECT_COMMAND = "SELECT * FROM `MagentoProductCategory` WHERE InventoryID=@InventoryID;";

        public override void Save(MagentoProductCategory entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            cmd.CommandText = SELECT_COMMAND;
            cmd.Parameters.Add("@InventoryID", MySqlDbType.Int32).Value = entity.InventoryID;

            cmd.CommandText = Load(cmd) != null ? UPDATE_COMMAND : INSERT_COMMAND;

            
            cmd.Parameters.Add("@CategoryName", MySqlDbType.VarChar).Value = entity.CategoryName;

            if (entity.InventoryID == null)
            {
                cmd.ExecuteScalar();
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("InventoryID ({0}) was not found in database.", entity.InventoryID));
                }
            }
        }

        public override MagentoProductCategory Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            return Load(cmd).FirstOrDefault();
        }

        public override MagentoProductCategory Load(DataRow row)
        {
            MagentoProductCategory res = new MagentoProductCategory();

            if (!(row["InventoryID"] is DBNull))
                res.InventoryID = Convert.ToInt32(row["InventoryID"]);
            if (!(row["CategoryName"] is DBNull))
                res.CategoryName = Convert.ToString(row["CategoryName"]);

            return res;
        }
    }
}
