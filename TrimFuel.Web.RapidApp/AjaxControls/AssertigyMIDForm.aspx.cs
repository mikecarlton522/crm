using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Web.RapidApp.Logic;

namespace TrimFuel.Web.RapidApp.AjaxControls
{
    public partial class AssertigyMIDForm : BaseControlPage
    {
        TPClientService tpSer = new TPClientService();
        private int? assertigyMIDID = null;

        public int? NMICompanyID
        {
            get
            {
                return Utility.TryGetInt(Request.QueryString["nmiCompanyId"]);
            }
        }

        public int? AssertigyMIDID
        {
            get
            {
                if (assertigyMIDID == null)
                    assertigyMIDID = Utility.TryGetInt(Request.QueryString["assertigyMIDID"]);
                return assertigyMIDID;
            }
            set
            {
                assertigyMIDID = value;
            }
        }

        public int? TPClientID
        {
            get
            {
                return Utility.TryGetInt(Request["ClientId"]);
            }
        }

        public string ServiceName
        {
            get
            {
                var company = tpSer.GetClientGatewayService(NMICompanyID.Value, TPClientID.Value);
                return company.GatewayIntegrated;
            }
        }

        public AssertigyMID AssertigyMIDProp
        {
            get
            {
                AssertigyMID res = null;
                if (AssertigyMIDID != null)
                    res = tpSer.GetAssertigyMID(AssertigyMIDID.Value, TPClientID.Value);
                else
                {
                    res = new AssertigyMID() { Visible = false, GatewayName = ServiceName };
                    var defProp = new ServicesManager().GetAllGatewayServices().Where(u => u.ServiceName == ServiceName).SingleOrDefault();
                    if (defProp != null)
                    {
                        res.ChargebackFee = defProp.ChargebackFeeRetail;
                        res.ProcessingRate = defProp.TransactionFeeRetail;
                    }
                }
                return res;
            }
        }

        public AssertigyMIDSettings AssertigyMIDSettingProp
        {
            get
            {
                AssertigyMIDSettings res = null;
                if (AssertigyMIDID != null)
                    res = tpSer.GetAssertigyMIDSetting(AssertigyMIDID.Value, TPClientID.Value);

                if (res == null)
                {
                    res = new AssertigyMIDSettings();
                    var defProp = new ServicesManager().GetAllGatewayServices().Where(u => u.ServiceName == ServiceName).SingleOrDefault();
                    if (defProp != null)
                    {
                        res.ChargebackFee = defProp.ChargebackFee;
                        res.ChargebackRepresentationFee = defProp.ChargebackRepresentationFee;
                        res.ChargebackRepresentationFeeRetail = defProp.ChargebackRepresentationFeeRetail;
                        res.TransactionFee = defProp.TransactionFee;
                        res.DiscountRate = defProp.DiscountRate;

                        res.GatewayFee = defProp.GatewayFee;
                        res.GatewayFeeRetail = defProp.GatewayFeeRetail;

                    }
                }

                return res;
            }
        }

        public List<MIDCategory> Categories
        {
            get
            {
                return tpSer.GetMIDCategoryList(TPClientID.Value).ToList();
            }
        }

        public Dictionary<string, bool> PaymentTypes
        {
            get
            {
                Dictionary<string, bool> res = tpSer.GetAllPaymentTypes(TPClientID.Value).ToDictionary(v => v.DisplayName, v => false);
                if (AssertigyMIDID != null)
                {
                    var assertigyTypes = tpSer.GetAssertigyMIDPaymentTypes(AssertigyMIDID.Value, TPClientID.Value);
                    foreach (var type in assertigyTypes)
                    {
                        res[type.DisplayName] = true;
                    }
                }
                return res;
            }
        }

        public Dictionary<string, bool> Products
        {
            get
            {
                Dictionary<string, bool> res = tpSer.GetAllProducts(TPClientID.Value).Where(u => u.ProductIsActive.Value && u.ProductID.Value > 0).ToDictionary(v => v.ProductName, v => false);
                if (AssertigyMIDID != null)
                {
                    var assertigyProducts = tpSer.GetAssertigyMIDProducts(AssertigyMIDID.Value, TPClientID.Value);
                    foreach (var type in assertigyProducts)
                    {
                        res[type.ProductName] = true;
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

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            dpdCategories.DataSource = Categories;
            if (AssertigyMIDID != null)
                dpdCategories.SelectedValue = AssertigyMIDProp.MIDCategoryID.ToString();
        }

        protected void SaveChanges_Click(object sender, EventArgs e)
        {
            int clientID = Utility.TryGetInt(tbTPClientID.Text).Value;

            AssertigyMID assertigy = new AssertigyMID();
            assertigy.AssertigyMIDID = Utility.TryGetInt(tbAssertigyMIDID.Text);
            assertigy.ChargebackFee = Utility.TryGetDouble(tbChargebackFeeRetail.Text);
            assertigy.DisplayName = tbDisplayName.Text;
            assertigy.GatewayName = ServiceName;//tbGatewayName.Text;
            assertigy.MID = tbMID.Text;
            assertigy.MIDCategoryID = Utility.TryGetInt(dpdCategories.SelectedValue);
            assertigy.MonthlyCap = Utility.TryGetDecimal(tbMonthlyCap.Text);
            assertigy.ParentMID = 0;//Utility.TryGetInt(tbParentMID.Text);
            assertigy.ProcessingRate = Utility.TryGetDouble(tbProcessingRate.Text);
            assertigy.ReserveAccountRate = Utility.TryGetDouble(tbReserveAccountRate.Text);
            assertigy.TransactionFee = Utility.TryGetDouble(tbTransactionFeeRetail.Text);
            assertigy.Visible = cbVisible.Checked;

            AssertigyMIDSettings setting = new AssertigyMIDSettings();
            setting.AssertigyMIDID = Utility.TryGetInt(tbAssertigyMIDID.Text);
            setting.ChargebackFee = Utility.TryGetDouble(tbChargebackFee.Text);
            setting.TransactionFee = Utility.TryGetDouble(tbTransactionFee.Text);
            setting.ChargebackRepresentationFeeRetail = Utility.TryGetDouble(tbChargebackRepresentationFeeRetail.Text);
            setting.ChargebackRepresentationFee = Utility.TryGetDouble(tbChargebackRepresentationFee.Text);
            //setting.DiscountRateRetail = Utility.TryGetDouble(tbDiscountRateRetail.Text);
            setting.DiscountRate = Utility.TryGetDouble(tbDiscountRate.Text);

            setting.GatewayFee = Utility.TryGetDouble(tbGatewayFee.Text);
            setting.GatewayFeeRetail = Utility.TryGetDouble(tbGatewayFeeRetail.Text);

            List<int?> paymentTypesID = new List<int?>();
            var allTypes = tpSer.GetAllPaymentTypes(clientID);
            // get payment types from form
            foreach (RepeaterItem rItem in rPaymentTypes.Controls)
            {
                var checkBox = rItem.Controls[1] as CheckBox;
                if (checkBox != null)
                {
                    if (checkBox.Checked)
                    {
                        var currType = allTypes.Where(u => u.DisplayName == checkBox.Text.Trim()).SingleOrDefault();
                        paymentTypesID.Add(currType.PaymentTypeID);
                    }
                }
            }

            List<int?> productsID = new List<int?>();
            var allProducts = tpSer.GetAllProducts(clientID);
            // get products from form
            foreach (RepeaterItem rItem in rProducts.Controls)
            {
                var checkBox = rItem.Controls[1] as CheckBox;
                if (checkBox != null)
                {
                    if (checkBox.Checked)
                    {
                        var currProd = allProducts.Where(u => u.ProductName == checkBox.Text.Trim()).SingleOrDefault();
                        productsID.Add(currProd.ProductID);
                    }
                }
            }

            tpSer.SaveAssertigyMID(assertigy, clientID, Utility.TryGetInt(tbNMICompanyID.Text).Value, paymentTypesID, productsID, setting);
            lSaved.Visible = true;
            AssertigyMIDID = assertigy.AssertigyMIDID;
            DataBind();
        }
    }
}
