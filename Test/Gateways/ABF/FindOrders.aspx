<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FindOrders.aspx.cs" Inherits="Gateways.ABF.FindOrders" %>
<% if (true)
   { %><orders>
<order>
<CustomerName>Triangle Media Corp</CustomerName>
<Facility>Chatsworth</Facility>
<WarehouseTransactionID>11777</WarehouseTransactionID>
<ReferenceNum>67791</ReferenceNum>
<Retailer />
<ShipToCompanyName></ShipToCompanyName>
<MarkForName></MarkForName>
<BatchOrderID>1</BatchOrderID>
<CreationDate>2010-11-18T09:28:00</CreationDate>
<EarliestShipDate />
<ShipCancelDate />
<PickupDate />
<Carrier>USPS</Carrier>
<BillingCode>Prepaid</BillingCode>
<TotWeight>0.03</TotWeight>
<TotCuFt>0.00</TotCuFt>
<TotPackages>1.0000</TotPackages>
<TotOrdQty>1.0000</TotOrdQty>
<TotLines>1.00</TotLines>
<PONum></PONum>
<Notes>Ecig</Notes>
<OverAllocated></OverAllocated>
<PickTicketPrintDate>2010-11-19</PickTicketPrintDate>
<ProcessDate>2012-03-06</ProcessDate>
<TrackingNumber>12341234123412341234</TrackingNumber>
<LoadNumber></LoadNumber>
<BillOfLading></BillOfLading>
<MasterBillOfLading></MasterBillOfLading>
<ASNSentDate />
<ConfirmASNSentDate></ConfirmASNSentDate>
<RememberRowInfo>11777:16:1::2010/11/19:173:True</RememberRowInfo>
</order>
<order>
<CustomerName>Triangle Media Corp</CustomerName>
<Facility>Chatsworth</Facility>
<WarehouseTransactionID>11777</WarehouseTransactionID>
<ReferenceNum>67770</ReferenceNum>
<Retailer />
<ShipToCompanyName></ShipToCompanyName>
<MarkForName></MarkForName>
<BatchOrderID>2</BatchOrderID>
<CreationDate>2010-11-18T09:28:00</CreationDate>
<EarliestShipDate />
<ShipCancelDate />
<PickupDate />
<Carrier>USPS</Carrier>
<BillingCode>Prepaid</BillingCode>
<TotWeight>0.03</TotWeight>
<TotCuFt>0.00</TotCuFt>
<TotPackages>1.0000</TotPackages>
<TotOrdQty>1.0000</TotOrdQty>
<TotLines>1.00</TotLines>
<PONum></PONum>
<Notes>Ecig</Notes>
<OverAllocated></OverAllocated>
<PickTicketPrintDate>2010-11-19</PickTicketPrintDate>
<ProcessDate>2012-03-07</ProcessDate>
<TrackingNumber>444444555555522222211111</TrackingNumber>
<LoadNumber></LoadNumber>
<BillOfLading></BillOfLading>
<MasterBillOfLading></MasterBillOfLading>
<ASNSentDate />
<ConfirmASNSentDate></ConfirmASNSentDate>
<RememberRowInfo>11777:16:1::2010/11/19:173:True</RememberRowInfo>
</order>
<order>
<CustomerName>Triangle Media Corp</CustomerName>
<Facility>Chatsworth</Facility>
<WarehouseTransactionID>11777</WarehouseTransactionID>
<ReferenceNum>1000000</ReferenceNum>
<Retailer />
<ShipToCompanyName></ShipToCompanyName>
<MarkForName></MarkForName>
<BatchOrderID>3</BatchOrderID>
<CreationDate>2010-11-18T09:28:00</CreationDate>
<EarliestShipDate />
<ShipCancelDate />
<PickupDate />
<Carrier>USPS</Carrier>
<BillingCode>Prepaid</BillingCode>
<TotWeight>0.03</TotWeight>
<TotCuFt>0.00</TotCuFt>
<TotPackages>1.0000</TotPackages>
<TotOrdQty>1.0000</TotOrdQty>
<TotLines>1.00</TotLines>
<PONum></PONum>
<Notes>Ecig</Notes>
<OverAllocated></OverAllocated>
<PickTicketPrintDate>2010-11-19</PickTicketPrintDate>
<ProcessDate>2012-03-07</ProcessDate>
<TrackingNumber>999999342342111119942341</TrackingNumber>
<LoadNumber></LoadNumber>
<BillOfLading></BillOfLading>
<MasterBillOfLading></MasterBillOfLading>
<ASNSentDate />
<ConfirmASNSentDate></ConfirmASNSentDate>
<RememberRowInfo>11777:16:1::2010/11/19:173:True</RememberRowInfo>
</order>
<order>
<CustomerName>Triangle Media Corp</CustomerName>
<Facility>Chatsworth</Facility>
<WarehouseTransactionID>11777</WarehouseTransactionID>
<ReferenceNum>67771</ReferenceNum>
<Retailer />
<ShipToCompanyName></ShipToCompanyName>
<MarkForName></MarkForName>
<BatchOrderID>4</BatchOrderID>
<CreationDate>2010-11-18T09:28:00</CreationDate>
<EarliestShipDate />
<ShipCancelDate />
<PickupDate />
<Carrier>USPS</Carrier>
<BillingCode>Prepaid</BillingCode>
<TotWeight>0.03</TotWeight>
<TotCuFt>0.00</TotCuFt>
<TotPackages>1.0000</TotPackages>
<TotOrdQty>1.0000</TotOrdQty>
<TotLines>1.00</TotLines>
<PONum></PONum>
<Notes>Ecig</Notes>
<OverAllocated></OverAllocated>
<PickTicketPrintDate>2010-11-19</PickTicketPrintDate>
<ProcessDate>2012-03-07</ProcessDate>
<TrackingNumber></TrackingNumber>
<LoadNumber></LoadNumber>
<BillOfLading></BillOfLading>
<MasterBillOfLading></MasterBillOfLading>
<ASNSentDate />
<ConfirmASNSentDate></ConfirmASNSentDate>
<RememberRowInfo>11777:16:1::2010/11/19:173:True</RememberRowInfo>
</order>
</orders>
<% }
   else
   { 
    throw new Exception("ABF emulator. Check tracking number FAILURE");
 } %>