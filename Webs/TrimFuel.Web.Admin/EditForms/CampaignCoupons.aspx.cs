using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

using TrimFuel.Business;
using TrimFuel.Business.Utils;
using TrimFuel.Model;

namespace TrimFuel.Web.Admin.EditForms
{
    public partial class CampaignCoupons : System.Web.UI.Page
    {
        private CouponService _cs = new CouponService();
        private ProductService _ps = new ProductService();

        private int _campaignID = 0;

        protected Campaign Campaign;

        protected Coupon Coupon;

        private IList<Coupon> _coupons = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            _campaignID = int.TryParse(Request["id"], out _campaignID) ? _campaignID : -1;

            Campaign = (new CampaignService()).GetCampaignByID(_campaignID);

            if (!IsPostBack)
            {
                _coupons = _cs.GetCoupons();

                FillCoupons(_coupons);

                rProducts.DataSource = _ps.GetProductList();
                rProducts.DataBind();

                if (_campaignID > 0)
                    FillPage(_campaignID, _coupons);
            }
        }

        protected void SaveCampaignCoupons(object sender, EventArgs e)
        {
            int campaignID = int.TryParse(Request["campaignid"], out _campaignID) ? _campaignID : -1;

            _cs.DeleteCampaignCoupons(campaignID);

            string[] arrCouponID = Request["coupon"].Split(',');

            foreach (string strCouponID in arrCouponID)
            {
                int couponID = int.TryParse(strCouponID, out couponID) ? couponID : -1;

                if (couponID > 0)
                    _cs.CreateCampaignCoupon(campaignID, couponID);
            }

            _coupons = _cs.GetCoupons();

            FillCoupons(_coupons);

            FillPage(_campaignID, _coupons);

            ltManageCouponsConfirm.Text = "Coupons saved!";
        }

        protected void SaveCoupon(object sender, EventArgs e)
        {
            int campaignID = int.TryParse(Request["campaignID"], out _campaignID) ? _campaignID : -1;

            int couponID = int.TryParse(Request["couponID"], out couponID) ? couponID : -1;

            decimal? discount = Request["couponType"] == "discount" ? Utility.TryGetDecimal(Request["discount"]) : null;

            decimal? price = Request["couponType"] == "price" ? Utility.TryGetDecimal(Request["newPrice"]) : null;

            BusinessError<Coupon> res = null;

            if (couponID > 0)
                res = _cs.UpdateCoupon(couponID, Utility.TryGetInt(Request["productID"]), Utility.TryGetStr(Request["code"]), discount, price);

            else
                res = _cs.CreateCoupon(Utility.TryGetInt(Request["productID"]), Utility.TryGetStr(Request["code"]), discount, price);

            if (res.State == BusinessErrorState.Success)
            {
                _cs.CreateCampaignCoupon(campaignID, (int)res.ReturnValue.CouponID);

                ltCreateCouponConfirm.Text = "Coupon saved!";
            }
            else
            {
                ltCreateCouponConfirm.Text = res.ErrorMessage;
            }

            _coupons = _cs.GetCoupons();

            FillCoupons(_coupons);

            FillPage(campaignID, _coupons);

            ph1.Visible = false;
            ph2.Visible = true;
        }

        protected void CancelSaveCoupon(object sender, EventArgs e)
        {
            ph1.Visible = false;
            ph2.Visible = true;
        }

        private void FillPage(int campaignID, IList<Coupon> coupons)
        {
            IList<Coupon> campaignCoupons = _cs.GetCampaignCoupons(campaignID);

            StringBuilder html = new StringBuilder();

            html.AppendLine(CouponDropDownList(-1, coupons));

            if (campaignCoupons.Count > 0)
            {
                foreach (Coupon coupon in campaignCoupons)
                {
                    string dropDownList = CouponDropDownList((int)coupon.CouponID, coupons);

                    html.AppendLine(dropDownList);
                }
            }

            ltCoupons.Text = html.ToString();
        }

        private void FillCoupons(IList<Coupon> coupons)
        {
            rCoupons.DataSource = coupons;
            rCoupons.DataBind();

            rCouponTable.DataSource = coupons;
            rCouponTable.DataBind();
        }

        private string CouponDropDownList(int selectedCouponID, IList<Coupon> coupons)
        {
            StringBuilder html = new StringBuilder();

            html.AppendLine("<div>");
            html.AppendLine("<select name=\"coupon\" style=\"float: left; margin-bottom:4px;\">");
            html.AppendLine("<option>-- Select --</option>");
            foreach (Coupon coupon in coupons)
            {
                bool selected = selectedCouponID == coupon.CouponID;

                html.AppendFormat("<option value=\"{0}\"{1}>{2} ({3})</option>", coupon.CouponID, selected ? " selected" : string.Empty, coupon.Code, CurrencyOrPercentage(coupon.Discount, coupon.NewPrice));
                html.AppendLine();
            }
            html.AppendLine("</select>");
            html.AppendLine("<a href=\"#\" onclick=\"return removeCoupon(this);\" class=\"removeIcon\" style=\"float:right;\">Remove</a>");
            html.AppendLine("<div class=\"clear\">");
            html.AppendLine("</div>");
            html.AppendLine("</div>");

            return html.ToString();
        }

        protected string CurrencyOrPercentage(object discount, object newPrice)
        {
            if (discount == null)
                return "$" + newPrice.ToString();

            return discount.ToString() + "%";
        }

        protected void DoCouponAction(object sender, CommandEventArgs e)
        {
            int campaignID = int.TryParse(Request["campaignID"], out _campaignID) ? _campaignID : -1;

            int couponID = 0;

            if (e.CommandName == "create")
            {
                ph1.Visible = true;
                ph2.Visible = false;
            }

            if (e.CommandName == "edit")
            {
                couponID = Convert.ToInt32(e.CommandArgument);

                Coupon = _cs.Load<Coupon>(couponID);

                ph1.Visible = true;
                ph2.Visible = false;
            }

            if (e.CommandName == "delete")
            {
                couponID = Convert.ToInt32(e.CommandArgument);                

                _cs.DeleteCoupon(couponID);

                _coupons = _cs.GetCoupons();

                FillCoupons(_coupons);

                FillPage(campaignID, _coupons);
            }
        }
    }


}