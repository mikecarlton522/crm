using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class VeriSectPost : Entity
    {
        public long? VeriSectPostID { get; set; }
        public long? ChargeHistoryID { get; set; }
        public int? VeriSectPostType { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("ChargeHistoryID", ChargeHistoryID);
            v.AssertNotNull("VeriSectPostType", VeriSectPostType);
            v.AssertString("Request", Request, 2000);
            v.AssertString("Response", Response, 2000);
        }
    }
}
