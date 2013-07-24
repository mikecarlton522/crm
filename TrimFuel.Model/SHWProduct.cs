using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class SHWProduct : Entity
    {
        public int? SHWProductID { get; set; }
        public int? ProductID { get; set; }
        public string IntegrationID { get; set; }
        public int? CourseID { get; set; }
        public int? SubscriptionID { get; set; }
        public int? CompanyID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("ProductID", ProductID);
            v.AssertNotNull("IntegrationID", IntegrationID);
            v.AssertNotNull("CourseID", CourseID);
            v.AssertNotNull("SubscriptionID", SubscriptionID);
            v.AssertNotNull("CompanyID", CompanyID);
            v.AssertString("IntegrationID", IntegrationID, 50);
        }
    }
}
