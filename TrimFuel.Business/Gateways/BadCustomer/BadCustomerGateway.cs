using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using TrimFuel.Model;
using TrimFuel.Business.Utils;

namespace TrimFuel.Business.Gateways.BadCustomer
{
    public class BadCustomerGateway : IBadCustomerGateway
    {
        private string GetResponse(string request)
        {
            WebClient wc = new WebClient();
            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            return wc.UploadString(Config.Current.BAD_CUSTOMER_URL, "POST", request);
        }

        private void ProcessResult(BusinessError<GatewayResult> result)
        {
            byte? found = Utility.TryGetByte(ExtractResponseParam(result.ReturnValue.Response, "found"));
            if (found == null || found == 0)
            {
                //Success
                result.State = BusinessErrorState.Success;
            }
            else
            {
                //Error
                result.State = BusinessErrorState.Error;
                result.ErrorMessage = ExtractResponseParam(result.ReturnValue.Response, "result");
            }
        }

        public BusinessError<GatewayResult> ValidateCustomer(Billing billing)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

            GatewayResult gatewayResult = new GatewayResult();
            gatewayResult.Request = PrepareRequest(billing);
            gatewayResult.Response = GetResponse(gatewayResult.Request);

            res.ReturnValue = gatewayResult;
            ProcessResult(res);

            gatewayResult.ResponseParams = new BadCustomerGatewayReponseParams(gatewayResult.Response);

            return res;
        }

        private string PrepareRequest(Billing billing)
        {
            string res = Utility.LoadFromEmbeddedResource(typeof(BadCustomerGateway), "badcustomer.tpl");

            res = res.Replace("##CARD_NUMBER_HASH##", billing.CreditCardCnt.MD5Hash);
            res = res.Replace("##CARD_HOLDER_NAME##", billing.FullName);

            return res;
        }

        private int? ExtractResponseParamInt(string response, string paramName)
        {
            int res = 0;
            if (int.TryParse(ExtractResponseParam(response, paramName), out res))
            {
                return res;
            }
            return null;
        }

        private string ExtractResponseParam(string response, string paramName)
        {
            BadCustomerGatewayReponseParams p = new BadCustomerGatewayReponseParams(response);
            return p.GetParam(paramName);
        }
    }
}
