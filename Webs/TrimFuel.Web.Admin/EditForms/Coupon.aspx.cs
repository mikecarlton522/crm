using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.EditForms
{
    public partial class Coupon_ : System.Web.UI.Page
    {
        private CouponService _cs = new CouponService();
        private ProductService _ps = new ProductService();

        protected void Page_Load(object sender, EventArgs e)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(Request["id"]) && int.TryParse(Request["id"], out id))
            {
                Coupon = _cs.Load<Coupon>(id);
            }
            else
            {
                Coupon = new Coupon();
            }

            rProducts.DataSource = _ps.GetProductList();
            rProducts.DataBind();

            if (!string.IsNullOrEmpty(Request["action"]))
            {
                Save();
            }
        }

        public Coupon Coupon { get; set; }

        private void Save()
        {
            if (Coupon.CouponID != null)
            {
                BusinessError<Coupon> updated = _cs.UpdateCoupon((int)Coupon.CouponID,
                    Utility.TryGetInt(Request["productID"]), Utility.TryGetStr(Request["code"]),
                    Utility.TryGetDecimal(Request["discount"]), Utility.TryGetDecimal(Request["newPrice"]));
                if (updated.State == BusinessErrorState.Success)
                {
                    Coupon = updated.ReturnValue;
                }
                else
                {
                    //Show error
                    //updated.ErrorMessage
                }
            }
            else
            {
                BusinessError<Coupon> created = _cs.CreateCoupon(Utility.TryGetInt(Request["productID"]), Utility.TryGetStr(Request["code"]),
                    Utility.TryGetDecimal(Request["discount"]), Utility.TryGetDecimal(Request["newPrice"]));
                if (created.State == BusinessErrorState.Success)
                {
                    Coupon = created.ReturnValue;
                }
                else
                {
                    //Show error
                    //updated.ErrorMessage
                }
            }
        }
    }
}
