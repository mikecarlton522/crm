using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class VeriSectPostDataProvider : EntityDataProvider<VeriSectPost>
    {
        private const string INSERT_COMMAND = "INSERT INTO VeriSectPost(ChargeHistoryID, VeriSectPostType, Request, Response) VALUES(@ChargeHistoryID, @VeriSectPostType, @Request, @Response); SELECT @@IDENTITY;";

        public override void Save(VeriSectPost entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.VeriSectPostID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                throw new NotImplementedException();
            }

            cmd.Parameters.Add("@ChargeHistoryID", MySqlDbType.Int64).Value = entity.ChargeHistoryID;
            cmd.Parameters.Add("@VeriSectPostType", MySqlDbType.Int32).Value = entity.VeriSectPostType;
            cmd.Parameters.Add("@Request", MySqlDbType.VarChar).Value = entity.Request;
            cmd.Parameters.Add("@Response", MySqlDbType.VarChar).Value = entity.Response;

            if (entity.VeriSectPostID == null)
            {
                entity.VeriSectPostID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("VeriSectPost({0}) was not found in database.", entity.VeriSectPostID));
                }
            }
        }

        public override VeriSectPost Load(object key, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override VeriSectPost Load(DataRow row)
        {
            throw new NotImplementedException();
        }
    }
}
