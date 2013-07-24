using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Business;
using TrimFuel.Model.Views;
using TrimFuel.Model;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class RecurringPlanCycleEdit : System.Web.UI.Page
    {
        SubscriptionNewService service = new SubscriptionNewService();
        //InventoryService invService = new InventoryService();

        protected void Page_Load(object sender, EventArgs e)
        {
            phError.Visible = false;
            phSuccess.Visible = false;

            IsRemoved = false;

            if (IsPostBack)
            {
                SelectedInventoryList = new List<KeyValuePair<string, int>>();
                //Response.Write(Request["inventory"] + "<br/>");
                //Response.Write(Request["quantity"] + "<br/>");
                if (Request["inventory"] != null)
                {
                    string[] inventory = Request["inventory"].Split(new string[] { "," }, StringSplitOptions.None);
                    string[] quantity = Request["quantity"].Split(new string[] { "," }, StringSplitOptions.None);
                    for (int i = 1; i < inventory.Length; i++)
                    {
                        SelectedInventoryList.Add(new KeyValuePair<string, int>(inventory[i], Utility.TryGetInt(quantity[i]).Value));
                    }
                }
            }
            else
            {
                DataBind();
            }
        }

        public int? RecurringPlanCycleID
        {
            get { return Utility.TryGetInt(hdnRecurringPlanCycleID.Value) ?? Utility.TryGetInt(Request["RecurringPlanCycleID"]); }
            set { hdnRecurringPlanCycleID.Value = value.ToString(); }
        }

        public int? RecurringPlanID
        {
            get { return Utility.TryGetInt(Request["RecurringPlanID"]); }
        }

        private RecurringPlanCycleView planCycle = null;
        public RecurringPlanCycleView PlanCycle
        {
            get
            {
                if (planCycle == null)
                {
                    if (RecurringPlanCycleID != null)
                    {
                        planCycle = service.GetRecurringPlanCycle(RecurringPlanCycleID);
                    }
                    else
                    {
                        planCycle = new RecurringPlanCycleView()
                        {
                            Cycle = new RecurringPlanCycle() { 
                                Cycle = null,
                                Interim = 30,
                                RetryInterim = 7,
                                RecurringPlanID = RecurringPlanID,
                            },
                            ShipmentList = new List<RecurringPlanShipment>()
                        };
                    }
                }
                return planCycle;
            }
            set
            {
                planCycle = value;
            }
        }

        public IList<KeyValuePair<string, int>> SelectedInventoryList { get; set; }

        private IList<ProductSKU> inventoryList = null;
        protected string InventoryOptionList(string selected)
        {
            if (inventoryList == null)
            {
                inventoryList = service.GetProductList();
            }

            string res = "";
            foreach (var item in inventoryList)
            {
                string isSelected = (selected == item.ProductSKU_ ? " selected" : "");
                res += "<option value='" + item.ProductSKU_ + "'" + isSelected + ">" + item.ProductSKU_ + "</option>";
            }
            return res;
        }

        public bool IsRemoved { get; set; }
        
        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            if (!IsRemoved)
            {
                if (SelectedInventoryList == null)
                {
                    SelectedInventoryList = PlanCycle.ShipmentList.Select(i => new KeyValuePair<string, int>(i.ProductSKU, i.Quantity.Value)).ToList();
                }
                if (PlanCycle.Constraint != null)
                {
                    ddlChargeType.SelectedValue = PlanCycle.Constraint.ChargeTypeID.ToString();
                    tbAmount.Text = PlanCycle.Constraint.Amount.ToString();
                }
                else
                {
                    ddlChargeType.SelectedValue = "";
                }
                tbInterim.Text = PlanCycle.Cycle.Interim.ToString();
                ddlShipping.SelectedValue = (PlanCycle.ShipmentList.Count > 0 ? "true" : "false");

                IList<RecurringPlanCycle> cycles = service.GetRecurringPlanCycleList(PlanCycle.Cycle.RecurringPlanID);
                RecurringPlanCycle firstRecurringCycle = cycles.FirstOrDefault(i => i.Recurring.Value);
                RecurringPlanCycle lastCycle = cycles.LastOrDefault();
                ddlAction.Items.Clear();
                ddlNextCycle.Items.Clear();
                if (PlanCycle.Cycle.RecurringPlanCycleID == null)
                {
                    //Adding of new cycle
                    ddlAction.Items.Add(new ListItem("End Plan", "false"));
                    ddlAction.Items.Add(new ListItem("Continue to next cycle", "true"));

                    int lastCycleIndex = 0;
                    if (lastCycle != null)
                    {
                        lastCycleIndex = lastCycle.Cycle.Value;
                    }

                    ddlNextCycle.Items.Add(new ListItem("Self", (lastCycleIndex + 1).ToString()));
                    for (int i = lastCycleIndex; i > 0; i--)
                    {
                        ddlNextCycle.Items.Add(new ListItem("#" + i.ToString(), i.ToString()));
                    }
                }
                else
                {
                    if (lastCycle != null && PlanCycle.Cycle.Cycle == lastCycle.Cycle)
                    {
                        //Editing of last cycle
                        ddlAction.Items.Add(new ListItem("End Plan", "false"));
                        ddlAction.Items.Add(new ListItem("Continue to next cycle", "true"));
                        if (firstRecurringCycle != null)
                        {
                            ddlAction.SelectedValue = "true";
                        }

                        ddlNextCycle.Items.Add(new ListItem("Self", lastCycle.Cycle.Value.ToString()));
                        for (int i = lastCycle.Cycle.Value - 1; i > 0; i--)
                        {
                            ddlNextCycle.Items.Add(new ListItem("#" + i.ToString(), i.ToString()));
                            if (firstRecurringCycle != null && i == firstRecurringCycle.Cycle.Value)
                            {
                                ddlNextCycle.Items[ddlNextCycle.Items.Count - 1].Selected = true;
                            }
                        }
                    }
                    else
                    {
                        //Editing of intermediate cycle
                        ddlAction.Items.Add(new ListItem("Continue to next cycle", "true"));
                        ddlNextCycle.Items.Add(new ListItem("#" + (PlanCycle.Cycle.Cycle + 1).ToString(), (PlanCycle.Cycle.Cycle + 1).ToString()));
                    }
                }
            }
        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            if (service.DeletePlanCycle(RecurringPlanCycleID))
            {
                PlanCycle = null;
                IsRemoved = true;
                DataBind();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            IList<KeyValuePair<string, int>> shipments = SelectedInventoryList;
            if (ddlShipping.SelectedValue == "false")
            {
                shipments = new List<KeyValuePair<string, int>>();
            }
            int? chargeTypeID = Utility.TryGetInt(ddlChargeType.SelectedValue);

            RecurringPlanCycleView res = null;
            int? nextCycle = null;
            if (ddlAction.SelectedValue == "true")
            {
                nextCycle = Utility.TryGetInt(ddlNextCycle.SelectedValue);
            }
            if (RecurringPlanCycleID == null)
            {
                res = service.InsertPlanCycle(RecurringPlanID, Utility.TryGetInt(tbInterim.Text), nextCycle,
                    chargeTypeID, Utility.TryGetDecimal(tbAmount.Text), shipments);
            }
            else
            {
                res = service.UpdatePlanCycle(RecurringPlanCycleID, Utility.TryGetInt(tbInterim.Text), nextCycle,
                    chargeTypeID, Utility.TryGetDecimal(tbAmount.Text), shipments);
            }

            if (res != null)
            {
                PlanCycle = res;
                phSuccess.Visible = true;
                DataBind();
            }
            else
            {
                phError.Visible = true;
            }
        }
    }
}
