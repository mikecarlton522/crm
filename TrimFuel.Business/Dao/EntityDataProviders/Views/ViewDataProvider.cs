using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class ViewDataProvider<TStruct> : EntityViewDataProvider<View<TStruct>> where TStruct : struct
    {
        public override View<TStruct> Load(DataRow row)
        {
            View<TStruct> res = new View<TStruct>();

            if (!(row["Value"] is DBNull))
                res.Value = (TStruct)Convert.ChangeType(row["Value"], typeof(TStruct));

            return res;
        }
    }
}
