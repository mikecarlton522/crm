using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business
{
    public class ConversionService : BaseService
    {
        public Conversion GetConversionByFilter(int? campaignId, int? pageTypeId, string affiliate, string subAffiliate)
        {
            Conversion conversion = null;
            try
            {
                var q = new MySqlCommand("SELECT * FROM Conversion " +
                                        "WHERE " +
                                        "Conversion.PageTypeID = @pageTypeId and " +
                                        "Conversion.Affiliate = @affiliate and " +
                                        "Conversion.SubAffiliate = @subAffiliate and " +
                                        "Conversion.CampaignID = @campaignId and " + 
                                        "Conversion.Hour = HOUR( NOW() )"
                                        );
                q.Parameters.Add("@campaignId", MySqlDbType.Int32).Value = campaignId;
                q.Parameters.Add("@pageTypeId", MySqlDbType.Int32).Value = pageTypeId;
                q.Parameters.Add("@affiliate", MySqlDbType.VarChar).Value = affiliate;
                q.Parameters.Add("@subAffiliate", MySqlDbType.VarChar).Value = subAffiliate;
                conversion = dao.Load<Conversion>(q).FirstOrDefault();
            }
            catch(Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return conversion;
        }

        public bool SetConversion(Conversion conversion)
        {
            bool res = false;
           
            try
            {
                dao.Save<Conversion>(conversion);
                res = true;
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }

            return res;
        }
    }
}
