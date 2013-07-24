using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;
using TrimFuel.Model;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class CampaignEmailTypeViewDataProvider : EntityViewDataProvider<CampaignEmailTypeView>
    {
        public override CampaignEmailTypeView Load(DataRow row)
        {
            CampaignEmailTypeView res = new CampaignEmailTypeView();

            if (!(row["DynamicEmailTypeID"] is DBNull))
                res.DynamicEmailTypeID = Convert.ToInt32(row["DynamicEmailTypeID"]);
            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);
            if (!(row["CampaignID"] is DBNull))
                res.CampaignID = Convert.ToInt32(row["CampaignID"]);
            if (!(row["Hours"] is DBNull))
                res.Hours = Convert.ToInt32(row["Hours"]);
            if (!(row["DisplayName"] is DBNull))
                res.DisplayName = Convert.ToString(row["DisplayName"]);
            if (!(row["CustomName"] is DBNull))
                res.CustomName = Convert.ToString(row["CustomName"]);
            if (!(row["DynamicEmailID"] is DBNull))
                res.DynamicEmailID = Convert.ToInt32(row["DynamicEmailID"]);
            if (!(row["Active"] is DBNull))
                res.Active = Convert.ToBoolean(row["Active"]);
            if (!(row["EmailCount"] is DBNull))
                res.EmailCount = Convert.ToInt32(row["EmailCount"]);
            if (!(row["GiftCertificateDynamicEmail_StoreID"] is DBNull))
                res.GiftCertificateDynamicEmail_StoreID = Convert.ToInt32(row["GiftCertificateDynamicEmail_StoreID"]);

            return res;
        }
    }
}
