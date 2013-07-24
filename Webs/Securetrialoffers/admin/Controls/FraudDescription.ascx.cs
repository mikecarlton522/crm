using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using TrimFuel.Business.Gateways;
using TrimFuel.Business;
using TrimFuel.Business.Gateways.MaxMind;

namespace Securetrialoffers.admin.Controls
{
    public partial class FraudDescription : System.Web.UI.UserControl
    {
        public enum State
        {
            Empty,
            Error,
            NonMinFraud,
            Success
        }

        private SaleService saleService = new SaleService();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public State CurrentState { get; set; }

        public long BillingID { get; set; }
        protected FraudScore FraudScore { get; set; }
        private IGatewayResponseParams ParamExtractor { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            FraudScore = saleService.GetFraudScoreByBilling(BillingID);
            if (FraudScore == null || FraudScore.Response == null || FraudScore.Error == null)
            {
                CurrentState = State.Empty;
            }
            else if (FraudScore.Error.Value)
            {
                CurrentState = State.Error;
                ParamExtractor = MaxMindGateway.CreateResponseParamsExtractor(FraudScore.Response);
            }
            else if (!FraudScore.Response.Contains("riskScore"))
            {
                CurrentState = State.NonMinFraud;
            }
            else
            {
                CurrentState = State.Success;
                ParamExtractor = MaxMindGateway.CreateResponseParamsExtractor(FraudScore.Response);
            }
        }

        protected string GetParam(string paramName)
        {
            string res = ParamExtractor.GetParam(paramName);
            return (string.IsNullOrEmpty(res)) ? "-" : res;
        }
    }
}