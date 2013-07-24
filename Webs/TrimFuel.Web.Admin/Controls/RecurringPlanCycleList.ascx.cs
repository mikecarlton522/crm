using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Model.Views;

namespace TrimFuel.Web.Admin.Controls
{
    public partial class RecurringPlanCycleList : System.Web.UI.UserControl
    {
        SubscriptionNewService service = new SubscriptionNewService();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public int? RecurringPlanID { get; set; }

        public int? ProposedProductID { get; set; }

        public IList<RecurringPlanCycleView> CycleList { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            CycleList = service.GetRecurringPlanCycles(RecurringPlanID);

            rPlanCycleList.DataSource = CycleList;
            RecurringPlanCycleNew.PlanCycle = new TrimFuel.Model.Views.RecurringPlanCycleView()
            {
                Cycle = new RecurringPlanCycle()
                {
                    Cycle = null,
                    Interim = 30,
                    RetryInterim = 7,                    
                },
                Constraint = null,
                ShipmentList = new List<RecurringPlanShipment>()
            };
        }

        protected bool IsLast(int cycle)
        {
            return (cycle == CycleList.Last().Cycle.Cycle);
        }

        protected bool IsFirstRecurring(int cycle)
        {
            if (CycleList.FirstOrDefault(i => i.Cycle.Recurring.Value) == null)
                return false;
            return (cycle == CycleList.First(i => i.Cycle.Recurring.Value).Cycle.Cycle);
        }
    }
}