using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class RecurringPlanConstraintDataProvider : EntityDataProvider<RecurringPlanConstraint>
    {
        private const string INSERT_COMMAND = "INSERT INTO RecurringPlanConstraint(RecurringPlanCycleID, ChargeTypeID, Amount, ShippingAmount, TaxAmount) VALUES(@RecurringPlanCycleID, @ChargeTypeID, @Amount, @ShippingAmount, @TaxAmount);";
        private const string UPDATE_COMMAND = "UPDATE RecurringPlanConstraint SET RecurringPlanCycleID=@RecurringPlanCycleID, ChargeTypeID=@ChargeTypeID, Amount=@Amount, ShippingAmount=@ShippingAmount, TaxAmount=@TaxAmount WHERE RecurringPlanCycleID=@RecurringPlanConstraintID_RecurringPlanCycleID;";
        private const string SELECT_COMMAND = "SELECT * FROM RecurringPlanConstraint WHERE RecurringPlanCycleID=@RecurringPlanCycleID;";

        public override void Save(RecurringPlanConstraint entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.RecurringPlanConstraintID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@RecurringPlanConstraintID_RecurringPlanCycleID", MySqlDbType.Int32).Value = entity.RecurringPlanConstraintID.Value.RecurringPlanCycleID;
            }

            cmd.Parameters.Add("@RecurringPlanCycleID", MySqlDbType.Int32).Value = entity.RecurringPlanCycleID;
            cmd.Parameters.Add("@ChargeTypeID", MySqlDbType.Int32).Value = entity.ChargeTypeID;
            cmd.Parameters.Add("@Amount", MySqlDbType.Decimal).Value = entity.Amount;
            cmd.Parameters.Add("@ShippingAmount", MySqlDbType.Decimal).Value = entity.ShippingAmount;
            cmd.Parameters.Add("@TaxAmount", MySqlDbType.Decimal).Value = entity.TaxAmount;

            if (entity.RecurringPlanConstraintID == null)
            {
                cmd.ExecuteNonQuery();
                entity.RecurringPlanConstraintID = new RecurringPlanConstraint.ID() { RecurringPlanCycleID = entity.RecurringPlanCycleID };
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("RecurringPlanConstraint({0}) was not found in database.", entity.RecurringPlanConstraintID.Value.RecurringPlanCycleID));
                }
                else
                {
                    entity.RecurringPlanConstraintID = new RecurringPlanConstraint.ID() { RecurringPlanCycleID = entity.RecurringPlanCycleID };
                }
            }
        }

        public override RecurringPlanConstraint Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@RecurringPlanCycleID", MySqlDbType.Int32).Value = ((RecurringPlanConstraint.ID)key).RecurringPlanCycleID;

            return Load(cmd).FirstOrDefault();
        }

        public override RecurringPlanConstraint Load(DataRow row)
        {
            RecurringPlanConstraint res = new RecurringPlanConstraint();

            if (!(row["RecurringPlanCycleID"] is DBNull))
                res.RecurringPlanConstraintID = new RecurringPlanConstraint.ID()
                {
                    RecurringPlanCycleID = Convert.ToInt32(row["RecurringPlanCycleID"])
                };
            if (!(row["RecurringPlanCycleID"] is DBNull))
                res.RecurringPlanCycleID = Convert.ToInt32(row["RecurringPlanCycleID"]);
            if (!(row["ChargeTypeID"] is DBNull))
                res.ChargeTypeID = Convert.ToInt32(row["ChargeTypeID"]);
            if (!(row["Amount"] is DBNull))
                res.Amount = Convert.ToDecimal(row["Amount"]);
            if (!(row["ShippingAmount"] is DBNull))
                res.ShippingAmount = Convert.ToDecimal(row["ShippingAmount"]);
            if (!(row["TaxAmount"] is DBNull))
                res.TaxAmount = Convert.ToDecimal(row["TaxAmount"]);

            return res;
        }
    }
}
