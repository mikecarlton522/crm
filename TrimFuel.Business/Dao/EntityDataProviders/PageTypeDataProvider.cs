using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class PageTypeDataProvider : EntityDataProvider<PageType>
    {
        public override void Save(PageType entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override PageType Load(object key, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override PageType Load(System.Data.DataRow row)
        {
            PageType res = new PageType();

            if (!(row["PageTypeID"] is DBNull))
                res.PageTypeID = Convert.ToInt32(row["PageTypeID"]);
            if (!(row["Name"] is DBNull))
                res.Name = Convert.ToString(row["Name"]);
            if (!(row["Order"] is DBNull))
                res.Order = Convert.ToInt32(row["Order"]);
            
            return res;

        }
    }
}
