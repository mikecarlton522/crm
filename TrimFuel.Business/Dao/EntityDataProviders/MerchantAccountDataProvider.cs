using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class MerchantAccountDataProvider : EntityDataProvider<MerchantAccount>
    {
        private const string SELECT_COMMAND = "SELECT * FROM MerchantAccount WHERE MerchantAccountID=@MerchantAccountID;";

        public override void Save(MerchantAccount entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override MerchantAccount Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@MerchantAccountID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override MerchantAccount Load(DataRow row)
        {
            MerchantAccount res = new MerchantAccount();

            if (!(row["MerchantAccountID"] is DBNull))
                res.MerchantAccountID = Convert.ToInt32(row["MerchantAccountID"]);
            if (!(row["Provider"] is DBNull))
                res.Provider = Convert.ToString(row["Provider"]);
            if (!(row["FriendlyName"] is DBNull))
                res.FriendlyName = Convert.ToString(row["FriendlyName"]);
            if (!(row["AccountNumber"] is DBNull))
                res.AccountNumber = Convert.ToString(row["AccountNumber"]);
            if (!(row["MerchantID"] is DBNull))
                res.MerchantID = Convert.ToString(row["MerchantID"]);
            if (!(row["MerchantPassword"] is DBNull))
                res.MerchantPassword = Convert.ToString(row["MerchantPassword"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["Active"] is DBNull))
                res.Active = Convert.ToBoolean(row["Active"]);
            if (!(row["DailyCap"] is DBNull))
                res.DailyCap = Convert.ToDecimal(row["DailyCap"]);

            return res;
        }
    }
}
