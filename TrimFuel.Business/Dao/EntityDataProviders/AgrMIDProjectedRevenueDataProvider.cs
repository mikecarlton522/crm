using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class AgrMIDProjectedRevenueDataProvider : EntityDataProvider<AgrMIDProjectedRevenue>
    {
        private const string INSERT_COMMAND = "INSERT INTO AgrMIDProjectedRevenue(Year, Month, MerchantAccountID, ProjectedRevenue) VALUES(@Year, @Month, @MerchantAccountID, @ProjectedRevenue); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE AgrMIDProjectedRevenue SET Year=@Year, Month=@Month, MerchantAccountID=@MerchantAccountID, ProjectedRevenue=@ProjectedRevenue WHERE Year=@ID_Year and Month=@ID_Month and MerchantAccountID=@ID_MerchantAccountID;";
        private const string SELECT_COMMAND = "SELECT * FROM AgrMIDProjectedRevenue WHERE Year=@ID_Year and Month=@ID_Month and MerchantAccountID=@ID_MerchantAccountID;;";

        public override void Save(AgrMIDProjectedRevenue entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.AgrMIDProjectedRevenueID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ID_Year", MySqlDbType.Int32).Value = entity.AgrMIDProjectedRevenueID.Value.Year;
                cmd.Parameters.Add("@ID_Month", MySqlDbType.Int32).Value = entity.AgrMIDProjectedRevenueID.Value.Month;
                cmd.Parameters.Add("@ID_MerchantAccountID", MySqlDbType.Int32).Value = entity.AgrMIDProjectedRevenueID.Value.MerchantAccountID;
            }

            cmd.Parameters.Add("@Year", MySqlDbType.Int32).Value = entity.Year;
            cmd.Parameters.Add("@Month", MySqlDbType.Int32).Value = entity.Month;
            cmd.Parameters.Add("@MerchantAccountID", MySqlDbType.Int32).Value = entity.MerchantAccountID;
            cmd.Parameters.Add("@ProjectedRevenue", MySqlDbType.Decimal).Value = entity.ProjectedRevenue;


            if (entity.AgrMIDProjectedRevenueID == null)
            {
                cmd.ExecuteNonQuery();
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("AgrMIDProjectedRevenue({0},{1},{2}) was not found in database.", entity.AgrMIDProjectedRevenueID.Value.Year, entity.AgrMIDProjectedRevenueID.Value.Month, entity.AgrMIDProjectedRevenueID.Value.MerchantAccountID));
                }
            }
            entity.AgrMIDProjectedRevenueID = new AgrMIDProjectedRevenue.ID()
            {
                Year = entity.Year.Value,
                Month = entity.Month.Value,
                MerchantAccountID = entity.MerchantAccountID.Value
            };
        }

        public override AgrMIDProjectedRevenue Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ID_Year", MySqlDbType.Int32).Value = ((AgrMIDProjectedRevenue.ID)key).Year;
            cmd.Parameters.Add("@ID_Month", MySqlDbType.Int32).Value = ((AgrMIDProjectedRevenue.ID)key).Month;
            cmd.Parameters.Add("@ID_MerchantAccountID", MySqlDbType.Int32).Value = ((AgrMIDProjectedRevenue.ID)key).MerchantAccountID;

            return Load(cmd).FirstOrDefault();
        }

        public override AgrMIDProjectedRevenue Load(DataRow row)
        {
            AgrMIDProjectedRevenue res = new AgrMIDProjectedRevenue();

            if (!(row["Year"] is DBNull))
                res.Year = Convert.ToInt32(row["Year"]);
            if (!(row["Month"] is DBNull))
                res.Month = Convert.ToInt32(row["Month"]);
            if (!(row["MerchantAccountID"] is DBNull))
                res.MerchantAccountID = Convert.ToInt32(row["MerchantAccountID"]);
            if (!(row["ProjectedRevenue"] is DBNull))
                res.ProjectedRevenue = Convert.ToDecimal(row["ProjectedRevenue"]);

            res.AgrMIDProjectedRevenueID = new AgrMIDProjectedRevenue.ID() { Year = res.Year.Value, Month = res.Month.Value, MerchantAccountID = res.MerchantAccountID.Value };

            return res;
        }
    }
}
