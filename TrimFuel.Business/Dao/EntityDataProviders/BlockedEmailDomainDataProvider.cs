using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class BlockedEmailDomainDataProvider : EntityDataProvider<BlockedEmailDomain>
    {
        private const string INSERT_COMMAND = "INSERT INTO BlockedEmailDomain(Name, CreateDT) VALUES(@Name, @CreateDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE BlockedEmailDomain SET Name=@Name, CreateDT=@CreateDT WHERE BlockedEmailDomainID=@BlockedEmailDomainID;";
        private const string SELECT_COMMAND = "SELECT * FROM BlockedEmailDomain WHERE BlockedEmailDomainID=@BlockedEmailDomainID;";

        public override void Save(BlockedEmailDomain entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.BlockedEmailDomainId == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@BlockedEmailDomainID", MySqlDbType.Int32).Value = entity.BlockedEmailDomainId;
            }

            cmd.Parameters.Add("@Name", MySqlDbType.VarChar).Value = entity.Name;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.DateTime).Value = entity.CreateDT;


            if (entity.BlockedEmailDomainId == null)
            {
                entity.BlockedEmailDomainId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("BlockedEmailDomain({0}) was not found in database.", entity.BlockedEmailDomainId));
                }
            }
        }

        public override BlockedEmailDomain Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand("SELECT * FROM BlockedEmailDomain WHERE BlockedEmailDomainID=@BlockedEmailDomainID;");
            cmd.Parameters.Add("@BlockedEmailDomainID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override BlockedEmailDomain Load(DataRow row)
        {
            BlockedEmailDomain res = new BlockedEmailDomain();

            if (!(row["BlockedEmailDomainID"] is DBNull))
                res.BlockedEmailDomainId = Convert.ToInt32(row["BlockedEmailDomainID"]);
            if (!(row["Name"] is DBNull))
                res.Name = Convert.ToString(row["Name"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);

            return res;
        }
    }
}
