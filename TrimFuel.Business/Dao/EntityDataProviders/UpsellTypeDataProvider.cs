using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class UpsellTypeDataProvider : EntityDataProvider<UpsellType>
    {
        private const string INSERT_COMMAND = "INSERT INTO UpsellType(ProductCode, Price, Quantity, DisplayName, DropDown, Description, ProductID) VALUES(@ProductCode, @Price, @Quantity, @DisplayName, @DropDown, @Description, @ProductID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE UpsellType SET ProductCode=@ProductCode, Price=@Price, Quantity=@Quantity, DisplayName=@DisplayName, DropDown=@DropDown, Description=@Description, ProductID=@ProductID WHERE UpsellTypeID=@UpsellTypeID;";
        private const string SELECT_COMMAND = "SELECT * FROM UpsellType WHERE UpsellTypeID=@UpsellTypeID;";

        public override void Save(UpsellType entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.UpsellTypeID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@UpsellTypeID", MySqlDbType.Int32).Value = entity.UpsellTypeID;
            }

            cmd.Parameters.Add("@ProductCode", MySqlDbType.VarChar).Value = entity.ProductCode;
            cmd.Parameters.Add("@Price", MySqlDbType.Decimal).Value = entity.Price;
            cmd.Parameters.Add("@Quantity", MySqlDbType.Int32).Value = entity.Quantity;
            cmd.Parameters.Add("@DisplayName", MySqlDbType.VarChar).Value = entity.DisplayName;
            cmd.Parameters.Add("@DropDown", MySqlDbType.Bit).Value = entity.DropDown;
            cmd.Parameters.Add("@Description", MySqlDbType.VarChar).Value = entity.Description;
            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = entity.ProductID;
            
            if (entity.UpsellTypeID == null)
            {
                entity.UpsellTypeID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("UpsellType({0}) was not found in database.", entity.UpsellTypeID));
                }
            }
        }

        public override UpsellType Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@UpsellTypeID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override UpsellType Load(DataRow row)
        {
            UpsellType res = new UpsellType();

            if (!(row["UpsellTypeID"] is DBNull))
                res.UpsellTypeID = Convert.ToInt32(row["UpsellTypeID"]);
            if (!(row["ProductCode"] is DBNull))
                res.ProductCode = Convert.ToString(row["ProductCode"]);
            if (!(row["Price"] is DBNull))
                res.Price = Convert.ToDecimal(row["Price"]);
            if (!(row["Quantity"] is DBNull))
                res.Quantity = Convert.ToInt16(row["Quantity"]);
            if (!(row["DisplayName"] is DBNull))
                res.DisplayName = Convert.ToString(row["DisplayName"]);
            if (!(row["DropDown"] is DBNull))
                res.DropDown = Convert.ToBoolean(row["DropDown"]);
            if (!(row["Description"] is DBNull))
                res.Description = Convert.ToString(row["Description"]);
            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);

            return res;
        }
    }
}
