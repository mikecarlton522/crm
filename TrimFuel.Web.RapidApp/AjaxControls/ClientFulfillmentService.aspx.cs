using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Web.RapidApp.Logic;
using TrimFuel.Business.Dao;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Model.Views;

namespace TrimFuel.Web.RapidApp.AjaxControls
{
    public partial class ClientFulfillmentService : BaseControlPage
    {
        ServicesManager ser = new ServicesManager();
        TPClientService tpSer = new TPClientService();

        protected bool IsNew
        {
            get
            {
                if (string.IsNullOrEmpty(Request.QueryString["new"]) || Request.QueryString["new"] != "new")
                    return false;
                return true;
            }
        }

        protected int? ShipperID
        {
            get
            {
                return (Utility.TryGetInt(hdnShipperID.Value)) ?? Utility.TryGetInt(Request["serviceID"]);
            }
        }

        protected string ShipperName
        {
            get
            {
                return string.IsNullOrEmpty(hdnShipperName.Value) ? Request["service"].ToString() : hdnShipperName.Value;
            }
        }

        public string HeaderTitle
        {
            get
            {
                if (ShipperID == null)
                    return "Add Fulfillment: " + ShipperName;
                return "Edit Fulfillment: " + ShipperName;
            }
        }

        public int? TPClientID
        {
            get
            {
                return Utility.TryGetInt(Request["ClientId"]);
            }
        }

        public List<Product> Products
        {
            get
            {
                var shipperProductsID = ShipperProducts.Select(u => u.ProductID);
                return tpSer.GetAllProducts(TPClientID.Value)
                    .Where(u => (u.ProductIsActive.Value && !shipperProductsID.Contains(u.ProductID.Value))).ToList();
            }
        }

        public List<ShipperProductView> ShipperProducts
        {
            get
            {
                return tpSer.GetShipperProductList(TPClientID.Value).Where(u => u.ShipperID == ShipperID.Value).ToList();
            }
        }

        public TPClientShipperSettings fulfillmentSettingsProp = null;
        public TPClientShipperSettings FulfillmentSettingsProp
        {
            get
            {
                if (fulfillmentSettingsProp != null)
                    return fulfillmentSettingsProp;

                var record = tpSer.GetClientShipperSettings(ShipperID.Value, TPClientID.Value);
                if (record != null)
                {
                    return record;
                }

                FulfillmentServiceItem item = ser.GetAllFulfillmentServices().Where(u => u.ID == ShipperID.Value).SingleOrDefault();
                if (item == null)
                    throw new ArgumentException();

                fulfillmentSettingsProp = new TPClientShipperSettings()
                    {
                        ReturnsFee = item.ReturnsFee,
                        KittingAndAsemblyFee = item.KittingAndAsemblyFee,
                        SetupFee = item.SetupFee,
                        ShipmentFee = item.ShipmentFee,
                        ShipmentSKUFee = item.ShipmentSKUFee,
                        ShipperID = 0,
                        SpecialLaborFee = item.SpecialLaborFee,
                        CustomDevelopmentFee = item.CustomDevelopmentFee,
                        ShipperSettingID = 0,

                        ReturnsFeeRetail = item.ReturnsFeeRetail,
                        KittingAndAsemblyFeeRetail = item.KittingAndAsemblyFeeRetail,
                        SetupFeeRetail = item.SetupFeeRetail,
                        ShipmentFeeRetail = item.ShipmentFeeRetail,
                        ShipmentSKUFeeRetail = item.ShipmentSKUFeeRetail,
                        SpecialLaborFeeRetail = item.SpecialLaborFeeRetail,
                        CustomDevelopmentFeeRetail = item.CustomDevelopmentFeeRetail
                    };
                return fulfillmentSettingsProp;
            }
        }

        public List<ShipperConfig> ConfigFields
        {
            get
            {
                List<string> resFromConfigKeys = TrimFuel.Model.Enums.ShipperConfigList.Values.Where(u => u.ShipperID == ShipperID.Value).Select(u => u.Key).ToList();
                List<ShipperConfig> res = tpSer.GetShipperConfig(ShipperID.Value, TPClientID.Value);
                for (int i = 0; i < res.Count; i++)
                {
                    if (!resFromConfigKeys.Contains(res[i].Key))
                    {
                        res.Remove(res[i]);
                        i--;
                    }
                }
                foreach (var configKey in resFromConfigKeys)
                {
                    if (res.Where(u => u.Key == configKey).Count() == 0)
                    {
                        res.Add(new ShipperConfig()
                            {
                                Key = configKey,
                                ShipperID = ShipperID,
                                Value = string.Empty
                            });
                    }
                }

                return res;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lSaved.Visible = false;
            if (ShipperID == null)
                return;
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        protected void SaveChanges_Click(object sender, EventArgs e)
        {
            List<ShipperConfig> config = new List<ShipperConfig>();

            // get config values from form
            foreach (RepeaterItem rItem in rConfig.Controls)
            {
                var textBox = rItem.Controls[1] as TextBox;
                if (textBox != null)
                {
                    config.Add(new ShipperConfig { Key = textBox.Attributes["Name"], Value = textBox.Text });
                }
            }

            TPClientShipperSettings setting = new TPClientShipperSettings()
            {
                CompanyEmail = tbEmail.Text,
                CompanyName = tbName.Text,
                CompanyPhone = tbPhone.Text,
                ShipperSettingID = TryGetInt(tbShipperSettingID.Text),
                ShipperID = ShipperID.Value,
                SpecialLaborFee = TryGetDouble(tbSpecialLaborFee.Text),
                CustomDevelopmentFee = TryGetDouble(tbCustomDevelopmentFee.Text),
                KittingAndAsemblyFee = TryGetDouble(tbKittingFee.Text),
                ShipmentFee = TryGetDouble(tbShipmentFee.Text),
                ReturnsFee = TryGetDouble(tbReturnsFee.Text),
                SetupFee = TryGetDouble(tbSetupFee.Text),
                SpecialLaborFeeRetail = TryGetDouble(tbSpecialLaborFeeRetail.Text),
                CustomDevelopmentFeeRetail = TryGetDouble(tbCustomDevelopmentFeeRetail.Text),
                KittingAndAsemblyFeeRetail = TryGetDouble(tbKittingFeeRetail.Text),
                ShipmentFeeRetail = TryGetDouble(tbShipmentFeeRetail.Text),
                ReturnsFeeRetail = TryGetDouble(tbReturnsFeeRetail.Text),
                SetupFeeRetail = TryGetDouble(tbSetupFeeRetail.Text),
                ShipmentSKUFee = TryGetDouble(tbShipmentSKUFee.Text),
                ShipmentSKUFeeRetail = TryGetDouble(tbShipmentSKUFeeRetail.Text)
            };

            tpSer.SaveShipperSettings(TryGetInt(tbClientID.Text), setting, config, ShipperName, ShipperID.Value);
            lSaved.Visible = true;
            DataBind();
        }

        protected void btnAddProduct_Click(object sender, EventArgs e)
        {
            List<ShipperProduct> lstToAdd = new List<ShipperProduct>();
            lstToAdd.Add(new ShipperProduct()
                {
                    NeedConfirm = true,
                    ProductID = Utility.TryGetInt(dpdProducts.SelectedValue),
                    ShipperID = ShipperID
                });
            tpSer.SaveShipperProducts(Utility.TryGetInt(tbClientID.Text).Value, lstToAdd);
            DataBind();
        }

        protected void rProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                tpSer.DeleteShipperProduct(TPClientID.Value, Convert.ToInt32(e.CommandArgument));
                DataBind();
            }
        }

        private double TryGetDouble(string str)
        {
            double res = 0;
            double.TryParse(str, out res);
            return res;
        }

        private int TryGetInt(string str)
        {
            int res = 0;
            int.TryParse(str, out res);
            return res;
        }
    }
}
