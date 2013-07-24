using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Web.RapidApp.Logic;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.RapidApp.AjaxControls
{
    public partial class ServiceDefaultSettings : BaseControlPage
    {
        protected struct ItemToDisplay
        {
            public string DisplayName { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
        }

        ServicesManager ser = new ServicesManager();

        protected int? ServiceID
        {
            get
            {
                return (Utility.TryGetInt(hdnServiceID.Value)) ?? Utility.TryGetInt(Request["serviceID"]);
            }
        }

        protected string ServiceType
        {
            get
            {
                return string.IsNullOrEmpty(hdnServiceType.Value) ? Request["type"].ToString() : hdnServiceType.Value;
            }
        }

        protected string ServiceName
        {
            get
            {
                return string.IsNullOrEmpty(hdnServiceName.Value) ? Request["sername"].ToString() : hdnServiceName.Value;
            }
        }

        protected List<ItemToDisplay> Fees
        {
            get
            {
                switch (ServiceType.ToLower())
                {
                    case "fulfillment":
                        {
                            return GetFulfillmentFees();
                        }
                    case "outbound":
                        {
                            return GetOutboundFees();
                        }
                    case "gateway":
                        {
                            return GetGatewayFees();
                        }
                }
                return new List<ItemToDisplay>();
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

        }

        protected void SaveChanges_Click(object sender, EventArgs e)
        {
            // get config values from form

            Dictionary<string, string> fees = new Dictionary<string, string>();

            foreach (RepeaterItem rItem in rFees.Controls)
            {
                var hidden = rItem.Controls[1] as HiddenField;
                var textBox = rItem.Controls[3] as TextBox;
                fees.Add(hidden.Value, textBox.Text);
            }

            switch (ServiceType.ToLower())
            {
                case "fulfillment":
                    {
                        ser.SaveFulfillmentSettings(fees, ServiceID.Value);
                        break;
                    }
                case "outbound":
                    {
                        ser.SaveOutboundSettings(fees, ServiceID.Value);
                        break;
                    }
                case "gateway":
                    {
                        ser.SaveGatewaySettings(fees, ServiceName);
                        break;
                    }
            }

            lSaved.Visible = true;
            DataBind();
        }

        private List<ItemToDisplay> GetFulfillmentFees()
        {
            List<ItemToDisplay> fees = new List<ItemToDisplay>();
            var fulfillmentDefProp = ser.GetAllFulfillmentServices().Where(u => u.ID == ServiceID.Value).SingleOrDefault();
            fees.Add(new ItemToDisplay()
            {
                Key = "CustomDevelopmentFee",
                Value = fulfillmentDefProp.CustomDevelopmentFee.ToString(),
                DisplayName = "Custom Development Fee"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "ReturnsFee",
                Value = fulfillmentDefProp.ReturnsFee.ToString(),
                DisplayName = "Returns Fee"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "KittingAndAsemblyFee",
                Value = fulfillmentDefProp.KittingAndAsemblyFee.ToString(),
                DisplayName = "Kitting and Asembly Fee"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "SetupFee",
                Value = fulfillmentDefProp.SetupFee.ToString(),
                DisplayName = "Setup Fee"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "ShipmentFee",
                Value = fulfillmentDefProp.ShipmentFee.ToString(),
                DisplayName = "Shipment Fee"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "ShipmentSKUFee",
                Value = fulfillmentDefProp.ShipmentSKUFee.ToString(),
                DisplayName = "Shipment Fee (Per SKU)"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "SpecialLaborFee",
                Value = fulfillmentDefProp.SpecialLaborFee.ToString(),
                DisplayName = "Special Labor Fee"
            });

            fees.Add(new ItemToDisplay()
            {
                Key = "CustomDevelopmentFeeRetail",
                Value = fulfillmentDefProp.CustomDevelopmentFeeRetail.ToString(),
                DisplayName = "Custom Development Fee (Retail)"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "ReturnsFeeRetail",
                Value = fulfillmentDefProp.ReturnsFeeRetail.ToString(),
                DisplayName = "Returns Fee (Retail)"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "KittingAndAsemblyFeeRetail",
                Value = fulfillmentDefProp.KittingAndAsemblyFeeRetail.ToString(),
                DisplayName = "Kitting and Asembly Fee (Retail)"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "SetupFeeRetail",
                Value = fulfillmentDefProp.SetupFeeRetail.ToString(),
                DisplayName = "Setup Fee (Retail)"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "ShipmentFeeRetail",
                Value = fulfillmentDefProp.ShipmentFeeRetail.ToString(),
                DisplayName = "Shipment Fee (Retail)"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "ShipmentSKUFeeRetail",
                Value = fulfillmentDefProp.ShipmentSKUFeeRetail.ToString(),
                DisplayName = "Shipment Fee (Per SKU) (Retail)"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "SpecialLaborFeeRetail",
                Value = fulfillmentDefProp.SpecialLaborFeeRetail.ToString(),
                DisplayName = "Special Labor Fee (Retail)"
            });

            return fees;
        }

        private List<ItemToDisplay> GetOutboundFees()
        {
            List<ItemToDisplay> fees = new List<ItemToDisplay>();
            var outboundDefProp = ser.GetAllOutboundServices().Where(u => u.ID == ServiceID.Value).SingleOrDefault();
            fees.Add(new ItemToDisplay()
            {
                Key = "SetupFee",
                Value = outboundDefProp.SetupFee.ToString(),
                DisplayName = "Setup Fee"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "MonthlyFee",
                Value = outboundDefProp.MonthlyFee.ToString(),
                DisplayName = "Monthly Fee"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "PerPourFee",
                Value = outboundDefProp.PerPourFee.ToString(),
                DisplayName = "Per Hour Fee"
            });

            fees.Add(new ItemToDisplay()
            {
                Key = "SetupFeeRetail",
                Value = outboundDefProp.SetupFeeRetail.ToString(),
                DisplayName = "Setup Fee (Retail)"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "MonthlyFeeRetail",
                Value = outboundDefProp.MonthlyFeeRetail.ToString(),
                DisplayName = "Monthly Fee (Retail)"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "PerPourFeeRetail",
                Value = outboundDefProp.PerPourFeeRetail.ToString(),
                DisplayName = "Per Pour Fee (Retail)"
            });

            return fees;
        }

        private List<ItemToDisplay> GetGatewayFees()
        {
            List<ItemToDisplay> fees = new List<ItemToDisplay>();
            var outboundDefProp = ser.GetAllGatewayServices().Where(u => u.ServiceName == ServiceName).SingleOrDefault();
            fees.Add(new ItemToDisplay()
            {
                Key = "ChargebackFee",
                Value = outboundDefProp.ChargebackFee.ToString(),
                DisplayName = "Chargeback Fee"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "ChargebackRepresentationFee",
                Value = outboundDefProp.ChargebackRepresentationFee.ToString(),
                DisplayName = "Chargeback Representation Fee"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "TransactionFee",
                Value = outboundDefProp.TransactionFee.ToString(),
                DisplayName = "Transaction Fee"
            });

            fees.Add(new ItemToDisplay()
            {
                Key = "ChargebackFeeRetail",
                Value = outboundDefProp.ChargebackFeeRetail.ToString(),
                DisplayName = "Chargeback Fee (Retail)"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "ChargebackRepresentationFeeRetail",
                Value = outboundDefProp.ChargebackRepresentationFeeRetail.ToString(),
                DisplayName = "Chargeback Representation Fee (Retail)"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "TransactionFeeRetail",
                Value = outboundDefProp.TransactionFeeRetail.ToString(),
                DisplayName = "Transaction Fee (Retail)"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "DiscountRateRetail",
                Value = outboundDefProp.DiscountRateRetail.ToString(),
                DisplayName = "Discount Rate (Retail)"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "DiscountRate",
                Value = outboundDefProp.DiscountRate.ToString(),
                DisplayName = "Discount Rate"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "GatewayFee",
                Value = outboundDefProp.GatewayFee.ToString(),
                DisplayName = "Gateway Fee (Cost)"
            });
            fees.Add(new ItemToDisplay()
            {
                Key = "GatewayFeeRetail",
                Value = outboundDefProp.GatewayFeeRetail.ToString(),
                DisplayName = "Gateway Fee (Retail)"
            });

            return fees;
        }
    }
}
