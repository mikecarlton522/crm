using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Model;
using TrimFuel.Business;

namespace BeautyTruth.Store1.Controls
{
    public partial class ProductReviews : System.Web.UI.UserControl
    {
        public int? WebStoreProductID { get; set; }

        WebStoreService service = new WebStoreService();
        List<WebStoreProductReview> _productReviewList = null;
        protected List<WebStoreProductReview> ProductReviewList
        {
            get
            {
                if (_productReviewList == null)
                    _productReviewList = service.GetWebStoreProductReviewsByWebStoreProductID(WebStoreProductID).Where(u => u.Confirmed == true).ToList();
                return _productReviewList;
            }
            set
            {
                _productReviewList = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                WebStoreProductID = Utility.TryGetInt(hdnWebStoreProductID.Value);
            else
                DataBind();
        }

        protected void dpdSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (dpdSort.SelectedValue)
            {
                case "2":
                    {
                        ProductReviewList = ProductReviewList.OrderByDescending(u => Utility.TryGetInt(u.Rating)).ToList();
                        break;
                    }
                case "3":
                    {
                        ProductReviewList = ProductReviewList.OrderBy(u => Utility.TryGetInt(u.Rating)).ToList();
                        break;
                    }
                case "4":
                    {
                        ProductReviewList = ProductReviewList.OrderByDescending(u => u.CreateDT).ToList();
                        break;
                    }
                case "5":
                    {
                        ProductReviewList = ProductReviewList.OrderBy(u => u.CreateDT).ToList();
                        break;
                    }
                case "6":
                    {
                        ProductReviewList = ProductReviewList.OrderByDescending(u => u.Review.Length).ToList();
                        break;
                    }
                case "7":
                    {
                        ProductReviewList = ProductReviewList.OrderBy(u => u.Review.Length).ToList();
                        break;
                    }
            }
            DataBind();
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            ddlAge.SelectedIndex = 0;
            ddlRating.SelectedIndex = 0;
            ddlSex.SelectedIndex = 0;
            ddlSkinTone.SelectedIndex = 0;
            ddlSkinType.SelectedIndex = 0;

            txtAddress.Text = string.Empty;
            txtTitle.Text = string.Empty;
            txtSummary.Text = string.Empty;
            txtReview.Text = string.Empty;
            txtDisplayName.Text = string.Empty;
            txtEmail.Text = string.Empty;
        }

        protected void sbmt_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                WebStoreProductReview review = new WebStoreProductReview()
                {
                    WebStoreProductID = WebStoreProductID,
                    Address = txtAddress.Text,
                    Title = txtTitle.Text,
                    Summary = txtSummary.Text,
                    SkinType = ddlSkinType.Value,
                    SkinTone = ddlSkinTone.Value,
                    Sex = ddlSex.Value,
                    Review = txtReview.Text,
                    Rating = ddlRating.Value,
                    Email = txtEmail.Text,
                    DisplayName = txtDisplayName.Text,
                    AgeRange = ddlAge.Value,
                    CreateDT = DateTime.Now,
                    Confirmed = false
                };

                service.SaveWebStoreProductReview(review);
                phReviewSubmited.Visible = true;
                phSubmitReview.Visible = false;
                phCaptchaError.Visible = false;
                DataBind();
            }
            if (!recaptcha.IsValid)
                phCaptchaError.Visible = true;
        }
    }
}