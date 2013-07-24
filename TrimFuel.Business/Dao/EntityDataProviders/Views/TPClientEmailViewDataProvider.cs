using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class TPClientEmailViewDataProvider : EntityViewDataProvider<TPClientEmailView>
    {
        public override TPClientEmailView Load(System.Data.DataRow row)
        {
            TPClientEmailView res = new TPClientEmailView();

            if (!(row["TPClientEmailID"] is DBNull))
                res.TPClientEmailID = Convert.ToInt32(row["TPClientEmailID"]);
            if (!(row["DisplayName"] is DBNull))
                res.AdminName = Convert.ToString(row["DisplayName"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["To"] is DBNull))
                res.To = Convert.ToString(row["To"]);

            return res;
        }
    }
}
