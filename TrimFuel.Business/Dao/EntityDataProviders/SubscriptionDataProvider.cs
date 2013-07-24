using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class SubscriptionDataProvider : EntityDataProvider<Subscription>
    {
        private const string INSERT_COMMAND = "INSERT INTO Subscription(DisplayName, ProductName, Description, ParentSubscriptionID, InitialInterim, InitialShipping, SaveShipping, SaveBilling, InitialBillAmount, SecondInterim, SecondShipping, SecondBillAmount, RegularInterim, RegularShipping, RegularBillAmount, ProductCode, Selectable, Quantity, ProductID, Recurring, ShipFirstRebill, SKU2, TrialText, RecurText, SaveText, UpsellText, ReplacementText, ReplacementTrialText, QuantitySKU2) VALUES(@DisplayName, @ProductName, @Description, @ParentSubscriptionID, @InitialInterim, @InitialShipping, @SaveShipping, @SaveBilling, @InitialBillAmount, @SecondInterim, @SecondShipping, @SecondBillAmount, @RegularInterim, @RegularShipping, @RegularBillAmount, @ProductCode, @Selectable, @Quantity, @ProductID, @Recurring, @ShipFirstRebill, @SKU2, @TrialText, @RecurText, @SaveText, @UpsellText, @ReplacementText, @ReplacementTrialText, @QuantitySKU2); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE Subscription SET DisplayName=@DisplayName, ProductName=@ProductName, Description=@Description, ParentSubscriptionID=@ParentSubscriptionID, InitialInterim=@InitialInterim, InitialShipping=@InitialShipping, SaveShipping=@SaveShipping, SaveBilling=@SaveBilling, InitialBillAmount=@InitialBillAmount, SecondInterim=@SecondInterim, SecondShipping=@SecondShipping, SecondBillAmount=@SecondBillAmount, RegularInterim=@RegularInterim, RegularShipping=@RegularShipping, RegularBillAmount=@RegularBillAmount, ProductCode=@ProductCode, Selectable=@Selectable, Quantity=@Quantity, ProductID=@ProductID, Recurring=@Recurring, ShipFirstRebill=@ShipFirstRebill, SKU2=@SKU2, TrialText=@TrialText, RecurText=@RecurText, SaveText=@SaveText, UpsellText=@UpsellText, ReplacementText=@ReplacementText, ReplacementTrialText=@ReplacementTrialText, QuantitySKU2=@QuantitySKU2 WHERE SubscriptionID=@SubscriptionID;";
        private const string SELECT_COMMAND = "SELECT * FROM Subscription WHERE SubscriptionID=@SubscriptionID;";

        public override void Save(Subscription entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.SubscriptionID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@SubscriptionID", MySqlDbType.Int32).Value = entity.SubscriptionID;
            }

            cmd.Parameters.Add("@DisplayName", MySqlDbType.VarChar).Value = entity.DisplayName;
            cmd.Parameters.Add("@ProductName", MySqlDbType.VarChar).Value = entity.ProductName;
            cmd.Parameters.Add("@Description", MySqlDbType.VarChar).Value = entity.Description;
            cmd.Parameters.Add("@ParentSubscriptionID", MySqlDbType.Int32).Value = entity.ParentSubscriptionID;
            cmd.Parameters.Add("@InitialInterim", MySqlDbType.Int32).Value = entity.InitialInterim;
            cmd.Parameters.Add("@InitialShipping", MySqlDbType.Decimal).Value = entity.InitialShipping;
            cmd.Parameters.Add("@SaveShipping", MySqlDbType.Decimal).Value = entity.SaveShipping;
            cmd.Parameters.Add("@SaveBilling", MySqlDbType.Decimal).Value = entity.SaveBilling;
            cmd.Parameters.Add("@InitialBillAmount", MySqlDbType.Decimal).Value = entity.InitialBillAmount;
            cmd.Parameters.Add("@SecondInterim", MySqlDbType.Int32).Value = entity.SecondInterim;
            cmd.Parameters.Add("@SecondShipping", MySqlDbType.Decimal).Value = entity.SecondShipping;
            cmd.Parameters.Add("@SecondBillAmount", MySqlDbType.Decimal).Value = entity.SecondBillAmount;
            cmd.Parameters.Add("@RegularInterim", MySqlDbType.Int32).Value = entity.RegularInterim;
            cmd.Parameters.Add("@RegularShipping", MySqlDbType.Decimal).Value = entity.RegularShipping;
            cmd.Parameters.Add("@RegularBillAmount", MySqlDbType.Decimal).Value = entity.RegularBillAmount;
            cmd.Parameters.Add("@ProductCode", MySqlDbType.VarChar).Value = entity.ProductCode;
            cmd.Parameters.Add("@Selectable", MySqlDbType.Bit).Value = entity.Selectable;
            cmd.Parameters.Add("@Quantity", MySqlDbType.Int32).Value = entity.Quantity;
            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = entity.ProductID;
            cmd.Parameters.Add("@Recurring", MySqlDbType.Bit).Value = entity.Recurring;
            cmd.Parameters.Add("@ShipFirstRebill", MySqlDbType.Bit).Value = entity.ShipFirstRebill;
            cmd.Parameters.Add("@SKU2", MySqlDbType.VarChar).Value = entity.SKU2;
            cmd.Parameters.Add("@TrialText", MySqlDbType.VarChar).Value = entity.TrialText;
            cmd.Parameters.Add("@RecurText", MySqlDbType.VarChar).Value = entity.RecurText;
            cmd.Parameters.Add("@SaveText", MySqlDbType.VarChar).Value = entity.SaveText;
            cmd.Parameters.Add("@UpsellText", MySqlDbType.VarChar).Value = entity.UpsellText;
            cmd.Parameters.Add("@ReplacementText", MySqlDbType.VarChar).Value = entity.ReplacementText;
            cmd.Parameters.Add("@ReplacementTrialText", MySqlDbType.VarChar).Value = entity.ReplacementTrialText;
            cmd.Parameters.Add("@QuantitySKU2", MySqlDbType.Int32).Value = entity.QuantitySKU2;

            if (entity.SubscriptionID == null)
            {
                entity.SubscriptionID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Subscription({0}) was not found in database.", entity.SubscriptionID));
                }
            }
        }

        public override Subscription Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@SubscriptionID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Subscription Load(DataRow row)
        {
            Subscription res = new Subscription();

            if (!(row["SubscriptionID"] is DBNull))
                res.SubscriptionID = Convert.ToInt32(row["SubscriptionID"]);
            if (!(row["DisplayName"] is DBNull))
                res.DisplayName = Convert.ToString(row["DisplayName"]);
            if (!(row["ProductName"] is DBNull))
                res.ProductName = Convert.ToString(row["ProductName"]);
            if (!(row["Description"] is DBNull))
                res.Description = Convert.ToString(row["Description"]);
            if (!(row["ParentSubscriptionID"] is DBNull))
                res.ParentSubscriptionID = Convert.ToInt32(row["ParentSubscriptionID"]);
            if (!(row["InitialInterim"] is DBNull))
                res.InitialInterim = Convert.ToInt32(row["InitialInterim"]);
            if (!(row["InitialShipping"] is DBNull))
                res.InitialShipping = Convert.ToDecimal(row["InitialShipping"]);
            if (!(row["SaveShipping"] is DBNull))
                res.SaveShipping = Convert.ToDecimal(row["SaveShipping"]);
            if (!(row["SaveBilling"] is DBNull))
                res.SaveBilling = Convert.ToDecimal(row["SaveBilling"]);
            if (!(row["InitialBillAmount"] is DBNull))
                res.InitialBillAmount = Convert.ToDecimal(row["InitialBillAmount"]);
            if (!(row["SecondInterim"] is DBNull))
                res.SecondInterim = Convert.ToInt32(row["SecondInterim"]);
            if (!(row["SecondShipping"] is DBNull))
                res.SecondShipping = Convert.ToDecimal(row["SecondShipping"]);
            if (!(row["SecondBillAmount"] is DBNull))
                res.SecondBillAmount = Convert.ToDecimal(row["SecondBillAmount"]);
            if (!(row["RegularInterim"] is DBNull))
                res.RegularInterim = Convert.ToInt32(row["RegularInterim"]);
            if (!(row["RegularShipping"] is DBNull))
                res.RegularShipping = Convert.ToDecimal(row["RegularShipping"]);
            if (!(row["RegularBillAmount"] is DBNull))
                res.RegularBillAmount = Convert.ToDecimal(row["RegularBillAmount"]);
            if (!(row["ProductCode"] is DBNull))
                res.ProductCode = Convert.ToString(row["ProductCode"]);
            if (!(row["Selectable"] is DBNull))
                res.Selectable = Convert.ToBoolean(row["Selectable"]);
            if (!(row["Quantity"] is DBNull))
                res.Quantity = Convert.ToInt32(row["Quantity"]);
            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);
            if (!(row["Recurring"] is DBNull))
                res.Recurring = Convert.ToBoolean(row["Recurring"]);
            if (!(row["ShipFirstRebill"] is DBNull))
                res.ShipFirstRebill = Convert.ToBoolean(row["ShipFirstRebill"]);
            if (!(row["SKU2"] is DBNull))
                res.SKU2 = Convert.ToString(row["SKU2"]);
            if (!(row["TrialText"] is DBNull))
                res.TrialText = Convert.ToString(row["TrialText"]);
            if (!(row["RecurText"] is DBNull))
                res.RecurText = Convert.ToString(row["RecurText"]);
            if (!(row["SaveText"] is DBNull))
                res.SaveText = Convert.ToString(row["SaveText"]);
            if (!(row["UpsellText"] is DBNull))
                res.UpsellText = Convert.ToString(row["UpsellText"]);
            if (!(row["ReplacementText"] is DBNull))
                res.ReplacementText = Convert.ToString(row["ReplacementText"]);
            if (!(row["ReplacementTrialText"] is DBNull))
                res.ReplacementTrialText = Convert.ToString(row["ReplacementTrialText"]);
            if (!(row["QuantitySKU2"] is DBNull))
                res.QuantitySKU2 = Convert.ToInt32(row["QuantitySKU2"]);

            return res;
        }
    }
}
