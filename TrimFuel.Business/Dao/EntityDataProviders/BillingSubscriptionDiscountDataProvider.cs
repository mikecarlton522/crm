using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class BillingSubscriptionDiscountDataProvider : EntityDataProvider<BillingSubscriptionDiscount>
    {
        private const string INSERT_COMMAND = "INSERT INTO BillingSubscriptionDiscount(BillingSubscriptionID, IsSavePrice, Discount, NewShippingAmount) VALUES(@BillingSubscriptionID, @IsSavePrice, @Discount, @NewShippingAmount);";
        private const string UPDATE_COMMAND = "UPDATE BillingSubscriptionDiscount SET BillingSubscriptionID=@BillingSubscriptionID, IsSavePrice=@IsSavePrice, Discount=@Discount, NewShippingAmount=@NewShippingAmount WHERE BillingSubscriptionID=@BillingSubscriptionID;";

        public override void Save(BillingSubscriptionDiscount entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            //BillingSubscription 1 <-> 0..1 BillingSubscriptionDiscount association
            //Try to update first
            cmd.CommandText = UPDATE_COMMAND;

            cmd.Parameters.Add("@BillingSubscriptionID", MySqlDbType.Int32).Value = entity.BillingSubscriptionID;
            cmd.Parameters.Add("@IsSavePrice", MySqlDbType.Bit).Value = entity.IsSavePrice;
            cmd.Parameters.Add("@Discount", MySqlDbType.Decimal).Value = entity.Discount;
            cmd.Parameters.Add("@NewShippingAmount", MySqlDbType.Decimal).Value = entity.NewShippingAmount;

            //BillingSubscription 1 <-> 0..1 BillingSubscriptionDiscount association
            if (cmd.ExecuteNonQuery() == 0)
            {
                //BillingSubscription 1 <-> 0..1 BillingSubscriptionDiscount association
                //If update failed try to insert
                cmd.CommandText = INSERT_COMMAND;
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Foreign Key BillingSubscription({0}) was not found in database.", entity.BillingSubscriptionID));
                }
            }
        }

        public override BillingSubscriptionDiscount Load(object key, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override BillingSubscriptionDiscount Load(DataRow row)
        {
            throw new NotImplementedException();
        }
    }
}
