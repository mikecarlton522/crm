using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class TempConversationDataProvider : EntityDataProvider<TempConversion>
    {
        private const string INSERT_COMMAND = "INSERT INTO TempConversion(PageTypeID, Affiliate, SubAffiliate, CreateDT, CampaignID) VALUES(@PageTypeID, @Affiliate, @SubAffiliate, @CreateDT, @CampaignID); SELECT @@IDENTITY;";

        public override void Save(TempConversion entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.TempConversionID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                throw new NotImplementedException();
            }

            cmd.Parameters.Add("@PageTypeID", MySqlDbType.Int32).Value = entity.PageTypeID;
            cmd.Parameters.Add("@Affiliate", MySqlDbType.VarChar).Value = entity.Affiliate;
            cmd.Parameters.Add("@SubAffiliate", MySqlDbType.VarChar).Value = entity.SubAffiliate;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;
            cmd.Parameters.Add("@CampaignID", MySqlDbType.Int32).Value = entity.CampaignID;

            if (entity.TempConversionID == null)
            {
                entity.TempConversionID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override TempConversion Load(object key, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override TempConversion Load(DataRow row)
        {
            throw new NotImplementedException();
        }
    }
}
