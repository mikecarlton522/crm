using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class GiftCertificateDynamicEmail : DynamicEmail
    {
        public int? StoreID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            base.ValidateFields(v);

            v.AssertNotNull("StoreID", StoreID);
        }

        #region Logic

        public void FillFromDynamicEmail(DynamicEmail dynamicEmail)
        {
            this.DynamicEmailID = dynamicEmail.DynamicEmailID;
            this.ProductID = dynamicEmail.ProductID;
            this.Days = dynamicEmail.Days;
            this.Content = dynamicEmail.Content;
            this.Landing = dynamicEmail.Landing;
            this.FromName = dynamicEmail.FromName;
            this.FromAddress = dynamicEmail.FromAddress;
            this.Subject = dynamicEmail.Subject;
            this.Active = dynamicEmail.Active;
            this.DynamicEmailTypeID = dynamicEmail.DynamicEmailTypeID;
            this.LandingLink = dynamicEmail.LandingLink;
        }

        #endregion
    }
}
