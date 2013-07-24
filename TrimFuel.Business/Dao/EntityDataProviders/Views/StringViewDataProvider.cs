using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class StringViewDataProvider : EntityViewDataProvider<StringView>
    {
        public override StringView Load(DataRow row)
        {
            StringView res = new StringView();

            if (!(row["Value"] is DBNull))
                res.Value = Convert.ToString(row["Value"]);

            return res;
        }
    }
}
