using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ProductCodeInfo : ProductCode
    {
        public string Description { get; set; }
        public string SalesContext { get; set; }
        public string Photo { get; set; }
        public string LargePhoto { get; set; }
        public decimal? RetailPrice { get; set; }
        public string Title { get; set; }
        public bool? FeaturedProduct { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("ProductCodeID", ProductCodeID);
        }

        public void FillFromProductCode(ProductCode pr)
        {
            ProductCodeID = pr.ProductCodeID;
            ProductCode_ = pr.ProductCode_;
            Name = pr.Name;
        }
    }
}
