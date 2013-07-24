<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Assertigy.aspx.cs" Inherits="Gateways.Assertigy" ValidateRequest="false" %>
<% if (IsSuccess)
   { %>
<ccTxnResponseV1 xmlns="http://www.assertigy.com/creditcard/xmlschema/v1">
  <confirmationNumber>306311898</confirmationNumber>
  <childAccountNum>1001168054</childAccountNum>
  <decision>ACCEPTED</decision>
  <code>0</code>
  <description>No Error</description>
  <authCode>995958</authCode>
  <avsResponse>N</avsResponse>
  <cvdResponse>M</cvdResponse>
  <detail>
    <tag>InternalResponseCode</tag>
    <value>0</value>
  </detail>
  <detail>
    <tag>SubErrorCode</tag>
    <value>0</value>
  </detail>
  <detail>
    <tag>InternalResponseDescription</tag>
    <value>no_error</value>
  </detail>
  <txnTime>2010-07-15T04:00:53.343-04:00</txnTime>
  <duplicateFound>false</duplicateFound>
</ccTxnResponseV1>
   
<% }
   else
   { %>
<ccTxnResponseV1 xmlns="http://www.assertigy.com/creditcard/xmlschema/v1">
  <confirmationNumber>306311892</confirmationNumber>
  <childAccountNum>1001168356</childAccountNum>
  <decision>DECLINED</decision>
  <code>3009</code>
  <actionCode>D</actionCode>
  <description>Your request has been declined by the issuing bank.</description>
  <avsResponse>Y</avsResponse>
  <cvdResponse>M</cvdResponse>
  <detail>
    <tag>InternalResponseCode</tag>
    <value>160</value>
  </detail>
  <detail>
    <tag>SubErrorCode</tag>
    <value>1005</value>
  </detail>
  <detail>
    <tag>InternalResponseDescription</tag>
    <value>auth declined</value>
  </detail>
  <txnTime>2010-07-15T04:00:49.039-04:00</txnTime>
  <duplicateFound>false</duplicateFound>
</ccTxnResponseV1>
<% } %>
