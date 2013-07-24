using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class TPClientNoteViewDataProvider : EntityViewDataProvider<TPClientNoteView>
    {
        public override TPClientNoteView Load(System.Data.DataRow row)
        {
            TPClientNoteView res = new TPClientNoteView();

            if (!(row["TPClientNoteID"] is DBNull))
                res.TPClientNoteID = Convert.ToInt32(row["TPClientNoteID"]);
            if (!(row["DisplayName"] is DBNull))
                res.AdminName = Convert.ToString(row["DisplayName"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);

            return res;
        }
    }
}
