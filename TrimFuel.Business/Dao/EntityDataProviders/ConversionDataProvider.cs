using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ConversionDataProvider : EntityDataProvider<Conversion>
    {
        private const string INSERT_COMMAND = "INSERT INTO Conversion(CampaignID, PageTypeID, Affiliate, SubAffiliate, Flow, Hits, Hour, CreateDT) VALUES(@CampaignID, @PageTypeID, @Affiliate, @SubAffiliate, @Flow, @Hits, @Hour, @CreateDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE Conversion SET CampaignID=@CampaignID, PageTypeID=@PageTypeID, Affiliate=@Affiliate, SubAffiliate=@SubAffiliate, Flow=@Flow, Hits=@Hits, Hour=@Hour, CreateDT=@CreateDT WHERE ConversionID=@ConversionID;";
        private const string SELECT_COMMAND = "SELECT * FROM Conversion WHERE ConversionID=@ConversionID;";

        public override void Save(Conversion entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.ConversionID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ConversionID", MySqlDbType.Int32).Value = entity.ConversionID;
            }

            cmd.Parameters.Add("@CampaignID", MySqlDbType.Int64).Value = entity.CampaignID;
            cmd.Parameters.Add("@PageTypeID", MySqlDbType.Int64).Value = entity.PageTypeID;
            cmd.Parameters.Add("@Affiliate", MySqlDbType.VarChar).Value = entity.Affiliate;
            cmd.Parameters.Add("@SubAffiliate", MySqlDbType.VarChar).Value = entity.SubAffiliate;
            cmd.Parameters.Add("@Flow", MySqlDbType.VarChar).Value = entity.Flow;
            cmd.Parameters.Add("@Hits", MySqlDbType.Int32).Value = entity.Hits;
            cmd.Parameters.Add("@Hour", MySqlDbType.Int32).Value = entity.Hour;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;

            if (entity.ConversionID == null)
            {
                entity.ConversionID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Conversion ({0}) was not found in database.", entity.ConversionID));
                }
            }
        }

        public override Conversion Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ConversionID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Conversion Load(DataRow row)
        {
            Conversion res = new Conversion();

            if (!(row["ConversionID"] is DBNull))
                res.ConversionID = Convert.ToInt32(row["ConversionID"]);
            if (!(row["PageTypeID"] is DBNull))
                res.PageTypeID = Convert.ToInt32(row["PageTypeID"]);
            if (!(row["Affiliate"] is DBNull))
                res.Affiliate = Convert.ToString(row["Affiliate"]);
            if (!(row["SubAffiliate"] is DBNull))
                res.SubAffiliate = Convert.ToString(row["SubAffiliate"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["Flow"] is DBNull))
                res.Flow = Convert.ToString(row["Flow"]);
            if (!(row["Hits"] is DBNull))
                res.Hits = Convert.ToInt32(row["Hits"]);
            if (!(row["CampaignID"] is DBNull))
                res.CampaignID = Convert.ToInt32(row["CampaignID"]);
            if (!(row["Hour"] is DBNull))
                res.Hour = Convert.ToByte(row["Hour"]);

            return res;

        }
    }
}
