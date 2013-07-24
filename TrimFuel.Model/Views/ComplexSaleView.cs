using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class ComplexSaleView : EntityView
    {
        public AssertigyMID AssertigyMID { get; set; }
        public Registration Registration { get; set; }
        public Billing ParentBilling { get; set; }
        public ICoupon Coupon { get; set; }
        public Referer Referer { get; set; }
        public decimal? RefererCommissionRedeem { get; set; }
        public IList<PromoGift> PromoGiftList { get; set; }
        public List<Set<BillingSale, BillingSubscription, Subscription>> BillingSales { get; set; }
        public List<Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, Subscription>> BillingFreeSales { get; set; }
        public List<Set<UpsellSale, BillingSubscription, Upsell, UpsellType>> UpsellSales { get; set; }
        public List<Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, UpsellType>> UpsellFreeSales { get; set; }
    }
}
