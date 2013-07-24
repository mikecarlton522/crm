using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using TrimFuel.Business.Utils;
using TrimFuel.Business;
using TrimFuel.Web.UI;

namespace TrimFuel.Web.Admin.Controls
{
    public partial class VIPPlan : ModelDataControl
    {
        SubscriptionPlanService service = new SubscriptionPlanService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                If5.Condition = false;
            }
        }

        public IList<SubscriptionPlanItem> AllPlanItems { get; set; }
        IList<SubscriptionPlanItemActionView> allPlanItemActions = null;
        public IList<SubscriptionPlanItemActionView> AllPlanItemActions 
        {
            get { return allPlanItemActions; }
            set { allPlanItemActions = value; }
        }

        public SubscriptionPlan Plan { get; set; }
        public IDictionary<SubscriptionPlanItem, IList<SubscriptionPlanItemActionView>> PlanSingleItems { get; set; }
        public IDictionary<SubscriptionPlanItem, IList<SubscriptionPlanItemActionView>> PlanRecurringItems { get; set; }

        private void FillPlanItems()
        {
            if (Plan == null)
            {
                Plan = new SubscriptionPlan();
            }

            IDictionary<SubscriptionPlanItem, IList<SubscriptionPlanItemActionView>> res = new Dictionary<SubscriptionPlanItem, IList<SubscriptionPlanItemActionView>>();
            IDictionary<SubscriptionPlanItem, IList<SubscriptionPlanItemActionView>> recurringRes = new Dictionary<SubscriptionPlanItem, IList<SubscriptionPlanItemActionView>>();
            SubscriptionPlanItem cur = null;
            if (Plan.StartSubscriptionPlanItemID != null)
            {
                cur = AllPlanItems.Single(i => i.SubscriptionPlanItemID == Plan.StartSubscriptionPlanItemID);
            }
            while (cur != null && !res.ContainsKey(cur))
            {
                res.Add(ItemToItemWithActions(cur));
                if (cur.NextSubscriptionPlanItemID != null)
                {
                    cur = AllPlanItems.Single(i => i.SubscriptionPlanItemID == cur.NextSubscriptionPlanItemID);
                }
                else
                {
                    cur = null;
                }
            }
            while (cur != null)
            {
                recurringRes.Add(cur, res[cur]);
                res.Remove(cur);
                cur = res.Keys.FirstOrDefault(i => i.SubscriptionPlanItemID == cur.NextSubscriptionPlanItemID);
            }
            PlanSingleItems = res;
            PlanRecurringItems = recurringRes;
        }

        protected void RecurringItemControl_RequireData(object sender, EventArgs e)
        {
            SubscriptionPlanItemControl recurringItem = (SubscriptionPlanItemControl)sender;
            RepeaterItem rpItem = (RepeaterItem)recurringItem.Parent;
            recurringItem.PlanItem = PlanRecurringItems.ToList()[rpItem.ItemIndex].Key;
            recurringItem.PlanItemAction = PlanRecurringItems.ToList()[rpItem.ItemIndex].Value[0];
            (rpItem.FindControl("IfView") as If).Condition = (recurringItem.ViewMode == ViewModeEnum.View && ViewMode != ViewModeEnum.NotEditable);
            (rpItem.FindControl("IfEdit") as If).Condition = (recurringItem.ViewMode == ViewModeEnum.Edit && ViewMode != ViewModeEnum.NotEditable);
        }

        private KeyValuePair<SubscriptionPlanItem, IList<SubscriptionPlanItemActionView>> ItemToItemWithActions(SubscriptionPlanItem item)
        {
            return new KeyValuePair<SubscriptionPlanItem, IList<SubscriptionPlanItemActionView>>(item, AllPlanItemActions.Where(i => i.SubscriptionPlanItemID == item.SubscriptionPlanItemID).ToList());
        }

        protected override void SetData()
        {
            FillPlanItems();

            If0.Condition = (ViewMode == ViewModeEnum.Edit || Plan != null && Plan.SubscriptionPlanID != null);
            If1.Condition = (ViewMode == ViewModeEnum.View || ViewMode == ViewModeEnum.NotEditable);
            If2.Condition = (ViewMode == ViewModeEnum.Edit);
            If3.Condition = (Plan != null && Plan.SubscriptionPlanID != null);
            
            if (PlanSingleItems.Count == 0)
            {
                SubscriptionPlanItemControl1.PlanItem = new SubscriptionPlanItem() { 
                    Interim = 30
                };
                SubscriptionPlanItemControl1.PlanItemAction = new SubscriptionPlanItemActionView()
                {
                    SubscriptionActionTypeID = TrimFuel.Model.Enums.SubscriptionActionType.Upsell,
                    SubscriptionActionQuantity = 1,
                    SubscriptionActionAmount = 0M                    
                };
            }
            else
            {
                SubscriptionPlanItemControl1.PlanItem = PlanSingleItems.First().Key;
                SubscriptionPlanItemControl1.PlanItemAction = PlanSingleItems.FirstOrDefault().Value.First();
            }
            SubscriptionPlanItemControl1.ViewMode = ViewMode;

            rpRecurringItems.DataSource = PlanRecurringItems;

            SubscriptionPlanItemControl2.ViewMode = ViewModeEnum.Edit;
            SubscriptionPlanItemControl2.PlanItem = new SubscriptionPlanItem()
            {
                Interim = 30
            };
            SubscriptionPlanItemControl2.PlanItemAction = new SubscriptionPlanItemActionView()
            {
                SubscriptionActionTypeID = TrimFuel.Model.Enums.SubscriptionActionType.FreeProduct,
                SubscriptionActionQuantity = 1                
            };
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            ViewMode = ViewModeEnum.Edit;
            DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            LoadData();
            var res = service.UpdateVIPPlan(Plan, 
                new KeyValuePair<SubscriptionPlanItem,SubscriptionPlanItemActionView>(
                    PlanSingleItems.First().Key,
                    PlanSingleItems.First().Value[0]
                    ));
            if (res != null)
            {
                Plan = res;
                ViewMode = ViewModeEnum.View;
                UpdateAllItems();
                DataBind();
            }
        }

        private void UpdateAllItems()
        {
            AllPlanItems = service.GetSubscriptionPlanItems();
            AllPlanItemActions = service.GetSubscriptionPlanItemActions();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ViewMode = ViewModeEnum.View;
            DataBind();
        }

        protected void btnSaveNew_Click(object sender, EventArgs e)
        {
            LoadData();
            var res = service.AddRecurringItemVIPPlan(Plan,
                new KeyValuePair<SubscriptionPlanItem, SubscriptionPlanItemActionView>(
                    SubscriptionPlanItemControl2.PlanItem,
                    SubscriptionPlanItemControl2.PlanItemAction
                    ));
            if (res != null)
            {
                UpdateAllItems();
                If5.Condition = false;
                DataBind();
            }
        }

        protected void btnCancelExisted_Click(object sender, EventArgs e)
        {
            RepeaterItem rpItem = (RepeaterItem)((Control)sender).Parent.Parent;
            SubscriptionPlanItemControl recurringItem = (SubscriptionPlanItemControl)rpItem.FindControl("SubscriptionPlanItemControl1");
            recurringItem.ViewMode = ViewModeEnum.View;
            rpItem.DataBind();
        }

        protected void btnSaveExisted_Click(object sender, EventArgs e)
        {
            RepeaterItem rpItem = (RepeaterItem)((Control)sender).Parent.Parent;
            SubscriptionPlanItemControl recurringItem = (SubscriptionPlanItemControl)rpItem.FindControl("SubscriptionPlanItemControl1");
            recurringItem.LoadData();

            if (service.UpdateRecurringItemVIPPlan(
                new KeyValuePair<SubscriptionPlanItem, SubscriptionPlanItemActionView>(
                    recurringItem.PlanItem, 
                    recurringItem.PlanItemAction)) != null)
            {
                UpdateAllItems();
                FillPlanItems();
                recurringItem.ViewMode = ViewModeEnum.View;
                rpItem.DataBind();
            }
        }

        protected void btnEditExisted_Click(object sender, EventArgs e)
        {
            RepeaterItem rpItem = (RepeaterItem)((Control)sender).Parent.Parent;
            SubscriptionPlanItemControl recurringItem = (SubscriptionPlanItemControl)rpItem.FindControl("SubscriptionPlanItemControl1");
            recurringItem.ViewMode = ViewModeEnum.Edit;
            rpItem.DataBind();
        }

        protected void btnRemoveExisted_Click(object sender, EventArgs e)
        {
        }

        protected void btnCancelNew_Click(object sender, EventArgs e)
        {
            If5.Condition = false;
        }

        protected void btnAddRecurring_Click(object sender, EventArgs e)
        {
            If5.Condition = true;
            If5.DataBind();
        }

        protected override void GetData()
        {            
            Plan.SubscriptionPlanName = Utility.TryGetStr(tbName.Text);
            PlanSingleItems.Clear();
            PlanSingleItems.Add(new KeyValuePair<SubscriptionPlanItem, IList<SubscriptionPlanItemActionView>>(
                SubscriptionPlanItemControl1.PlanItem,
                new List<SubscriptionPlanItemActionView>() { SubscriptionPlanItemControl1.PlanItemAction }));
        }

        protected string CancelValidationJavascript
        {
            get 
            {
                return string.Format("$(\"#vip-plan-container-{0} form\").addClass(\"donotvalidate\");", (Plan != null && Plan.SubscriptionPlanID != null ? Plan.SubscriptionPlanID.ToString() : ""));
            }
        }
    }
}