using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;

namespace TrimFuel.Business.Gateways
{
    public interface IEmailGateway
    {
        BusinessError<GatewayResult> SendEmail(long emailID, DynamicEmail dynaminEmail, Billing billing, decimal? shippingAmount, decimal? productAmount, string merchantName, string billingCancelCode, string ownRefererCode, string refererCode, string refererPassword);
        BusinessError<GatewayResult> SendEmail(long emailID, DynamicEmail dynaminEmail, Billing billing, decimal? shippingAmount, decimal? productAmount, string merchantName, string billingCancelCode, string ownRefererCode, string refererCode, string refererPassword, DateTime createDT, Product product);
        BusinessError<GatewayResult> SendEmail(string fromName, string fromAddress, string toName, string toAddress, string subject, string body);
    }
}
