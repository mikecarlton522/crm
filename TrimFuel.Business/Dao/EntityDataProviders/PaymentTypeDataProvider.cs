using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class PaymentTypeDataProvider : EntityDataProvider<PaymentType>
    {
        public override void Save(PaymentType entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override PaymentType Load(object key, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override PaymentType Load(System.Data.DataRow row)
        {
            PaymentType res = new PaymentType();

            if (!(row["PaymentTypeID"] is DBNull))
                res.PaymentTypeID = Convert.ToInt32(row["PaymentTypeID"]);
            if (!(row["DisplayName"] is DBNull))
                res.DisplayName = Convert.ToString(row["DisplayName"]);

            return res;
        }
    }
}
