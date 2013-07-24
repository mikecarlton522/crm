using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Web.Admin.Logic;

namespace TrimFuel.Web.Admin
{
    public partial class management_coupons : PageX
    {
        private CouponService _cs = new CouponService();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void DoCouponAction(Object sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                int couponID = int.TryParse(e.CommandArgument as string, out couponID) ? couponID : -1;

                if(couponID != -1)
                    _cs.DeleteCoupon(couponID);

                base.DataBind();
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            IList<Coupon> coupons = _cs.GetCoupons();

            rCoupons.DataSource = coupons;
        }

        public override string HeaderString
        {
            get { return "Coupon Management"; }
        }
    }
}