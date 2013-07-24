using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ChargeHistoryCardDataProvider : EntityDataProvider<ChargeHistoryCard>
    {
        private const string INSERT_COMMAND = "INSERT INTO ChargeHistoryCard(ChargeHistoryID, CreditCardLeft6, CreditCardRight4, PaymentTypeID, ExpMonth, ExpYear) VALUES(@ChargeHistoryID, @CreditCardLeft6, @CreditCardRight4, @PaymentTypeID, @ExpMonth, @ExpYear); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE ChargeHistoryCard SET CreditCardLeft6=@CreditCardLeft6, CreditCardRight4=@CreditCardRight4, PaymentTypeID=@PaymentTypeID, ExpMonth=@ExpMonth, ExpYear=@ExpYear WHERE ChargeHistoryID=@ChargeHistoryID;";
        private const string SELECT_COMMAND = "SELECT * FROM ChargeHistoryCard WHERE ChargeHistoryID=@ChargeHistoryID;";

        public override void Save(ChargeHistoryCard entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            var existed = Load(entity.ChargeHistoryID, cmdCreater);
            if (existed == null)
                cmd.CommandText = INSERT_COMMAND;
            else
                cmd.CommandText = UPDATE_COMMAND;

            cmd.Parameters.Add("@ChargeHistoryID", MySqlDbType.Int64).Value = entity.ChargeHistoryID;
            cmd.Parameters.Add("@CreditCardLeft6", MySqlDbType.VarChar).Value = entity.CreditCardLeft6;
            cmd.Parameters.Add("@CreditCardRight4", MySqlDbType.VarChar).Value = entity.CreditCardRight4;
            cmd.Parameters.Add("@PaymentTypeID", MySqlDbType.Int32).Value = entity.PaymentTypeID;
            cmd.Parameters.Add("@ExpMonth", MySqlDbType.Int32).Value = entity.ExpMonth;
            cmd.Parameters.Add("@ExpYear", MySqlDbType.Int32).Value = entity.ExpYear;

            if (entity.ChargeHistoryID == null)
            {
                entity.ChargeHistoryID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("ChargeHistoryCard({0}) was not found in database.", entity.ChargeHistoryID));
                }
            }
        }

        public override ChargeHistoryCard Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ChargeHistoryID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ChargeHistoryCard Load(System.Data.DataRow row)
        {
            ChargeHistoryCard res = new ChargeHistoryCard();

            if (!(row["ChargeHistoryID"] is DBNull))
                res.ChargeHistoryID = Convert.ToInt64(row["ChargeHistoryID"]);
            if (!(row["CreditCardRight4"] is DBNull))
                res.CreditCardRight4 = Convert.ToString(row["CreditCardRight4"]);
            if (!(row["CreditCardLeft6"] is DBNull))
                res.CreditCardLeft6 = Convert.ToString(row["CreditCardLeft6"]);
            if (!(row["PaymentTypeID"] is DBNull))
                res.PaymentTypeID = Convert.ToInt32(row["PaymentTypeID"]);
            if (!(row["ExpMonth"] is DBNull))
                res.ExpMonth = Convert.ToInt32(row["ExpMonth"]);
            if (!(row["ExpYear"] is DBNull))
                res.ExpYear = Convert.ToInt32(row["ExpYear"]);

            return res;
        }
    }
}
