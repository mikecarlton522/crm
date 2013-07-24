using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class SaleCouponCodeDataProvider : EntityDataProvider<SaleCouponCode>
    {
        private const string INSERT_COMMAND = "INSERT INTO SaleCouponCode(SaleID, CouponCode, CreateDT) VALUES(@SaleID, @CouponCode, @CreateDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE SaleCouponCode SET SaleID=@SaleID, CouponCode=@CouponCode, CreateDT=@CreateDT WHERE SaleCouponCodeID=@SaleCouponCodeID;";

        public override void Save(SaleCouponCode entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.SaleCouponCodeID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@SaleCouponCodeID", MySqlDbType.Int32).Value = entity.SaleCouponCodeID;
            }

            cmd.Parameters.Add("@SaleID", MySqlDbType.Int32).Value = entity.SaleID;
            cmd.Parameters.Add("@CouponCode", MySqlDbType.VarChar).Value = entity.CouponCode;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;

            if (entity.SaleCouponCodeID == null)
            {
                entity.SaleCouponCodeID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("SaleCouponCode({0}) was not found in database.", entity.SaleCouponCodeID));
                }
            }
        }

        public override SaleCouponCode Load(object key, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override SaleCouponCode Load(DataRow row)
        {
            throw new NotImplementedException();
        }
    }
}
