using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class CurrencyDataProvider : EntityDataProvider<Currency>
    {
        private const string SELECT_COMMAND = "SELECT * FROM Currency WHERE CurrencyID=@CurrencyID;";

        public override void Save(Currency entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override Currency Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@CurrencyID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Currency Load(DataRow row)
        {
            Currency res = new Currency();

            if (!(row["CurrencyID"] is DBNull))
                res.CurrencyID = Convert.ToInt32(row["CurrencyID"]);
            if (!(row["Rate"] is DBNull))
                res.Rate = Convert.ToDecimal(row["Rate"]);
            if (!(row["CurrencyName"] is DBNull))
                res.CurrencyName = Convert.ToString(row["CurrencyName"]);
            if (!(row["HtmlSymbol"] is DBNull))
                res.HtmlSymbol = Convert.ToString(row["HtmlSymbol"]);

            return res;
        }
    }
}
