using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class OutboundSalesViewDataProvider : EntityViewDataProvider<OutboundSalesView>
    {
        public override OutboundSalesView Load(DataRow row)
        {
            OutboundSalesView res = new OutboundSalesView();

            if (!(row["GrossRevenue"] is DBNull))
                res.GrossRevenue = Convert.ToDecimal(row["GrossRevenue"]);
            if (!(row["NetRevenue"] is DBNull))
                res.NetRevenue = Convert.ToDecimal(row["NetRevenue"]);
            if (!(row["NumberOfChargebacks"] is DBNull))
                res.NumberOfChargebacks = Convert.ToInt32(row["NumberOfChargebacks"]);
            if (!(row["NumberOfLeads"] is DBNull))
                res.NumberOfLeads = Convert.ToInt32(row["NumberOfLeads"]);
            if (!(row["Sales"] is DBNull))
                res.NumberOfSales = Convert.ToInt32(row["Sales"]);
            if (!(row["Refunds"] is DBNull))
                res.Refunds = Convert.ToDecimal(row["Refunds"]);
            if (!(row["LeadPartnerID"] is DBNull))
                res.LeadPartnerID = Convert.ToInt32(row["LeadPartnerID"]);
            if (!(row["CostOfSales"] is DBNull))
                res.CostOfSales = Convert.ToDecimal(row["CostOfSales"]);
            if (!(row["Conversion"] is DBNull))
                res.Conversion = Convert.ToDouble(row["Conversion"]);

            return res;
        }
    }
}
