using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using TrimFuel.Business.Utils;
using TrimFuel.Business;
using System.Collections;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class ProductMIDManager_test : System.Web.UI.Page
    {
        ProductService ps = new ProductService();
        SaleService ss = new SaleService();

        protected List<int?> ActiveMIDs = null;

        protected List<AssertigyMID> MIDs = null;

        protected List<AssertigyMID> MIDsToAdd = null;

        protected IList<ChargeHistoryEx> ChargedHistoryEx = null;

        protected int CurrCurrency
        {
            get
            {
                var item = ps.GetProductCurrency(ProductID.Value);
                return item == null ? 0 : item.CurrencyID.Value;
            }
        }

        protected List<Currency> CurrencyList
        {
            get
            {
                var returnedList = ps.GetCurrencyList().ToList();
                returnedList.Insert(0, new Currency() { CurrencyID = 0, CurrencyName = "USD" });
                return returnedList;
            }
        }        

        protected string ProductName
        {
            get
            {
                return ps.GetProduct(ProductID.Value).ProductName;
            }
        }

        protected int? ProductID
        {
            get
            {
                return Utility.TryGetInt(Request["productId"]) == null ? Utility.TryGetInt(hdnProductID.Value) : Utility.TryGetInt(Request["productId"]);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ProductID == null)
                return;
            MIDs = ps.GetMIDs().Where(u => u.Visible.Value).ToList();
            ActiveMIDs = MIDs.Select(u => u.AssertigyMIDID).ToList();
            MIDs.Insert(0, new AssertigyMID { AssertigyMIDID = 0, DisplayName = "-- Not Set --" });
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            var prodMID = ps.GetMIDProductList(ProductID.Value);
            ChargedHistoryEx = ss.GetChargeHistoryByProductID(ProductID.Value).Where(ch => ch.ChargeDate >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) 
                                                                                        && ch.ChargeDate < new DateTime(DateTime.Now.AddMonths(1).Year, DateTime.Now.AddMonths(1).Month, 1)).ToList();
            rMIDProd.DataSource = prodMID;
            MIDsToAdd = MIDs.Where(u => !prodMID.Select(t => t.AssertigyMIDID).Contains(u.AssertigyMIDID)).ToList();

            //var v = ChargedHistoryEx.Where(ch=>ch.MerchantAccountID == ).Sum(ch => ch.Amount);
        }

        protected void rMIDProd_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                int? midID = ps.GetMIDProduct(Convert.ToInt32(e.CommandArgument)).AssertigyMIDID;
                string MIDName = MIDs.Where(u => u.AssertigyMIDID == midID).SingleOrDefault().DisplayName;
                Note.Text = "MID " + MIDName + " was successfuly removed";
                ps.DeleteMIDProduct(Convert.ToInt32(e.CommandArgument));
            }
            if (e.CommandName == "save")
            {
                var editdpdrMIDs = e.Item.FindControl("editdpdrMIDs") as DropDownList;
                var editcbQueueRebills = e.Item.FindControl("editcbQueueRebills") as CheckBox;
                var editcbUseForTrial = e.Item.FindControl("editcbUseForTrial") as CheckBox;

                var midProduct = ps.GetMIDProduct(Convert.ToInt32(e.CommandArgument));
                string MIDName = MIDs.Where(u => u.AssertigyMIDID == midProduct.AssertigyMIDID).SingleOrDefault().DisplayName;
                
                midProduct.AssertigyMIDID = midProduct.AssertigyMIDID;
                midProduct.UseForTrial = editcbUseForTrial.Checked;
                midProduct.QueueRebills = editcbQueueRebills.Checked;
                midProduct.RolloverAssertigyMIDID = Utility.TryGetInt(editdpdrMIDs.SelectedValue) == 0 ? null : Utility.TryGetInt(editdpdrMIDs.SelectedValue);

                Note.Text = "MID " + MIDName + " was successfuly updated";
                ps.SaveMIDProduct(midProduct);
            }
            CurrencyNote.Text = string.Empty;
            DataBind();
        }

        protected void btnAddMID_Click(object sender, EventArgs e)
        {
            NMIMerchantAccountProduct midProduct = new NMIMerchantAccountProduct()
                {
                    ProductID = ProductID,
                    AssertigyMIDID = Utility.TryGetInt(dpdMIDs.SelectedValue),
                    Percentage = 100,
                    OnlyRefundCredit = false,
                    UseForRebill = true,
                    UseForTrial = cbUseForTrial.Checked,
                    QueueRebills = cbQueueRebills.Checked,
                    RolloverAssertigyMIDID = Utility.TryGetInt(dpdrMIDs.SelectedValue),
                    MerchantAccountProductID = null
                };
            ps.SaveMIDProduct(midProduct);
            string MIDName = MIDs.Where(u => u.AssertigyMIDID == Utility.TryGetInt(dpdMIDs.SelectedValue)).SingleOrDefault().DisplayName;
            Note.Text = "MID " + MIDName + " was successfuly added";
            CurrencyNote.Text = string.Empty;
            DataBind();
        }

        protected void currency_Save(object sender, EventArgs e)
        {
            ps.SaveProductCurrency(ProductID.Value, Convert.ToInt32(dpCurrency.SelectedValue));
            CurrencyNote.Text = "Currency was successfuly updated";
            Note.Text = string.Empty;
            DataBind();
        }
    }
}
