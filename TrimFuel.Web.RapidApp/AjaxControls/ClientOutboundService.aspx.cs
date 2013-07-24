using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Web.RapidApp.Logic;
using TrimFuel.Business;
using TrimFuel.Business.Utils;
using TrimFuel.Model;

namespace TrimFuel.Web.RapidApp.AjaxControls
{
    public partial class ClientOutboundService : BaseControlPage
    {
        ServicesManager ser = new ServicesManager();
        TPClientService tpSer = new TPClientService();
        protected bool IsNew
        {
            get
            {
                if (LeadPartnerID != null)
                {
                    var record = tpSer.GetClientLeadPartner(LeadPartnerID.Value, TPClientID.Value);
                    if (record != null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        List<LeadType> leadTypes = null;
        List<LeadPartnerConfigValue> configValues = null;
        List<Product> products = null;

        public string HeaderTitle
        {
            get
            {
                if (IsNew)
                    return "Add Call Center (Outbound): " + LeadPartnerName;
                return "Edit Call Center (Outbound): " + LeadPartnerName;
            }
        }

        protected int? TPClientID
        {
            get
            {
                return (Utility.TryGetInt(hdnTPClientID.Value)) ?? Utility.TryGetInt(Request["ClientId"]);
            }
        }

        protected int? LeadPartnerID
        {
            get
            {
                return (Utility.TryGetInt(hdnLeadPartnerID.Value)) ?? Utility.TryGetInt(Request["serviceID"]);
            }
        }

        protected string LeadPartnerName
        {
            get
            {
                return string.IsNullOrEmpty(hdnLeadPartnerName.Value) ? Request["service"].ToString() : hdnLeadPartnerName.Value;
            }
        }

        protected LeadPartnerSettings OutboundSettingsProp
        {
            get
            {
                if (LeadPartnerID != null)
                {
                    var record = tpSer.GetClientLeadPartnerSettings(LeadPartnerID.Value, TPClientID.Value);
                    if (record != null)
                    {
                        return record;
                    }

                    OutboundServiceItem item = ser.GetAllOutboundServices().Where(u => u.ID == LeadPartnerID.Value).SingleOrDefault();
                    if (item == null)
                        throw new ArgumentException();

                    return new LeadPartnerSettings()
                    {
                        SetupFee = item.SetupFee,
                        SetupFeeRetail = item.SetupFeeRetail,
                        MonthlyFee = item.MonthlyFee,
                        MonthlyFeeRetail = item.MonthlyFeeRetail,
                        PerPourFee = item.PerPourFee,
                        PerPourFeeRetail = item.PerPourFeeRetail
                    };
                }
                else
                {
                    return tpSer.GetClientLeadPartnerSettings(LeadPartnerID.Value, TPClientID.Value);
                }
            }
        }

        protected List<string> PossibleKeys
        {
            get
            {
                return TrimFuel.Model.Enums.LeadPartnerConfigList.Values.Where(u => u.LeadPartnerID == LeadPartnerID.Value).Select(u => u.Key).ToList();
            }
        }

        protected List<LeadType> LeadTypes
        {
            get
            {
                if (leadTypes == null)
                    leadTypes = tpSer.GetLeadTypes(TPClientID.Value).ToList();
                return leadTypes;
            }
        }

        protected List<LeadPartnerConfigValue> ConfigValues
        {
            get
            {
                if (configValues == null)
                    configValues = tpSer.GetLeadPartnerConfigValues(LeadPartnerID.Value, TPClientID.Value);
                return configValues;
            }
        }

        protected List<Product> Products
        {
            get
            {
                if (products == null)
                {
                    products = tpSer.GetAllProducts(TPClientID.Value).Where(u => u.ProductIsActive.Value).ToList();
                    products.Insert(0, new Product() { ProductID = null, ProductName = "-- Select --" });
                }
                return products;
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

        protected void rConfigValues_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "remove")
            {
                var paramsArr = e.CommandArgument.ToString().Split(',');
                LeadPartnerConfigValue.ID configID = new LeadPartnerConfigValue.ID()
                {
                    LeadPartnerID = LeadPartnerID,
                    LeadTypeID = Utility.TryGetInt(paramsArr[0]),
                    ProductID = Utility.TryGetInt(paramsArr[1]),
                    Key = paramsArr[2]
                };
                tpSer.DeleteLeadPartnerConfigValue(TPClientID.Value, configID);
            }
            lSaved.Visible = true;
            DataBind();
        }

        protected void SaveChanges_Click(object sender, EventArgs e)
        {
            LeadPartnerSettings setting = new LeadPartnerSettings()
            {
                LeadPartnerSettingID = Utility.TryGetInt(hdnLeadPartnerSettingID.Value),
                LeadPartnerID = Utility.TryGetInt(hdnLeadPartnerID.Value),
                SetupFee = Utility.TryGetDouble(tbSetupFee.Text),
                SetupFeeRetail = Utility.TryGetDouble(tbSetupFeeRetail.Text),
                MonthlyFee = Utility.TryGetDouble(tbMonthlyFee.Text),
                MonthlyFeeRetail = Utility.TryGetDouble(tbMonthlyFeeRetail.Text),
                PerPourFee = Utility.TryGetDouble(tbPerPourFee.Text),
                PerPourFeeRetail = Utility.TryGetDouble(tbPerPourFeeRetail.Text)
            };

            tpSer.SaveLeadPartnerSettings(TPClientID.Value, setting, LeadPartnerName, LeadPartnerID.Value);
            lSaved.Visible = true;
            DataBind();
        }

        protected void btnAddConfig_Click(object sender, EventArgs e)
        {
            LeadPartnerConfigValue newConfigValue = new LeadPartnerConfigValue()
            {
                LeadPartnerConfigValueID = null,
                Key = Request["fKey"],
                LeadPartnerID = LeadPartnerID,
                Value = Request["fValue"],
                LeadTypeID = Utility.TryGetInt(Request["fLeadTypeID"]),
                ProductID = Utility.TryGetInt(Request["fProductID"])
            };

            tpSer.SaveLeadPartnerConfigValue(TPClientID.Value, newConfigValue);

            lSaved.Visible = true;
            DataBind();
        }

        protected void btnEditConfig_Click(object sender, EventArgs e)
        {
            LeadPartnerConfigValue configValue = new LeadPartnerConfigValue()
            {
                LeadPartnerConfigValueID = new LeadPartnerConfigValue.ID()
                {
                    Key = Request["old_lblKey"],
                    LeadPartnerID = LeadPartnerID,
                    LeadTypeID = Utility.TryGetInt(Request["lblLeadTypeID"]),
                    ProductID = Utility.TryGetInt(Request["lblSelectedProductID"]),
                },
                Key = Request["editdpdrPossibleKeys"],
                LeadPartnerID = LeadPartnerID,
                Value = Request["editTbValue"],
                LeadTypeID = Utility.TryGetInt(Request["lblLeadTypeID"]),
                ProductID = Utility.TryGetInt(Request["editdpdrProducts"]),
            };

            tpSer.SaveLeadPartnerConfigValue(TPClientID.Value, configValue);

            lSaved.Visible = true;
            DataBind();
        }
    }
}
