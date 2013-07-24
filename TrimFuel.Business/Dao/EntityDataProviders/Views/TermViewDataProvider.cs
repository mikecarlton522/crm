using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class TermViewDataProvider : EntityViewDataProvider<TermView>
    {
        public override TermView Load(System.Data.DataRow row)
        {
            TermView res = new TermView();

            if (!(row["CorporationName"] is DBNull))
                res.CorporationName = Convert.ToString(row["CorporationName"]);
            if (!(row["CorporationBody"] is DBNull))
                res.CorporationBody = Convert.ToString(row["CorporationBody"]);
            if (!(row["ReturnAddressName"] is DBNull))
                res.ReturnAddressName = Convert.ToString(row["ReturnAddressName"]);
            if (!(row["ReturnAddressBody"] is DBNull))
                res.ReturnAddressBody = Convert.ToString(row["ReturnAddressBody"]);
            if (!(row["Phone"] is DBNull))
                res.Phone = Convert.ToString(row["Phone"]);
            if (!(row["Email"] is DBNull))
                res.Email = Convert.ToString(row["Email"]);
            if (!(row["MembershipTerms"] is DBNull))
                res.MembershipTerms = Convert.ToString(row["MembershipTerms"]);
            if (!(row["StraightSaleTerms"] is DBNull))
                res.StraightSaleTerms = Convert.ToString(row["StraightSaleTerms"]);
            if (!(row["Outline"] is DBNull))
                res.Outline = Convert.ToString(row["Outline"]);
            if (!(row["PrivacyPolicy"] is DBNull))
                res.PrivacyPolicy = Convert.ToString(row["PrivacyPolicy"]);

            return res;
        }
    }
}
