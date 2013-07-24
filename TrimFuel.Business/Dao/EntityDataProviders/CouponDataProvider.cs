using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class CouponDataProvider : EntityDataProvider<Coupon>
    {
        private const string SELECT_COMMAND = "SELECT * FROM Coupon WHERE ID=@CouponID;";
        private const string INSERT_COMMAND = "INSERT INTO Coupon(ProductID, Code, Discount, NewPrice) VALUES(@ProductID, @Code, @Discount, @NewPrice);";
        private const string UPDATE_COMMAND = "UPDATE Coupon SET ProductID=@ProductID, Code=@Code, Discount=@Discount, NewPrice=@NewPrice Where ID=@CouponID;";

        public override void Save(Coupon entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            int id = GetNewID(cmdCreater);

            if (entity.CouponID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@CouponID", MySqlDbType.Int32).Value = entity.CouponID;
            }

            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = entity.ProductID;
            cmd.Parameters.Add("@Code", MySqlDbType.VarChar).Value = entity.Code;
            cmd.Parameters.Add("@Discount", MySqlDbType.Decimal).Value = entity.Discount;
            cmd.Parameters.Add("@NewPrice", MySqlDbType.Decimal).Value = entity.NewPrice;            

            if (entity.CouponID == null)
            {
                cmd.ExecuteNonQuery();
                entity.CouponID = id;
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Coupon({0}) was not found in database.", entity.CouponID));
                }
            }
        }

        public override Coupon Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@CouponID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Coupon Load(DataRow row)
        {
            Coupon res = new Coupon();

            if (!(row["ID"] is DBNull))
                res.CouponID = Convert.ToInt32(row["ID"]);
            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);
            if (!(row["Code"] is DBNull))
                res.Code = Convert.ToString(row["Code"]);
            if (!(row["Discount"] is DBNull))
                res.Discount = Convert.ToDecimal(row["Discount"]);
            if (!(row["NewPrice"] is DBNull))
                res.NewPrice = Convert.ToDecimal(row["NewPrice"]);

            return res;
        }

        private int GetNewID(IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand q = cmdCreater.CreateCommand(@"
                select IfNull(max(ID), 0) from Coupon
            ");
            object res = q.ExecuteScalar();
            return Convert.ToInt32(res) + 1;
        }
    }
}
