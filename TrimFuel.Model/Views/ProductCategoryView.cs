using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class ProductCategoryView : EntityView
    {
        public int? ProductCategoryID { get; set; }
        public string CategoryName { get; set; }
        public string ProductCode { get; set; }
    }
}
