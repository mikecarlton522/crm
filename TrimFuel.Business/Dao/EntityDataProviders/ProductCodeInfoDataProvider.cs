using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ProductCodeInfoDataProvider : EntityDataProvider<ProductCodeInfo>
    {
        private ProductCodeDataProvider productCodeDataProvider = new ProductCodeDataProvider();

        private const string INSERT_COMMAND = "INSERT INTO ProductCodeInfo(ProductCodeID, Description, SalesContext, Photo, RetailPrice, Title, FeaturedProduct, LargePhoto) VALUES(@ProductCodeID, @Description, @SalesContext, @Photo, @RetailPrice, @Title, @FeaturedProduct, @LargePhoto);";
        private const string UPDATE_COMMAND = "UPDATE ProductCodeInfo SET Description=@Description, SalesContext=@SalesContext, Photo=@Photo, RetailPrice=@RetailPrice, Title=@Title, FeaturedProduct=@FeaturedProduct, LargePhoto=@LargePhoto WHERE ProductCodeID=@ProductCodeID;";
        private const string SELECT_COMMAND = "SELECT * FROM ProductCodeInfo pi INNER JOIN ProductCode p on pi.ProductCodeID=p.ProductCodeID WHERE pi.ProductCodeID=@ProductCodeID;";

        public override void Save(ProductCodeInfo entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            var oldItem = Load(entity.ProductCodeID, cmdCreater);

            if (oldItem == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
            }

            productCodeDataProvider.Save(entity, cmdCreater);

            cmd.Parameters.Add("@ProductCodeID", MySqlDbType.Int32).Value = entity.ProductCodeID;
            cmd.Parameters.Add("@Description", MySqlDbType.VarChar).Value = entity.Description;
            cmd.Parameters.Add("@SalesContext", MySqlDbType.VarChar).Value = entity.SalesContext;
            cmd.Parameters.Add("@Photo", MySqlDbType.VarChar).Value = entity.Photo;
            cmd.Parameters.Add("@RetailPrice", MySqlDbType.Decimal).Value = entity.RetailPrice;
            cmd.Parameters.Add("@Title", MySqlDbType.VarChar).Value = entity.Title;
            cmd.Parameters.Add("@FeaturedProduct", MySqlDbType.Bit).Value = entity.FeaturedProduct;
            cmd.Parameters.Add("@LargePhoto", MySqlDbType.VarChar).Value = entity.LargePhoto;

            if (cmd.ExecuteNonQuery() == 0)
            {
                throw new Exception(string.Format("ProductCodeInfo({0}) was not found in database.", entity.ProductCodeID));
            }
        }

        public override ProductCodeInfo Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ProductCodeID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ProductCodeInfo Load(System.Data.DataRow row)
        {
            ProductCodeInfo res = new ProductCodeInfo();
            ProductCode productCode = (new ProductCodeDataProvider()).Load(row);
            res.FillFromProductCode(productCode);

            if (!(row["Description"] is DBNull))
                res.Description = Convert.ToString(row["Description"]);
            if (!(row["FeaturedProduct"] is DBNull))
                res.FeaturedProduct = Convert.ToBoolean(row["FeaturedProduct"]);
            if (!(row["Photo"] is DBNull))
                res.Photo = Convert.ToString(row["Photo"]);
            if (!(row["LargePhoto"] is DBNull))
                res.LargePhoto = Convert.ToString(row["LargePhoto"]);
            if (!(row["RetailPrice"] is DBNull))
                res.RetailPrice = Convert.ToDecimal(row["RetailPrice"]);
            if (!(row["SalesContext"] is DBNull))
                res.SalesContext = Convert.ToString(row["SalesContext"]);
            if (!(row["Title"] is DBNull))
                res.Title = Convert.ToString(row["Title"]);
         
            return res;
        }
    }
}
