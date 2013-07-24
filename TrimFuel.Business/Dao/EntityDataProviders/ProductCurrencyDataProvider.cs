using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ProductCurrencyDataProvider : EntityDataProvider<ProductCurrency>
    {
        string SELECT_COMMAND = "SELECT * FROM ProductCurrency WHERE CurrencyID=@CurrencyID AND ProductID=@ProductID;";
        private const string INSERT_COMMAND = "INSERT INTO ProductCurrency(ProductID, CurrencyID) VALUES(@ProductID, @CurrencyID);";
        private const string UPDATE_COMMAND = "UPDATE ProductCurrency SET ProductID=@ProductID, CurrencyID=@CurrencyID WHERE CurrencyID=@ID_CurrencyID AND ProductID=@ID_ProductID;";

        public override void Save(ProductCurrency entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.ProductCurrencyID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ID_ProductID", MySqlDbType.Int32).Value = entity.ProductCurrencyID.Value.ProductID;
                cmd.Parameters.Add("@ID_CurrencyID", MySqlDbType.Int32).Value = entity.ProductCurrencyID.Value.CurrencyID;
            }

            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = entity.ProductID;
            cmd.Parameters.Add("@CurrencyID", MySqlDbType.Int32).Value = entity.CurrencyID;

            if (entity.ProductCurrencyID == null)
            {
                cmd.ExecuteNonQuery();
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("ProductCurrency ({0}) was not found in database.", entity.ProductID));
                }
            }
            entity.ProductCurrencyID = new ProductCurrency.ID() { CurrencyID = entity.CurrencyID.Value, ProductID = entity.ProductID };
        }

        public override ProductCurrency Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = ((ProductCurrency.ID)key).ProductID;
            cmd.Parameters.Add("@CurrencyID", MySqlDbType.Int32).Value = ((ProductCurrency.ID)key).CurrencyID;

            return Load(cmd).FirstOrDefault();
        }

        public override ProductCurrency Load(System.Data.DataRow row)
        {
            ProductCurrency res = new ProductCurrency();

            if (!(row["CurrencyID"] is DBNull))
                res.CurrencyID = Convert.ToInt32(row["CurrencyID"]);
            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);

            res.ProductCurrencyID = new ProductCurrency.ID() { CurrencyID = res.CurrencyID.Value, ProductID = res.ProductID };

            return res;
        }
    }
}
