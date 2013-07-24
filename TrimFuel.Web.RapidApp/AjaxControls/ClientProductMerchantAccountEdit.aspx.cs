using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Web.RapidApp.Logic;

namespace TrimFuel.Web.RapidApp.AjaxControls
{
    public partial class ClientProductMerchantAccountEdit : BaseControlPage
    {
        TPClientService tpSer = new TPClientService();

        protected int? TPClientID
        {
            get
            {
                return Utility.TryGetInt(Request["ClientId"]);
            }
        }

        protected int? ProductID
        {
            get
            {
                return (Utility.TryGetInt(tbProductID.Text)) ?? Utility.TryGetInt(Request["productID"]);
            }
        }

        protected int CurrCurrency
        {
            get
            {
                var item = tpSer.GetProductCurrency(TPClientID.Value, ProductID.Value);
                return item == null ? 0 : item.CurrencyID.Value;
            }
        }

        protected List<Currency> CurrencyList
        {
            get
            {
                var returnedList = tpSer.GetCurrencyList(TPClientID.Value).ToList();
                returnedList.Insert(0, new Currency() { CurrencyID = 0, CurrencyName = "USD" });
                return returnedList;
            }
        }

        public Dictionary<AssertigyMID, bool> MerchantAccounts
        {
            get
            {
                Dictionary<AssertigyMID, bool> res = new Dictionary<AssertigyMID, bool>();
                var allAccounts = tpSer.GetAllAssertigyMIDList(TPClientID.Value);
                foreach (var account in allAccounts)
                    res.Add(account, false);

                if (ProductID != null)
                {
                    var merchantAccounts = tpSer.GetProductAssertigyMIDs(ProductID.Value, TPClientID.Value);
                    foreach (var merchantAccount in merchantAccounts)
                    {
                        res[allAccounts.Where(u => u.AssertigyMIDID == merchantAccount.AssertigyMIDID).SingleOrDefault()] = true;
                    }
                }

                return res;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lSaved.Visible = false;
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            List<int> accounts = new List<int>();
            List<AssertigyMID> allAccounts = tpSer.GetAllAssertigyMIDList(TPClientID.Value).ToList();

            foreach (RepeaterItem rItem in rProducts.Controls)
            {
                var checkBox = rItem.Controls[1] as CheckBox;
                if (checkBox != null)
                {
                    if (checkBox.Checked)
                    {
                        var currAccount = allAccounts.Where(u => u.DisplayName == checkBox.Text.Trim()).SingleOrDefault();
                        accounts.Add(currAccount.AssertigyMIDID.Value);
                    }
                }
            }

            tpSer.SaveProductMerchantAccount(TPClientID.Value, ProductID, Convert.ToInt32(dpCurrency.SelectedValue), accounts);
            lSaved.Visible = true;
            DataBind();
        }
    }
}
