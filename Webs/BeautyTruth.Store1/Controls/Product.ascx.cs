using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using BeautyTruth.Store1.Logic;

namespace BeautyTruth.Store1.Controls
{
    public partial class Product : System.Web.UI.UserControl
    {
        public string AdditionalClass { get; set; }
        protected string Title { get; set; }
        protected string Description { get; set; }
        public string ProductCode { get; set; }
        public string Photo { get; set; }
        public string LargePhoto { get; set; }

        private ShoppingCart ShoppingCart
        {
            get
            {
                return ShoppingCart.Current;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //    this.DataBind();
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            Photo = ShoppingCart.DASHBOARD_PATH + "images/empty.png";
            LargePhoto = ShoppingCart.DASHBOARD_PATH + "images/empty.png";
            Title = "&nbsp;";
            var product = new ProductService().GetProductCodeInfo(ProductCode);
            if (product != null)
            {
                Title = string.IsNullOrEmpty(product.Title) ? "&nbsp;" : product.Title;
                Description = product.Description;
                if (!string.IsNullOrEmpty(product.Photo))
                    Photo = ShoppingCart.PHOTO_PATH + product.Photo;
                if (!string.IsNullOrEmpty(product.LargePhoto))
                    LargePhoto = ShoppingCart.PHOTO_PATH + product.LargePhoto;
            }
        }
    }
}