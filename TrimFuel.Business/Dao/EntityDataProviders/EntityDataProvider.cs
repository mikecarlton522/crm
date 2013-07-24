using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data.Common;
using System.Data;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Views;
using TrimFuel.Business.Dao.EntityDataProviders.Views;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public abstract class EntityDataProvider<TEntity> where TEntity : Entity
    {
        public abstract void Save(TEntity entity, IMySqlCommandCreater cmdCreater);
        public abstract TEntity Load(object key, IMySqlCommandCreater cmdCreater);
        public abstract TEntity Load(DataRow row);

        public IList<TEntity> Load(MySqlCommand selectCommand)
        {
            MySqlDataAdapter adapt = new MySqlDataAdapter(selectCommand);
            DataTable dt = new DataTable();
            adapt.Fill(dt);

            return (from row in dt.Rows.Cast<DataRow>()
                    select Load(row)).ToList();
        }

        public static EntityDataProvider<TEntity> CreateProvider()
        {
            if (!providers.ContainsKey(typeof(TEntity)))
            {
                throw new Exception(string.Format("Can not find EntityDataProvider for {0}", typeof(TEntity).Name));
            }
            return (EntityDataProvider<TEntity>)providers[typeof(TEntity)];
        }

        public void Register()
        {
            if (providers.ContainsKey(typeof(TEntity)))
            {
                throw new Exception(string.Format("EntityDataProvider for {0} already registered", typeof(TEntity).Name));
            }
            providers.Add(typeof(TEntity), this);
        }

        public void UnRegister()
        {
            if (!providers.ContainsKey(typeof(TEntity)))
            {
                throw new Exception(string.Format("EntityDataProvider for {0} is not registered", typeof(TEntity).Name));
            }
            else if (providers[typeof(TEntity)].GetType() != this.GetType())
            {
                throw new Exception(string.Format("EntityDataProvider {0} is not registered for {0}", typeof(TEntity).Name));
            }

            providers.Remove(typeof(TEntity));
        }

        private static IDictionary<Type, object> providers = new Dictionary<Type, object>(){
            {typeof(StringView), new StringViewDataProvider()},
            {typeof(View<byte>), new ViewDataProvider<byte>()},
            {typeof(View<short>), new ViewDataProvider<short>()},
            {typeof(View<int>), new ViewDataProvider<int>()},
            {typeof(View<long>), new ViewDataProvider<long>()},
            {typeof(View<double>), new ViewDataProvider<double>()},
            {typeof(View<decimal>), new ViewDataProvider<decimal>()},
            {typeof(View<DateTime>), new ViewDataProvider<DateTime>()},
            {typeof(Registration), new RegistrationDataProvider()},
            {typeof(Billing), new BillingDataProvider()},
            {typeof(Subscription), new SubscriptionDataProvider()},
            {typeof(BillingSubscription), new BillingSubscriptionDataProvider()},
            {typeof(PagePixelView), new PagePixelViewDataProvider()},
            {typeof(USState), new USStateDataProvider()},
            {typeof(TempConversion), new TempConversationDataProvider()},
            {typeof(Coupon), new CouponDataProvider()},
            {typeof(Pixel), new PixelDataProvider()},
            {typeof(BillingSale), new BillingSaleDataProvider()},
            {typeof(UpsellSale), new UpsellSaleDataProvider()},
            {typeof(Sale), new SaleDataProvider()},
            {typeof(PartnerClick), new PartnerClickDataProvider()},
            {typeof(ChargeDetails), new ChargeDetailsDataProvider()},
            {typeof(AuthOnlyChargeDetails), new AuthOnlyChargeDetailsDataProvider()},
            {typeof(ChargeHistoryEx), new ChargeHistoryExDataProvider()},
            {typeof(MerchantAccount), new MerchantAccountDataProvider()},
            {typeof(MerchantAccountProduct), new MerchantAccountProductDataProvider()},
            {typeof(EmergencyQueue), new EmergencyQueueDataProvider()},
            {typeof(Paygea), new PaygeaDataProvider()},
            {typeof(FraudScore), new FraudScoreDataProvider()},
            {typeof(Notes), new NotesDataProvider()},
            {typeof(AssertigyMID), new AssertigyMIDDataProvider()},
            {typeof(SHWProduct), new SHWProductDataProvider()},
            {typeof(SHWRegistration), new SHWRegistrationDataProvider()},
            {typeof(GiftCertificateDynamicEmail), new GiftCertificateDynamicEmailDataProvider()},
            {typeof(DynamicEmail), new DynamicEmailDataProvider()},
            {typeof(Email), new EmailDataProvider()},
            {typeof(AuthOnlyFailedChargeDetails), new AuthOnlyFailedChargeDetailsDataProvider()},
            {typeof(FailedChargeHistory), new FaildChargeHistoryDataProvider()},
            {typeof(FailedChargeHistoryDetails), new FailedChargeHistoryDetailsDataProvider()},
            {typeof(ReturnsReportView), new ReturnsReportViewDataProvider()},
            {typeof(NMICompany), new NMICompanyDataProvider()},
            {typeof(AssertigyMIDDailyCap), new AssertigyMIDDailyCapDataProvider()},
            {typeof(BillingCancelCode), new BillingCancelCodeDataProvider()},
            {typeof(UpsellType), new UpsellTypeDataProvider()},
            {typeof(Upsell), new UpsellDataProvider()},
            {typeof(DeclineUpsell), new DeclineUpsellDataProvider()},
            {typeof(BillingLinked), new BillingLinkedDataProvider()},
            {typeof(SaleChargeback), new SaleChargebackDataProvider()},
            {typeof(ChargebackStatusType), new ChargebackStatusTypeDataProvider()},
            {typeof(ChargebackReasonCode), new ChargebackReasonCodeDataProvider()},
            {typeof(BillingBatchEmailType), new BillingBatchEmailTypeDataProvider()},
            {typeof(BillingBatchEmail), new BillingBatchEmailDataProvider()},
            {typeof(VoidQueue), new VoidQueueDataProvider()},
            {typeof(Tag), new TagDataProvider()},
            {typeof(TagGroup), new TagGroupDataProvider()},
            {typeof(TagGroupTagLink), new TagGroupTagLinkDataProvider()},
            {typeof(TagBillingLink), new TagBillingLinkDataProvider()},
            {typeof(Conversion), new ConversionDataProvider()},
            {typeof(SalesAgrReportView), new SalesAgrReportViewDataProvider()},
            {typeof(SalesAgrByAffReportView), new SalesAgrByAffReportViewDataProvider()},
            {typeof(SalesAgrByTypeReportView), new SalesAgrByTypeReportViewDataProvider()},
            {typeof(BillingBadCustomer), new BillingBadCustomerDataProvider()},
            {typeof(SaleCouponCode), new SaleCouponCodeDataProvider()},
            {typeof(BillingReferred), new BillingReferredDataProvider()},
            {typeof(CampaignReferralDiscount), new CampaignReferralDiscountDataProvider()},
            {typeof(BillingSubscriptionDiscount), new BillingSubscriptionDiscountDataProvider()},
            {typeof(Referer), new RefererDataProvider()},
            {typeof(SaleReferer), new SaleRefererDataProvider()},
            {typeof(RefererBilling), new RefererBillingDataProvider()},
            {typeof(Inventory), new InventoryDataProvider()},
            {typeof(Product), new ProductDataProvider()},
            {typeof(ProductCode), new ProductCodeDataProvider()},
            {typeof(ExtraTrialShipSale), new ExtraTrialShipSaleDataProvider()},
            {typeof(ExtraTrialShip), new ExtraTrialShipDataProvider()},
            {typeof(ExtraTrialShipType), new ExtraTrialShipTypeDataProvider()},
            {typeof(InventorySaleView), new InventorySaleViewDataProvider()},
            {typeof(RefererCommission), new RefererCommissionDataProvider()},
            {typeof(ABFRecord), new ABFRecordDataProvider()},
            {typeof(TFRecord), new TFRecordDataProvider()},
            {typeof(SaleShipperView), new SaleShipperViewDataProvider()},
            {typeof(InventoryView), new InventoryViewDataProvider()},
            {typeof(RefererView), new RefererViewDataProvider()},
            {typeof(KeymailRecord), new KeymailRecordDataProvider()},
            {typeof(KeymailRecordToSend), new KeymailRecordToSendDataProvider()},
            {typeof(Currency), new CurrencyDataProvider()},
            {typeof(FailedChargeHistoryCurrency), new FailedChargeHistoryCurrencyDataProvider()},
            {typeof(ChargeHistoryExCurrency), new ChargeHistoryExCurrencyDataProvider()},
            {typeof(SaleRefund), new SaleRefundDataProvider()},
            {typeof(CampaignRoutingView), new CampaignRoutingViewDataProvider()},
            {typeof(DynamicEmailType), new DynamicEmailTypeDataProvider()},
            {typeof(ProductEmailTypeView), new ProductEmailTypeViewDataProvider()},
            {typeof(RegistrationInfo), new RegistrationInfoDataProvider()},
            {typeof(Country), new CountryDataProvider()},
            {typeof(SaleBillingView), new SaleBillingViewDataProvider()},
            {typeof(BillingExternalInfo), new BillingExternalInfoDataProvider()},
            {typeof(TPClient), new TPClientDataProvider()},
            {typeof(AragonLead), new AragonLeadDataProvider()},
            {typeof(Admin), new AdminDataProvider()},
            {typeof(Call), new CallDataProvider()},
            {typeof(PromoGift), new PromoGiftDataProvider()},
            {typeof(PromoGiftSale), new PromoGiftSaleDataProvider()},
            {typeof(RefererCommissionSale), new RefererCommissionSaleDataProvider()},
            {typeof(Store), new StoreDataProvider()},
            {typeof(SaleChargeDetails), new SaleChargeDetailsDataProvider()},
            {typeof(SalesAgent), new SalesAgentProvider()},
            {typeof(ChargeHistoryExSale), new ChargeHistoryExSaleDataProvider()},
            {typeof(SalesAgentTPClient), new SalesAgentTPClientDataProvider()},
            {typeof(SaleDetails), new SaleDetailsDataProvider()},
            {typeof(ALFRecord), new ALFRecordDataProvider()},
            {typeof(MBRecord), new MBRecordDataProvider()},
            {typeof(AssertigyMIDGroup), new AssertigyMIDGroupDataProvider()},
            {typeof(MIDCategory), new MIDCategoryDataProvider()},
            {typeof(VeriSectPost), new VeriSectPostDataProvider()},
            {typeof(GFRecord), new GFRecordDataProvider()},
            {typeof(NPFRecord), new NPFRecordDataProvider()},
            {typeof(NPFRecordToSend), new NPFRecordToSendDataProvider()},
            {typeof(LeadPost), new LeadPostDataProvider()},
            {typeof(LeadRouting), new LeadRoutingDataProvider()},
            {typeof(LeadRoutingView), new LeadRoutingViewDataProvider()},
            {typeof(LeadPartner), new LeadPartnerDataProvider()},
            {typeof(ShortShipmentView), new ShortShipmentViewDataProvider()},
            {typeof(ReturnedSaleView), new ReturnedSaleViewDataProvider()},
            {typeof(ReturnedSale), new ReturnedSaleDataProvider()},
            {typeof(TPClientNoteView), new TPClientNoteViewDataProvider()},
            {typeof(TPClientEmailView), new TPClientEmailViewDataProvider()},
            {typeof(SaleRecordView), new SaleRecordViewDataProvider()},
            {typeof(AssertigyMIDSettings), new AssertigyMIDSettingsDataProvider()},
            {typeof(LeadType), new LeadTypeDataProvider()},
            {typeof(LeadPartnerConfigValue), new LeadPartnerConfigValueDataProvider()},
            {typeof(LeadPartnerSettings), new LeadPartnerSettingsDataProvider()},
            {typeof(ProductCurrency), new ProductCurrencyDataProvider()},
            {typeof(NMIMerchantAccountProduct), new NMIMerchantAccountProductDataProvider()},
            {typeof(PaymentType), new PaymentTypeDataProvider()},
            {typeof(Shipper), new ShipperDataProvider()},
            {typeof(ShipperProduct), new ShipperProductDataProvider()},
            {typeof(ShipperProductView), new ShipperProductViewDataProvider()},
            {typeof(ProductCodeInventory), new ProductCodeInventoryDataProvider()},
            {typeof(ShipperConfig), new ShipperConfigDataProvider()},
            {typeof(TPClientShipperSettings), new ShipperSettingsDataProvider()},
            {typeof(TPClientNote), new TPClientNoteDataProvider()},
            {typeof(TPClientEmail), new TPClientEmailDataProvider()},
            {typeof(TPClientSetting), new TPClientSettingDataProvider()},
            {typeof(Campaign), new CampaignDataProvider()},
            {typeof(CampaignPage), new CampaignPageDataProvider()},
            {typeof(CampaignControl), new CampaignControlDataProvider()},
            {typeof(CampaignUpsell), new CampaignUpsellDataProvider()},
            {typeof(CampaignDetailsView), new CampaignDetailsViewDataProvider()},
            {typeof(BillingAPIRequest), new BillingAPIRequestDataProvider()},
			{typeof(RestrictLevel), new RestrictLevelDataProvider()},
            {typeof(PageType), new PageTypeDataProvider()},
            {typeof(TermView), new TermViewDataProvider()},
            {typeof(SubscriptionPlan), new SubscriptionPlanDataProvider()},
            {typeof(SubscriptionPlanItem), new SubscriptionPlanItemDataProvider()},
            {typeof(SubscriptionPlanItemAction), new SubscriptionPlanItemActionDataProvider()},
            {typeof(TPClientNews), new TPClientNewsDataProvider()},
            {typeof(TPClientNewsRead), new TPClientNewsReadDataProvider()},
            {typeof(BillingSubscriptionPlan), new BillingSubscriptionPlanDataProvider()},
            {typeof(SubscriptionPlanItemActionView), new SubscriptionPlanItemActionViewDataProvider()},
            {typeof(MagentoConfig), new MagentoConfigDataProvider()},
            {typeof(ProductEvent), new ProductEventDataProvider()},
            {typeof(WebStore), new WebStoreDataProvider()},
            {typeof(ProductCategory), new ProductCategoryDataProvider()},
            {typeof(ProductCategoryView), new ProductCategoryViewDataProvider()},
            {typeof(WebStoreProduct), new WebStoreProductDataProvider()},
            {typeof(ProductCodeInfo), new ProductCodeInfoDataProvider()},
            {typeof(SMTPSetting), new SMTPSettingDataProvider()},
            {typeof(SalesAgrByReasonReportView), new SalesAgrByReasonReportViewDataProvider()},
            {typeof(MagentoProductCategoryView), new MagentoProductCategoryViewDataProvider()},
            {typeof(MagentoProductCategory), new MagentoProductCategoryDataProvider()},
            {typeof(ShippmentsToSendView), new ShippmentsToSendViewDataProvider()},
            {typeof(ErrorsLog), new ErrorsLogDataProvider()},
            {typeof(RecurringPlanView), new RecurringPlanViewDataProvider()},
            {typeof(RecurringPlan), new RecurringPlanDataProvider()},
            {typeof(ProductWiki), new ProductWikiDataProvider()},
            {typeof(UnsentUnpayedView), new UnsentUnpayedViewDataProvider()},
            {typeof(UnsentUnpayedExView), new UnsentUnpayedExViewDataProvider()},
            {typeof(RecurringPlanCycle), new RecurringPlanCycleDataProvider()},
            {typeof(RecurringPlanConstraint), new RecurringPlanConstraintDataProvider()},
            {typeof(UnpayedRebillsView), new UnpayedRebillsViewDataProvider()},
            {typeof(DailyBillingResults), new DailyBillingResultsDataProvider()},
            {typeof(WebStoreEmails), new WebStoreEmailsDataProvider()},
            {typeof(RecurringPlanShipment), new RecurringPlanShipmentDataProvider()},
            {typeof(WebStoreProductReview), new WebStoreProductReviewDataProvider()},
            {typeof(CampaignEmailTypeView), new CampaignEmailTypeViewDataProvider()},
            {typeof(CampaignLeadRouting), new CampaignLeadRoutingDataProvider()},
            {typeof(CampaignLeadRoutingView), new CampaignLeadRoutingViewDataProvider()},
            {typeof(CampaignAffiliate), new CampaignAffiliateDataProvider()},
            {typeof(QueueRebill), new QueueRebillDataProvider()},
            {typeof(AssertigyMIDCapView), new AssertigyMIDCapViewDataProvider()},
            {typeof(Order), new OrderDataProvider()},
            {typeof(Invoice), new InvoiceDataProvider()},
            {typeof(RecurringSale), new RecurringSaleDataProvider()},
            {typeof(OrderSale), new OrderSaleDataProvider()},
            {typeof(OrderRecurringPlan), new OrderRecurringPlanDataProvider()},
            {typeof(OrderProduct), new OrderProductDataProvider()},
            {typeof(Shipment), new ShipmentDataProvider()},
            {typeof(ShipperRequest), new ShipperRequestDataProvider()},
            {typeof(ShipmentShipperRequest), new ShipmentShipperRequestDataProvider()},
            {typeof(ChargeHistoryInvoice), new ChargeHistoryInvoiceDataProvider()},
            {typeof(ProductSKU), new ProductSKUDataProvider()},
            {typeof(Affiliate), new AffiliateDataProvider()},
            {typeof(GroupProductSKU), new GroupProductSKUViewDataProvider()},
            {typeof(ShippingNote), new ShippingNoteDataProvider()},
            {typeof(ShipmentShippingNote), new ShipmentShippingNoteDataProvider()},
            {typeof(ShippingEventView), new ShippingEventViewDataProvider()},
            {typeof(ChargeHistoryView), new ChargeHistoryViewDataProvider()},
            {typeof(SaleReturnProcessing), new SaleReturnProcessingDataProvider()},
            {typeof(CampaignRecurringPlan), new CampaignRecurringPlanDataProvider()},
            {typeof(ProspectView), new ProspectViewDataProvider()},
            {typeof(CampaignTrialProduct), new CampaignTrialProductDataProvider()},
            {typeof(AgrMIDProjectedRevenue), new AgrMIDProjectedRevenueDataProvider()},
            {typeof(AssertigyMIDBriefView), new AssertigyMIDBriefViewDataProvider()},
            {typeof(AssertigyMIDAmountView), new AssertigyMIDAmountViewDataProvider()},
            {typeof(ProductInventory), new ProductInventoryDataProvider()},
            {typeof(ChargeHistoryCard), new ChargeHistoryCardDataProvider()},
            {typeof(EmailQueue), new EmailQueueDataProvider()},
            {typeof(IPRestriction), new IPRestrictionDataProvider()},
            {typeof(OrderAutoProcessing), new OrderAutoProcessingDataProvider()},
            {typeof(InventorySKU), new InventorySKUDataProvider()},
            {typeof(OrderSaleBillingView), new OrderSaleBillingViewDataProvider()},
            {typeof(LeadPartnerAffiliateView), new LeadPartnerAffiliateViewDataProvider()},
            {typeof(OutboundSalesView), new OutboundSalesViewDataProvider()},
            {typeof(Job), new JobDataProvider()},
            {typeof(Geo), new GeoDataProvider()},
            {typeof(ClosedMIDRouting), new ClosedMIDRoutingDataProvider()},
            {typeof(ClosedMIDQueue), new ClosedMIDQueueDataProvider()},
            {typeof(AssertigyMIDPaymentType), new AssertigyMIDPaymentTypeDataProvider()},
            {typeof(NMICompanyMID), new NMICompanyMIDDataProvider()},
            {typeof(AssertigyMIDCapView2), new AssertigyMIDCapView2DataProvider()},
            {typeof(OrderRecurringPlanQueueNote), new OrderRecurringPlanQueueNoteDataProvider()},
            {typeof(AggUnsentShipments), new AggUnsentShipmentsDataProvider()},
            {typeof(AggShipperSale), new AggShipperSaleDataProvider()},
            {typeof(SaleShippingOption), new SaleShippingOptionDataProvider()},
            {typeof(ProductProductCode), new ProductProductCodeDataProvider()},
            {typeof(CustomShipperRecord), new CustomShipperRecordDataProvider()},
	        {typeof(ProductRouting), new ProductRoutingDataProvider()},
            {typeof(ProductDomain), new ProductDomainDataProvider()},
            {typeof(ProductDomainRouting), new ProductDomainRoutingDataProvider()},
            {typeof(CustomShipperRecordToSend), new CustomShipperRecordToSendDataProvider()},
            {typeof(BlockedEmailDomain), new BlockedEmailDomainDataProvider()},
            {typeof(BillingSubscriptionRebillDiscount), new BillingSubscriptionRebillDiscountDataProvider()},
            {typeof(RebillDiscountType), new RebillDiscountTypeDataProvider()},
            {typeof(BillingCallView), new BillingCallViewDataProvider()},
            {typeof(IgnoreUnbilledTransaction), new IgnoreUnbilledTransactionDataProvider()},
            {typeof(BillingStopCharge), new BillingStopChargeDataProvider()},
            {typeof(SubscriptionRecurringPlanView), new SubscriptionRecurringPlanViewDataProvider()},
            {typeof(BillingSubscriptionStatusHistory), new BillingSubscriptionStatusHistoryDataProvider()},
            {typeof(OrderRecurringPlanStatusHistory), new OrderRecurringPlanStatusHistoryDataProvider()},
            {typeof(OrderQueueNote), new OrderQueueNoteDataProvider()},
            {typeof(CustomShipperToSendView), new CustomShipperToSendViewDataProvider()},
            {typeof(ShippmentNotSendedToSendView), new ShippmentNotSendedToSendViewDataProvider()}
        };
    }
}
