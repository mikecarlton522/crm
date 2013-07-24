using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class WebStoreProductReview : Entity
    {
        public int? WebStoreProductReviewID { get; set; }
        public int? WebStoreProductID { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string SkinType { get; set; }
        public string SkinTone { get; set; }
        public string Sex { get; set; }
        public string AgeRange { get; set; }
        public string Rating { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Review { get; set; }
        public DateTime? CreateDT { get; set; }
        public bool Confirmed { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("DisplayName", DisplayName, 300);
            v.AssertString("Address", Address, 300);
            v.AssertString("Email", Email, 150);
        }
    }
}
