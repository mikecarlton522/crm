<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrismPay.aspx.cs" Inherits="Gateways.PrismPay" %>
<% if (IsSuccess)
   { %>
Accepted=SALE:VITAL5:::46031495:::
historyid=46031495
orderid=36665845
Accepted=SALE:VITAL5:::46031495:::
ACCOUNTNUMBER=************5454
authcode=VITAL5
AuthNo=SALE:VITAL5:::46031495:::
historyid=46031495
orderid=36665845
recurid=0
refcode=46031495-VITAL5
result=1
Status=Accepted
transid=0
<% }
   else
   { %>
Declined=DECLINED:1101440001:Invalid Expiration Date
historyid=46031833
orderid=36666162
ACCOUNTNUMBER=************5454
Declined=DECLINED:1101440001:Invalid Expiration Date
historyid=46031833
orderid=36666162
rcode=1101440001
Reason=DECLINED:1101440001:Invalid Expiration Date
recurid=0
result=0
Status=Declined
transid=0
<% } %>
