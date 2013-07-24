using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class InvoiceDataProvider : EntityDataProvider<Invoice>
    {
        private const string INSERT_COMMAND = "INSERT INTO Invoice(Amount, AuthAmount, OrderID, CurrencyID, InvoiceStatus, CreateDT, ProcessDT) VALUES(@Amount, @AuthAmount, @OrderID, @CurrencyID, @InvoiceStatus, @CreateDT, @ProcessDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE Invoice SET Amount=@Amount, AuthAmount=@AuthAmount, OrderID=@OrderID, CurrencyID=@CurrencyID, InvoiceStatus=@InvoiceStatus, CreateDT=@CreateDT, ProcessDT=@ProcessDT WHERE InvoiceID=@InvoiceID;";
        private const string SELECT_COMMAND = "SELECT * FROM Invoice WHERE InvoiceID=@InvoiceID;";

        public override void Save(Invoice entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.InvoiceID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@InvoiceID", MySqlDbType.Int64).Value = entity.InvoiceID;
            }

            cmd.Parameters.Add("@Amount", MySqlDbType.Decimal).Value = entity.Amount;
            cmd.Parameters.Add("@AuthAmount", MySqlDbType.Decimal).Value = entity.AuthAmount;
            cmd.Parameters.Add("@OrderID", MySqlDbType.Int64).Value = entity.OrderID;
            cmd.Parameters.Add("@CurrencyID", MySqlDbType.Int32).Value = entity.CurrencyID;
            cmd.Parameters.Add("@InvoiceStatus", MySqlDbType.Int32).Value = entity.InvoiceStatus;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;
            cmd.Parameters.Add("@ProcessDT", MySqlDbType.Timestamp).Value = entity.ProcessDT;


            if (entity.InvoiceID == null)
            {
                entity.InvoiceID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Invoice({0}) was not found in database.", entity.InvoiceID));
                }
            }
        }

        public override Invoice Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@InvoiceID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Invoice Load(DataRow row)
        {
            Invoice res = new Invoice();

            if (!(row["InvoiceID"] is DBNull))
                res.InvoiceID = Convert.ToInt64(row["InvoiceID"]);
            if (!(row["Amount"] is DBNull))
                res.Amount = Convert.ToDecimal(row["Amount"]);
            if (!(row["AuthAmount"] is DBNull))
                res.AuthAmount = Convert.ToDecimal(row["AuthAmount"]);
            if (!(row["OrderID"] is DBNull))
                res.OrderID = Convert.ToInt64(row["OrderID"]);
            if (!(row["CurrencyID"] is DBNull))
                res.CurrencyID = Convert.ToInt32(row["CurrencyID"]);
            if (!(row["InvoiceStatus"] is DBNull))
                res.InvoiceStatus = Convert.ToInt32(row["InvoiceStatus"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["ProcessDT"] is DBNull))
                res.ProcessDT = Convert.ToDateTime(row["ProcessDT"]);

            return res;
        }
    }
}
