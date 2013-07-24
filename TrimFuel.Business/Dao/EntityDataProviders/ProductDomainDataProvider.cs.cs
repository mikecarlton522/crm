using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ProductDomainDataProvider : EntityDataProvider<ProductDomain>
    {
        private const string INSERT_COMMAND = "INSERT INTO ProductDomain(ProductID, DomainName) VALUES(@ProductID, @DomainName); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE ProductDomain SET ProductID=@ProductID, DomainName=@DomainName WHERE ProductDomainID=@ProductDomainID;";
        private const string SELECT_COMMAND = "SELECT * FROM ProductDomain WHERE ProductDomainID=@ProductDomainID;";

        public override void Save(ProductDomain entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.ProductDomainID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ProductDomainID", MySqlDbType.Int32).Value = entity.ProductDomainID;
            }

            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = entity.ProductID;
            cmd.Parameters.Add("@DomainName", MySqlDbType.VarChar).Value = entity.DomainName;


            if (entity.ProductDomainID == null)
            {
                entity.ProductDomainID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("ProductDomain({0}) was not found in database.", entity.ProductDomainID));
                }
            }
        }

        public override ProductDomain Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ProductDomainID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ProductDomain Load(DataRow row)
        {
            ProductDomain res = new ProductDomain();

            if (!(row["ProductDomainID"] is DBNull))
                res.ProductDomainID = Convert.ToInt32(row["ProductDomainID"]);
            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);
            if (!(row["DomainName"] is DBNull))
                res.DomainName = Convert.ToString(row["DomainName"]);

            return res;
        }
    }
}
