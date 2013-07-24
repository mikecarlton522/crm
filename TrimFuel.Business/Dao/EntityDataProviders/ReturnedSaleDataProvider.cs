using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ReturnedSaleDataProvider : EntityDataProvider<ReturnedSale>
    {
        private const string INSERT_COMMAND = "INSERT INTO ReturnedSale(SaleID, ReturnDate, Reason) VALUES(@SaleID, @ReturnDate, @Reason);";
        private const string UPDATE_COMMAND = "UPDATE ReturnedSale SET ReturnDate=@ReturnDate, Reason=@Reason WHERE SaleID=@SaleID;";
        private const string SELECT_COMMAND = "SELECT * FROM ReturnedSale WHERE SaleID=@SaleID;";

        /// <summary>
        /// always add new row!!!!!!!!!!!
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cmdCreater"></param>
        public override void Save(ReturnedSale entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            cmd.CommandText = INSERT_COMMAND;

            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@ReturnDate", MySqlDbType.Timestamp).Value = entity.ReturnDate;
            cmd.Parameters.Add("@Reason", MySqlDbType.VarChar).Value = entity.Reason;

            cmd.ExecuteNonQuery();
        }

        public override ReturnedSale Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ReturnedSale Load(System.Data.DataRow row)
        {
            ReturnedSale res = new ReturnedSale();

            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["Reason"] is DBNull))
                res.Reason = Convert.ToString(row["Reason"]);
            if (!(row["ReturnDate"] is DBNull))
                res.ReturnDate = Convert.ToDateTime(row["ReturnDate"]);

            return res;
        }
    }
}
