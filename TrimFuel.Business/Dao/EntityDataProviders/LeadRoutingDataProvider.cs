using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class LeadRoutingDataProvider : EntityDataProvider<LeadRouting>
    {
        private const string INSERT_COMMAND = "INSERT INTO LeadRouting(ProductID, LeadTypeID, LeadPartnerID, Percentage) VALUES(@ProductID, @LeadTypeID, @LeadPartnerID, @Percentage);";
        private const string UPDATE_COMMAND = "UPDATE LeadRouting SET ProductID=@ProductID, LeadTypeID=@LeadTypeID, LeadPartnerID=@LeadPartnerID, Percentage=@Percentage WHERE ProductID=@IDProductID and LeadTypeID=@IDLeadTypeID and LeadPartnerID=@IDLeadPartnerID;";
        private const string SELECT_COMMAND = "SELECT * FROM LeadRouting WHERE ProductID=@IDProductID and LeadTypeID=@IDLeadTypeID and LeadPartnerID=@IDLeadPartnerID;";


        public override void Save(LeadRouting entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.LeadRoutingID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@IDProductID", MySqlDbType.Int32).Value = entity.LeadRoutingID.Value.ProductID;
                cmd.Parameters.Add("@IDLeadTypeID", MySqlDbType.Int32).Value = entity.LeadRoutingID.Value.LeadTypeID;
                cmd.Parameters.Add("@IDLeadPartnerID", MySqlDbType.Int32).Value = entity.LeadRoutingID.Value.LeadPartnerID;
            }

            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = entity.ProductID;
            cmd.Parameters.Add("@LeadTypeID", MySqlDbType.Int32).Value = entity.LeadTypeID;
            cmd.Parameters.Add("@LeadPartnerID", MySqlDbType.Int32).Value = entity.LeadPartnerID;
            cmd.Parameters.Add("@Percentage", MySqlDbType.Int32).Value = entity.Percentage;

            if (entity.LeadRoutingID == null)
            {
                cmd.ExecuteNonQuery();
                entity.LeadRoutingID = new LeadRouting.ID() { ProductID = entity.ProductID.Value, LeadTypeID = entity.LeadTypeID.Value, LeadPartnerID = entity.LeadPartnerID.Value };
            }
            else
            {
                cmd.ExecuteNonQuery();
                entity.LeadRoutingID = new LeadRouting.ID() { ProductID = entity.ProductID.Value, LeadTypeID = entity.LeadTypeID.Value, LeadPartnerID = entity.LeadPartnerID.Value };
            }
        }

        public override LeadRouting Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@IDProductID", MySqlDbType.Int32).Value = ((LeadRouting.ID?)key).Value.ProductID;
            cmd.Parameters.Add("@IDLeadTypeID", MySqlDbType.Int32).Value = ((LeadRouting.ID?)key).Value.LeadTypeID;
            cmd.Parameters.Add("@IDLeadPartnerID", MySqlDbType.Int32).Value = ((LeadRouting.ID?)key).Value.LeadPartnerID;

            return Load(cmd).FirstOrDefault();
        }

        public override LeadRouting Load(DataRow row)
        {
            LeadRouting res = new LeadRouting();

            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);
            if (!(row["LeadTypeID"] is DBNull))
                res.LeadTypeID = Convert.ToInt32(row["LeadTypeID"]);
            if (!(row["LeadPartnerID"] is DBNull))
                res.LeadPartnerID = Convert.ToInt32(row["LeadPartnerID"]);
            if (!(row["Percentage"] is DBNull))
                res.Percentage = Convert.ToInt32(row["Percentage"]);

            return res;
        }
    }
}
