<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FraudDescription.ascx.cs" Inherits="Securetrialoffers.admin.Controls.FraudDescription" %>
<%@ Register assembly="TrimFuel.Web.UI" namespace="TrimFuel.Web.UI" tagprefix="tf" %>
<table id="fraud-description" width="300px" class="value-set">
<tf:If ID="If1" runat="server" Condition='<%# CurrentState == State.Success %>'>
    <tr class="section-caption">
        <td colspan="2">Risk Score</td>
    </tr>
    <tr>
        <td class="value-caption">Risk Score<span class='briefInfo'>Representing the estimated probability that the order is fraud, based off of analysis of past minFraud transactions.</span class='briefInfo'></td>
        <td><%# GetParam("riskScore")%></td>
    </tr>
    <tr class="section-caption">
        <td colspan="2">Explanation</td>
    </tr>
    <tr>
        <td colspan="2"><%# GetParam("explanation")%></td>
    </tr>
    <tr class="section-caption">
        <td colspan="2">Geographical IP address location checking</td>
    </tr>
    <tr>
        <td class="value-caption">Country Match<span class='briefInfo'>Whether country of IP address matches billing address country (mismatch = higher risk).</span class='briefInfo'></td>
        <td><%# GetParam("countryMatch")%></td>
    </tr>
    <tr>
        <td class="value-caption">Country Code<span class='briefInfo'>Country Code of the IP address.</span class='briefInfo'></td>
        <td><%# GetParam("countryCode")%></td>
    </tr>
    <tr>
        <td class="value-caption">High Risk Country<span class='briefInfo'>Whether IP address or billing address country is in Egypt, Ghana, Indonesia, Lebanon, Macedonia, Morocco, Nigeria, Pakistan, Romania, Serbia and Montenegro, Ukraine, or Vietnam.</span class='briefInfo'></td>
        <td><%# GetParam("highRiskCountry")%></td>
    </tr>
    <tr>
        <td class="value-caption">Distance<span class='briefInfo'>Distance from IP address to Billing Location in kilometers (large distance = higher risk).</span class='briefInfo'></td>
        <td><%# GetParam("Distance")%></td>
    </tr>
    <tr>
        <td class="value-caption">IP Region<span class='briefInfo'>Estimated State/Region of the IP address, ISO-3166-2/FIPS 10-4 code.</span class='briefInfo'></td>
        <td><%# GetParam("ip_region")%></td>
    </tr>
    <tr>
        <td class="value-caption">IP City<span class='briefInfo'>Estimated City of the IP address.</span class='briefInfo'></td>
        <td><%# GetParam("ip_city")%></td>
    </tr>
    <tr>
        <td class="value-caption">IP Latitude<span class='briefInfo'>Estimated Latitude of the IP address.</span class='briefInfo'></td>
        <td><%# GetParam("ip_latitude")%></td>
    </tr>
    <tr>
        <td class="value-caption">IP Longitude<span class='briefInfo'>Estimated Longitude of the IP address.</span class='briefInfo'></td>
        <td><%# GetParam("ip_longitude")%></td>
    </tr>
    <tr>
        <td class="value-caption">IP ISP<span class='briefInfo'>ISP of the IP address.</span class='briefInfo'></td>
        <td><%# GetParam("ip_isp")%></td>
    </tr>
    <tr>
        <td class="value-caption">IP Organization<span class='briefInfo'>Organization of the IP address.</span class='briefInfo'></td>
        <td><%# GetParam("ip_org")%></td>
    </tr>
    <tr class="section-caption">
        <td colspan="2">Proxy Detection</td>
    </tr>
    <tr>
        <td class="value-caption">Aanonymous Proxy<span class='briefInfo'>Whether IP address is Anonymous Proxy (anonymous proxy = very high risk).</span class='briefInfo'></td>
        <td><%# GetParam("anonymousProxy")%></td>
    </tr>
    <tr>
        <td class="value-caption">Proxy Score<span class='briefInfo'>Likelihood of IP Address being an Open Proxy.</span class='briefInfo'></td>
        <td><%# GetParam("proxyScore")%></td>
    </tr>
    <tr>
        <td class="value-caption">Transparent Proxy<span class='briefInfo'>Whether IP address is in minFraud database of known transparent proxy servers.</span class='briefInfo'></td>
        <td><%# GetParam("isTransProxy")%></td>
    </tr>
    <tr class="section-caption">
        <td colspan="2">E-mail and Login Checks</td>
    </tr>
    <tr>
        <td class="value-caption">Free Mail<span class='briefInfo'>Whether e-mail is from free e-mail provider (free e-mail = higher risk).</span class='briefInfo'></td>
        <td><%# GetParam("freeMail")%></td>
    </tr>
    <tr>
        <td class="value-caption">Carder Email<span class='briefInfo'>Whether e-mail is in database of high risk e-mails.</span class='briefInfo'></td>
        <td><%# GetParam("carderEmail")%></td>
    </tr>
    <tr class="section-caption">
        <td colspan="2">Issuing Bank BIN Number Checks </td>
    </tr>
    <tr>
        <td class="value-caption">Bin Match<span class='briefInfo'>Whether country of issuing bank based on BIN number matches billing address country.</span class='briefInfo'></td>
        <td><%# GetParam("binMatch")%></td>
    </tr>
    <tr>
        <td class="value-caption">Bin Country<span class='briefInfo'>Country Code of the bank which issued the credit card based on BIN number.</span class='briefInfo'></td>
        <td><%# GetParam("binCountry")%></td>
    </tr>
    <tr>
        <td class="value-caption">Bin Name<span class='briefInfo'>Name of the bank which issued the credit card based on BIN number. Available for approximately 96% of BIN numbers.</span class='briefInfo'></td>
        <td><%# GetParam("binName")%></td>
    </tr>
    <tr>
        <td class="value-caption">Bin Phone<span class='briefInfo'>Customer service phone number listed on back of credit card. Available for approximately 75% of BIN numbers. In some cases phone number returned may be outdated.</span class='briefInfo'></td>
        <td><%# GetParam("binPhone")%></td>
    </tr>
    <tr class="section-caption">
        <td colspan="2">Address and Phone Number Checks</td>
    </tr>
    <tr>
        <td class="value-caption">Customer Phone In Billing Location<span class='briefInfo'>Whether the customer phone number is in the billing zip code. A return value of Yes provides a positive indication that the phone number listed belongs to the cardholder. A return value of No indicates that the phone number may be in a different area, or may not be listed in minFraud database. NotFound is returned when the phone number prefix cannot be found in minFraud database at all. Currently minFraud only supports US Phone numbers.</span class='briefInfo'></td>
        <td><%# GetParam("custPhoneInBillingLoc")%></td>
    </tr>
    <tr>
        <td class="value-caption">Ship Forward<span class='briefInfo'>Whether shipping address is in database of known mail drops</span class='briefInfo'></td>
        <td><%# GetParam("shipForward")%></td>
    </tr>
    <tr>
        <td class="value-caption">City Postal Match<span class='briefInfo'>Whether billing city and state match zipcode. Currently available for US addresses only, returns empty string outside the US. </span class='briefInfo'></td>
        <td><%# GetParam("cityPostalMatch")%></td>
    </tr>
    <tr>
        <td class="value-caption">Ship City Postal Match<span class='briefInfo'>Whether shipping city and state match zipcode. Currently available for US addresses only, returns empty string outside the US.</span class='briefInfo'></td>
        <td><%# GetParam("shipCityPostalMatch")%></td>
    </tr>
</tf:If>
<tf:If ID="If2" runat="server" Condition='<%# CurrentState == State.Empty %>'>
    <tr>
        <td>Fraud Score was not found for Billing</td>
    </tr>
</tf:If>
<tf:If ID="If3" runat="server" Condition='<%# CurrentState == State.NonMinFraud %>'>
    <tr class="section-caption">
        <td>Fraud Score was calculated by FraudShield</td>
    </tr>
    <tr>
        <td><%# FraudScore.Response.Replace("|", "<br/>") %></td>
    </tr>
</tf:If>
<tf:If ID="If4" runat="server" Condition='<%# CurrentState == State.Error %>'>
    <tr>
        <td class="value-caption">Error occured while Fraud Score was calculating</td>
    </tr>
    <tr>
        <td><%# GetParam("err") %></td>
    </tr>
</tf:If>
</table>