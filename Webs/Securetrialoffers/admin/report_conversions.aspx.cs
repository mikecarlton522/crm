using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Securetrialoffers.admin.Logic.UI;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using TrimFuel.Model.Enums;

namespace Securetrialoffers.admin
{
    public partial class report_conversions : ReportBasePage
    {
        protected struct ConversionGrouping
        {
            public string Affiliate { get; set; }
            public string SubAffiliate { get; set; }
            public DateTime CreateDT { get; set; }
        }

        private PageService pageService = new PageService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DataBind();
            }
        }

        public override string ReportName
        {
            get { return "Conversion Statistics"; }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            IList<Conversion> conversions = pageService.GetStatusticsIndependentOnCompany(StartDate, EndDate);
            IEnumerable<IGrouping<ConversionGrouping, Conversion>> conversionsGroups = conversions.GroupBy(c => new ConversionGrouping()
                {
                    Affiliate = c.Affiliate,
                    SubAffiliate = c.SubAffiliate,
                    CreateDT = new DateTime(c.CreateDT.Value.Year, c.CreateDT.Value.Month, c.CreateDT.Value.Day, (c.Hour != null) ? c.Hour.Value : 0, 0, 0)
                });

            List<Set<View<ConversionGrouping>, View<int>, View<int>, View<int>>> ds = new List<Set<View<ConversionGrouping>, View<int>, View<int>, View<int>>>();
            foreach (IGrouping<ConversionGrouping, Conversion> conversionGroup in conversionsGroups)
            {
                Set<View<ConversionGrouping>, View<int>, View<int>, View<int>> view = new Set<View<ConversionGrouping>, View<int>, View<int>, View<int>>();
                view.Value1 = new View<ConversionGrouping>();
                view.Value1.Value = conversionGroup.Key;

                IEnumerable<Conversion> landingHits = conversionGroup.Where(c => c.PageTypeID == PageTypeEnum.Landing);
                view.Value2 = new View<int>() { Value = (landingHits.Count() > 0) ? landingHits.Sum(c => c.Hits.Value) : 0 };

                IEnumerable<Conversion> billingHits = conversionGroup.Where(c => c.PageTypeID == PageTypeEnum.Billing);
                view.Value3 = new View<int>() { Value = (billingHits.Count() > 0) ? billingHits.Sum(c => c.Hits.Value) : 0 };

                IEnumerable<Conversion> upsellHits = conversionGroup.Where(c => c.PageTypeID == PageTypeEnum.Upsell_1);
                view.Value4 = new View<int>() { Value = (upsellHits.Count() > 0) ? upsellHits.Sum(c => c.Hits.Value) : 0 };

                ds.Add(view);
            }


            rConversions.DataSource = ds
                .OrderBy(view => view.Value1.Value.Value.CreateDT)
                .OrderBy(view => view.Value1.Value.Value.SubAffiliate)                
                .OrderBy(view => view.Value1.Value.Value.Affiliate)                
                .ToList();
        }        
    }
}
