using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class CampaignReferralDiscountDataProvider : EntityDataProvider<CampaignReferralDiscount>
    {
        private const string SELECT_COMMAND = "SELECT * FROM CampaignReferralDiscount WHERE CampaignID=@CampaignID;";

        public override void Save(CampaignReferralDiscount entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override CampaignReferralDiscount Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@CampaignID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override CampaignReferralDiscount Load(DataRow row)
        {
            CampaignReferralDiscount res = new CampaignReferralDiscount();

            if (!(row["CampaignID"] is DBNull))
                res.CampaignID = Convert.ToInt32(row["CampaignID"]);
            if (!(row["Discount"] is DBNull))
                res.Discount = Convert.ToDecimal(row["Discount"]);

            return res;
        }
    }
}
