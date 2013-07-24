<!-- #include file="..\shared_include\connect.inc" -->
<!-- #include file="include\controls.inc" -->
<!-- #include file="include\date_filter.inc" -->
<!-- #include file="include\utility.inc" -->
<!-- #include file="..\shared_include\process_nmi.inc" -->
<%

dim reportMode
if (request("reportMode") = "prospects") then reportMode = "prospects" else reportMode = "orders"

Dim page_action
		page_action = Request("action")
		
	Dim billingIDs 
		billingIDs = Request("billingIDs")
		
	Dim billingIDArray
		billingIDArray = Split(billingIDS, ",")
	
	Dim bShippingBlocked, bScrub
	
	if Request("shippingblock") = "on" Then bShippingBlocked = True Else bShippingBlocked = False
	if Request("scrub") = "on" Then bScrub = True Else bScrub = False
		
	Dim strError
		strError = ""
		
	
	Dim action, chargeDate, nowDate, xml, strStatus, nBid '!!!DO NOT REMOVE, action variable is used in process_nmi.DoRefund
		if (page_action = "Update") then
			IF UBound(billingIDArray) < 1 THEN
				Adderror "No orders are currently selected. Please select one or more records and try again.", true
			ELSE
				OpenDatabase
				For i = LBound(billingIDArray) + 1 TO UBound(billingIDArray)
					UpdateScrub bScrub, billingIDArray(i)
					ShippingBlocked bShippingBlocked, billingIDArray(i)
				Next
				AddError "Succesfully updated", false				
			END IF
		elseif (page_action = "VoidDupe") then
			IF UBound(billingIDArray) < 1 THEN
				Adderror "No orders are currently selected. Please select one or more records and try again.", true
			ELSE
				OpenDatabase
				For i = LBound(billingIDArray) + 1 TO UBound(billingIDArray)
					ProcessVoidDupeOrTest false, billingIDArray(i)
				Next
				AddError "Succesfully updated", false
			END IF
		elseif (page_action = "VoidFraud") then
			IF UBound(billingIDArray) < 1 THEN
				Adderror "No orders are currently selected. Please select one or more records and try again.", true
			ELSE
				OpenDatabase
				For i = LBound(billingIDArray) + 1 TO UBound(billingIDArray)
					ProcessVoidFraud billingIDArray(i)
				Next	
				AddError "Succesfully updated", false				
			END IF
		elseif (page_action = "Scrub") Then
			IF UBound(billingIDArray) < 1 THEN
				Adderror "No orders are currently selected. Please select one or more records and try again.", true
			ELSE
				OpenDatabase
				For i = LBound(billingIDArray) + 1 TO UBound(billingIDArray)
					UpdateScrub true, billingIDArray(i)
				Next
				AddError "Succesfully scrubbed", false
			END IF
		''elseif (page_action = "VoidTest") then
			''ProcessVoidDupeOrTest true
		''elseif (page_action = "VoidFraud") then
			''ProcessVoidFraud
		end if
	

	function UpdateScrub(bScrub, nBillingID)
		DataConn.Execute "Delete From AffiliateScrub Where BillingID=" & nBillingID
		If bScrub Then
			DataConn.Execute "Insert into AffiliateScrub (BillingID, Reason, CreateDT) values(" & nBillingID & ", '', '" & mysqlNow() & "')"
			'DataConn.Execute "Update BillingSubscription Set StatusTID=8 where BillingID=" & nBillingID		
		End if 
	end function
	
	function ShippingBlocked(bShippingBlocked, nBillingID)
		DataConn.Execute "Delete From ShippingBlocked Where BillingID=" &  nBillingID
		If bShippingBlocked Then
			DataConn.Execute "Insert into ShippingBlocked (BillingID, CreateDT) values(" &  nBillingID & ",'" & mysqlNow() & "')"
		End if 
	end function
	
	function ProcessVoidDupeOrTest(isTest, nBillingID)
		DataCmd.CommandText = "select ch.ChargeHistoryID from Billing b " &_
			"inner join BillingSubscription bs on bs.BillingID = b.BillingID " &_
			"inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID " &_
			"where ch.Success = 1 and ch.Amount > 0 and b.BillingID = " & nBillingID & " " &_
			"order by ch.ChargeHistoryID desc " &_
			"limit 1"
		Set objRec = DataCmd.Execute
		if (objRec.RecordCount > 0) then
			Dim chId
			chId = objRec("ChargeHistoryID")
			ProcessVoid chId, nBillingID
		end if
		
		Dim note
		if (isTest) then 
			note = "Test Account"
			strScrubReason = "Test" 
		else 
			note = "Duplicate Account"
			strScrubReason = "Dupe" 
		end if
		
		DataConn.Execute "INSERT INTO Notes (BillingID, AdminID, Content, CreateDT) VALUES(" & nBillingID & ", 0, '" & note &"','" & mysqlNow() & "')"
		DataConn.Execute "UPDATE BillingSubscription SET StatusTID=8 WHERE BillingID=" & nBillingID
		UpdateScrub true, nBillingID
		ShippingBlocked true, nBillingID
	end function
	
	function ProcessVoid(chargeHistoryID, nBillingID)
		nBid = nBillingID
		DataCmd.CommandText = "select ch.Amount, chCur.CurrencyAmount, ch.TransactionNumber, ch.BillingSubscriptionID from BillingSubscription bs " &_
			"inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID " &_
			"left join ChargeHistoryExCurrency chCur on chCur.ChargeHistoryID = ch.ChargeHistoryID "&_
			"left join Currency cur on cur.CurrencyID = chCur.CurrencyID "&_
			"where ch.ChargeHistoryID = " & chargeHistoryID
		Set objRec = DataCmd.Execute
		if (objRec.RecordCount > 0) then
			Dim chargeHistoryAmount, chargeHistoryTransactionNumber, chargeHistoryBillingSubscriptionID
			chargeHistoryAmount = objRec("Amount")
			if (not IsNull(objRec("CurrencyAmount"))) then chargeHistoryAmount = objRec("CurrencyAmount")
			chargeHistoryTransactionNumber = objRec("TransactionNumber")
			chargeHistoryBillingSubscriptionID = objRec("BillingSubscriptionID")

			Dim strNMIRes, refundChargeHistoryID
			strNMIRes = DoRefund(chargeHistoryTransactionNumber, chargeHistoryAmount, nBillingID, chargeHistoryID)
			refundChargeHistoryID = ChargeLogging(action, MERCHANT_ACCOUNT_ID, chargeHistoryBillingSubscriptionID, 0 - CDbl(chargeHistoryAmount), strNMIRes)

			Dim currencyAmount, dollarsAmount
			currencyAmount = ""
			dollarsAmount = 0 - CDbl(chargeHistoryAmount)
			'variable from process_nmi
			if (currencyID > 0) then
				currencyAmount = dollarsAmount
				dollarsAmount = convertedAmount
			end if
			
			if IsGatewayResponseSuccess(GATEWAY_NAME, strNMIRes) then 
				AddError "SUCCESS: Void/Refund " & DisplayAmount(dollarsAmount, currencyAmount, currencyName, currencySymbol) & " was successfuly processed<br/>NMI:" & strNMIRes & "<br />", false

				Dim saleID
				DataCmd.CommandText = "select bs.SaleID from  BillingSale bs "&_		
					"where bs.ChargeHistoryID = " & chargeHistoryID &_
					" union select us.SaleID from UpsellSale us "&_
					"where us.ChargeHistoryID = " & chargeHistoryID
				set objrec = DataCmd.Execute
				do while (not objRec.EOF)
					saleID = CLng(objrec("SaleID"))
					DataConn.Execute "insert into SaleRefund (SaleID, ChargeHistoryID) values(" & saleId & "," & refundChargeHistoryID & ")"
					DataCmd.CommandText = "select * from ChargeHistoryExSale chs where chs.ChargeHistoryID = " & chargeHistoryID & " and SaleID = " & saleID
					set objRec2 = DataCmd.Execute
					if (objRec2.RecordCount > 0) then
						dim chsAmount, chsCurrencyAmount, chsCurrencyID
						if (IsNull(objRec2("Amount"))) then chsAmount = "NULL" else chsAmount = 0 - CDbl(objRec2("Amount"))
						if (IsNull(objRec2("CurrencyAmount"))) then chsCurrencyAmount = "NULL" else chsCurrencyAmount = 0 - CDbl(objRec2("CurrencyAmount"))
						if (IsNull(objRec2("CurrencyID"))) then chsCurrencyID = "NULL" else chsCurrencyID = objRec2("CurrencyID")
						DataConn.Execute "insert into ChargeHistoryExSale(ChargeHistoryID, SaleID, Amount, CurrencyID, CurrencyAmount) values(" & refundChargeHistoryID & ", " & saleId & ", " & chsAmount & ", " & chsCurrencyID & ", " & chsCurrencyAmount & ")"
					end if				
					
					objRec.MoveNext
				loop
			else
				AddError "ERROR: Void/Refund " & DisplayAmount(dollarsAmount, currencyAmount, currencyName, currencySymbol) & " was not processed<br/>NMI:" & strNMIRes, true
			end if
		else
			AddError "ERROR: Can't process void. ChargeHistoryEx(" + chargeHistoryID + ") was not found", true
		end if            
	end function
	
	function ProcessVoidFraud(nBillingID)
		DataCmd.CommandText = "select ch.ChargeHistoryID from Billing b " &_
			"inner join BillingSubscription bs on bs.BillingID = b.BillingID " &_
			"inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID " &_
			"where ch.Success = 1 and ch.Amount > 0 and b.BillingID = " & nBillingID & " " &_
			"order by ch.ChargeHistoryID desc "
		Set objRec = DataCmd.Execute
		if (objRec.RecordCount > 0) then
			Dim chIDs
			chIDs = ""
			while (not objRec.EOF)
				chIDs = chIDs & objRec("ChargeHistoryID") & ","
				objRec.MoveNext
			wend
			Dim arrCh, i
			arrCh = split(chIDs, ",")
			for i = 0 to UBound(arrCh) - 1
				ProcessVoid arrCh(i), nBillingID
			next		
		else
			AddError "Charges to Void were not found", false
		end if            

		Dim note
		note = "Fraud Account"
		strScrubReason = "Fraud" 
		
		DataConn.Execute "INSERT INTO Notes (BillingID, AdminID, Content, CreateDT) VALUES(" & nBillingID & ", 0, '" & note &"','" & mysqlNow() & "')"
		DataConn.Execute "UPDATE BillingSubscription SET StatusTID=8 WHERE BillingID=" & nBillingID
		UpdateScrub true, nBillingID
		ShippingBlocked true, nBillingID
		
		AddError "Account was set to 'Scrubbed' and excluded from Shipping and Affiliate Reporting", false
	end function
	
	function AddError(errorString, bError)
		if (bError) then 
			errorString = "<span class='small-alert'>" & errorString & "</span>"
		else
			errorString = "<span>" & errorString & "</span>"
		end if
		if (strError = "") then
			strError = errorString
		else
			strError = strError & "<div class='space'></div>" & errorString
		end if
	end function
	
    opendatabase
    opendatabase2

    Dim strStatusFilter
	Dim strCardTypeFilter
    Dim strState
    Dim nAffID
    Dim sql
    Dim nRecordsFound
    Dim strSubscriptionID
    Dim strUpsell
    Dim strDateType
    Dim hideTest
	Dim strTag
	Dim filterProductID

    strAff = Request("Affiliate")
	strFlow = Request("flow")
    strStatusFilter = Request("status")
	strCardTypeFilter = Request("cardType")
    strState = Request("state")
    strUpsell = Request("upsell")
    strSubscriptionID = Request("subscription")
    strDateType = Request("datetype")
	strTag = Request("tag")
	filterProductID = Request("filterProductID")
    hideTest = (request("hideTest") = "on")

    If Len(Request.Querystring) < 1 Then
        hideTest = True
    End if

    if strAff = "" then strAff = 0

    If strDateType = "" Then
	    strDateType = 1
    End if

    If Request("rep") = 1 Then
	    strAff = session("aff")
		strFlow = session("flow")
	    strStatusFilter = session("status")
		strCardTypeFilter = session("cardType")
	    strStart = session("start")
	    strEnd = session("end")
	    strUpsell = session("upsell")
	    strSubscriptionID = session("subscription")
	    strDateType = session("DateType")
    Else
	    session("aff") = strAff
		session("flow") = strFlow
	    session("status") = strStatusFilter
		session("cardType") = strCardTypeFilter
	    session("start") = strStart
	    session("end") = strEnd
	    session("upsell") = strUpsell		
	    session("subscription") = strSubscriptionID
	    session("datetype") = strDateType
    end if

    sql = ""
	
	if (len(strTag) > 1) then
		sql = sql & " inner join TagBillingLink tbl on tbl.BillingID = B.BillingID inner join Tag t on t.TagID = tbl.TagID " 
	end if
	
    If strUpsell = "1" Then
	    sql = sql & " inner join Upsell as u on B.billingid=u.billingid " 
    end if

    If strDateType = "1" Then
	    sql = sql & " Where B.CreateDT Between '" & mysqldatestr(strStart & " 12:00:00 AM") & "' and '" & mysqldatestr(strEnd & " 11:59:59 PM") & "'"
    End If
    If strDateType = "2" Then
	    sql = sql & " Where BS.NextBillDate Between '" & mysqldatestr(strStart & " 12:00:00 AM") & "' and '" & mysqldatestr(strEnd & " 11:59:59 PM") & "'"
    End If
    If strDateType = "3" Then
	    sql = sql & " Where BS.LastBillDate Between '" & mysqldatestr(strStart & " 12:00:00 AM") & "' and '" & mysqldatestr(strEnd & " 11:59:59 PM") & "'"
    End If
    If strDateType = "4" Then
	    sql = sql & " Inner Join ChargeHistoryEx c on c.BillingSubscriptionID=BS.BillingSubscriptionID Where DATEDIFF(c.ChargeDate, BS.CreateDT) > 1 and c.ChargeDate Between '" & mysqldatestr(strStart & " 12:00:00 AM") & "' and '" & mysqldatestr(strEnd & " 11:59:59 PM") & "' and c.Success=0 and c.ChargeTypeID=1 and c.Amount>10"
    End If
    If strDateType = "5" Then
	    sql = sql & " Inner Join ChargeHistoryEx c on c.BillingSubscriptionID=BS.BillingSubscriptionID Where DATEDIFF(c.ChargeDate, BS.CreateDT) > 1 and c.ChargeDate Between '" & mysqldatestr(strStart & " 12:00:00 AM") & "' and '" & mysqldatestr(strEnd & " 11:59:59 PM") & "' and c.Success=1 and c.ChargeTypeID=1 and c.Amount>10"
    End If

    if (len(strTag) > 1) then
        sql = sql & " and t.TagValue like '%" & strTag & "%'"
    end if

    if (hideTest) then
        sql = sql & " and B.CreditCard <> '*...............'"
    end if

    if (len(filterProductID) > 1) then
        sql = sql & " and S.ProductID = " & filterProductID
    end if

    If strAff<>"0" Then
        DataCmd.CommandText = "select Code from Affiliate where AffiliateID = " & CInt(strAff)
        set objrec = DataCmd.Execute
        While Not objRec.EOF
			    sql = sql & " and B.Affiliate='" & objrec("Code") & "'"
			    objRec.MoveNext
		    Wend
    ENd if

    if strFlow<>"" Then
	    sql = sql & " and B.CampaignID=" & strFlow
    End if
	
    if strStatusFilter<>"" Then
	    sql = sql & " and StatusTID=" & strStatusFilter
    End if
	
	if strCardTypeFilter<>"" Then
		sql = sql & " and PaymentTypeID=" & strCardTypeFilter
	End if

    if strSubscriptionID <> "" Then
	    sql = sql & " and BS.SubscriptionID=" & strSubscriptionID
    End if
%>
<!-- #include file="include\header_start.inc" -->
<title>Orders Report | Admin</title>
<link href="css/smoothness/jquery-ui-1.8.2.custom.css" rel="stylesheet" type="text/css"><link rel="stylesheet" href="css/autosuggest_inquisitor.css" type="text/css" media="screen" charset="utf-8" />
<style type="text/css">
	/* fix for diagrams panel */
	#diagrams-panel .clear {
		clear: none !important;
	}
</style>
<!--[if IE]><script type="text/javascript" src="js/excanvas.js"></script><![endif]-->
<script src="js/chart.js" type="text/javascript"></script>
<script type="text/javascript" src="js/dropdowntree.js" charset="utf-8"></script>
<script type="text/javascript" src="js/bsn.AutoSuggest_c_2.1.3.js" charset="utf-8"></script>
<script language="JavaScript" type="text/javascript">

<% LoadDateScript() %>
function fncMaxLen(val)
{
	if(val==10)
	{
		document.getElementById("search_query").value='';
		document.getElementById("search_query").maxLength=4;
	}
	else
	{
		document.getElementById("search_query").maxLength=1000;
	}
}
function goRecord()
{
	var id;
	
	id = document.getElementById("search_id").value;
	
		window.location = "billing_edit.asp?id=" + id;
		
}
function clearInput()
{
	document.getElementById("tbLastName").value = 
	document.getElementById("tbPhone").value =
	document.getElementById("tbAddress").value =
	document.getElementById("tbCity").value =
	document.getElementById("tbSaleID").value =
	document.getElementById("tbOrderID").value = "";
	for (var i = 0; i< document.getElementById("state").options.length; i++)
	{
		document.getElementById("state").options[i].selected = false;
			
	}
	document.getElementById("state").options[0].selected = true;
	document.getElementById("logicAnd").checked = true;
}
function only()
{
	var options_xml;
	var as_xml;
	as_xml = '';
	options_xml = '';
	/*
	if( document.getElementById("search_type").value==10)
	{
		alert(document.getElementById("search_type").value);
		return false;
	}
	else
	{*/
	options_xml = {
		script: function (input) { return "textfill.asp?id=" + document.getElementById("search_type").value + "&root=1&query=" + document.getElementById("search_query").value;},
		varname:"input",maxresults:35,callback: function (obj) { document.getElementById("search_id").value = obj.id; }
		
		};
//alert("textfill.asp?id=" + document.getElementById("search_type").value + "&root=1&query=" + document.getElementById("search_query").value);
		 as_xml = new bsn.AutoSuggest('search_query', options_xml);
		
	/*}*/

}
function showDiagrams()
{
	inlineDiagram("reg-diagram-container", "Registrations", "controls/registrations_diagram.asp", "POST", $("#formDiagrams").serialize());
	inlineDiagram("sub-diagram-container", "Subscriptions", "controls/subscriptions_diagram.asp", "POST", $("#formDiagrams").serialize());
}
$(document).ready(function(){
	$("#tabs").tabs({cookie:true});

	showDiagrams();
	
	$('#checkall').click(function () {
		$("#table input").attr('checked', this.checked);
		$('#billingIDs').val('');
		var billingIDs = '';
		$('.mark').each(function(){
			if (this.checked)
				billingIDs += ',' + $(this).val();
		});
		$('#billingIDs').val(billingIDs);
	});
	
	$('.mark').change(function(){
		$('#billingIDs').val('');
		var billingIDs = '';
		$('.mark').each(function(){
			if (this.checked)
				billingIDs += ',' + $(this).val();
		});
		$('#billingIDs').val(billingIDs);
	});
});
</script>
<!-- #include file="include\header_end.inc" -->
<div id="header">
	Orders Report
</div>
<div id="tabs" class="data">
    <ul>
		<li><a href="#tabs-1">Orders</a></li>
		<li><a href="#tabs-2">Prospects</a></li>
	</ul>
<div id="tabs-1">
<div id="toggle" class="section">
	<a href="#">
	<h1 style="text-align:left;">Registrations / Subscriptions Charts</h1>
	</a>
	<div class="hidden" id="diagrams-panel">
		<form id="formDiagrams" name="formDiagrams">
		<% LoadDateFilter() %>
		<div class="module">
			<h2>Affiliate</h2>
			<% LoadDdlAffWithAffAsValue(true) %>
		</div>
		</form>
		<div class="module">
			<h2>Charts</h2>
			<input type="button" name="btnDiagram" onclick="showDiagrams();" value="Show Charts">
		</div>
		<div style="clear:both;"></div>
		<div class="module" id="reg-diagram-container"><img src="images/loading2.gif" /></div>		
		<div class="module" id="sub-diagram-container"><img src="images/loading2.gif" /></div>		
		<div style="clear:both;"></div>
	</div>
	<form name='formReport' id='formReport' method='get'>
	<input type='hidden' name='go' id='go' value="go" />
	<a href="#">
	<h1 style="text-align:left;">Quick View</h1>
	</a>
	<% LoadDateFilter() %>
	<a href="#">
	<h1 style="text-align:left;">Refine By Order Details</h1>
	</a>
	<div class="hidden">
		<div class="module">
			<h2>Search type</h2>
            <select name='datetype' id='datetype'>
                <option <%If strDateType="1" Then Response.write " selected "%> value="1">Orders placed within this date range</option>
                <option <%If strDateType="2" Then Response.write " selected "%> value="2">Orders due to bill in this date range</option>
                <option <%If strDateType="3" Then Response.write " selected "%> value="3">Orders last billed within this date range</option>
                <option <%If strDateType="4" Then Response.write " selected "%> value="4">Rebill attempts failed in this date range</option>
                <option <%If strDateType="5" Then Response.write " selected "%> value="5">Rebill success in this date range</option>
            </select>
		</div>
		<div class="module">
			<h2>Test Orders</h2>
			<input type="checkbox" id="hideTest" name="hideTest" value="on" <% if hideTest then response.write("checked") %> /> Hide
		</div>
		<div class="module">
			<h2>Product Group</h2>
			<select name='filterProductID'>
	            <option value=''>All</option>
				<%=GetProductDropDown(filterProductID)%>
			</select>
		</div>
		<div class="module">
			<h2>Affiliate</h2>
			<select name='affiliate' id='affiliate'>
			    <option <% If strAff="0" Then Response.write " selected " %>value='0'>All</option>
<%
    DataCmd.CommandText = "Select a.* from Affiliate a "&_
        "inner join AffiliateDropDownItem ai on ai.AffiliateID = a.AffiliateID "&_
        "order by a.Code Asc"
    Set objRec = DataCmd.Execute
    While Not objRec.EOF
	    If CInt(obJRec("AffiliateID")) = CInt(strAff) Then
		    Response.write "<option selected value='" & objRec("AffiliateID") & "'>" & objRec("Code") & "</option>" & Vbcrlf
	    Else
		    Response.write "<option value='" & objRec("AffiliateID") & "'>" & objRec("Code") & "</option>" & Vbcrlf
	    End if
	    objRec.movenext
    wend
%>
			</select>
		</div>
		<div class="module">
			<h2>Flow</h2>
			<select name='flow' id='flow'>
			    <option value=''>All</option>
<%
    DataCmd.CommandText = "Select c.* from Campaign c "&_
        "inner join FlowDropDownItem fi on fi.CampaignID = c.CampaignID "&_
        "order by c.DisplayName Asc"
    Set objRec = DataCmd.Execute
    While Not objRec.EOF
	    If CStr(obJRec("CampaignID")) = CStr(strFlow) Then
		    Response.write "<option selected value='" & objRec("CampaignID") & "'>" & objRec("DisplayName") & "</option>" & Vbcrlf
	    Else
		    Response.write "<option value='" & objRec("CampaignID") & "'>" & objRec("DisplayName") & "</option>" & Vbcrlf
	    End if
	    objRec.movenext
    wend
%>
			</select>
		</div>
		<div class="module">
			<h2>Status</h2>
			<select name='status' id='status'>
<% if strStatusFilter="" Then %>
                <option value="" selected >All</option>
<% else %>
                <option value=""  >All</option>
<% end if %>
<%
    DataCmd.CommandText = "Select * From StatusType order by StatusTypeID Asc"
    Set objRec = DataCmd.Execute
    While Not objRec.EOF
	    if strStatusFilter="" Then strStatusFilter=-1

	    If cint(objRec("StatusTypeID")) = cint(strStatusFilter) Then
		    response.write "<option value='" & objRec("StatusTypeID") & "' selected>" & objRec("DisplayName") & "</option>" & Vbcrlf
	    Else
		    response.write "<option value='" & objRec("StatusTypeID") & "'>" & objRec("DisplayName") & "</option>" & Vbcrlf
	    end If
    	
	    objrec.MoveNext
    Wend
%>
			</select>
		</div>
		<div class="module">
			<h2>Card type</h2>
			<select name='cardtype' id='cardtype'>
<% if strCardTypeFilter="" Then %>
				<option value="" selected>All</option>
<% else %>
				<option value="">All</option>
<% end if %>
<%
	DataCmd.CommandText = "Select * From PaymentType"
    Set objRec = DataCmd.Execute
    While Not objRec.EOF
	    if strCardTypeFilter="" Then strCardTypeFilter=-1

	    If cint(objRec("PaymentTypeID")) = cint(strCardTypeFilter) Then
		    response.write "<option value='" & objRec("PaymentTypeID") & "' selected>" & objRec("DisplayName") & "</option>" & Vbcrlf
	    Else
		    response.write "<option value='" & objRec("PaymentTypeID") & "'>" & objRec("DisplayName") & "</option>" & Vbcrlf
	    end If
    	
	    objrec.MoveNext
    Wend
%>
			</select>
		</div>
		<div class="module">
			<h2>Purchase History</h2>
			<select name='upsell' id='upsell'>
                <option value= <%if strUpsell="" Then response.write " selected " %>>All</option>
                <option value=1 <%if strUpsell="1" Then response.write " selected " %>>Only Upsells</option>
			</select>
		</div>
		<div class="module" style="min-width:300px;">
			<h2>Billing Plan</h2>
			<table border="0" cellspacing="0" cellpadding="3" class="editForm">
			<% =GetSubscriptionDropdowns(strSubscriptionID) %>
			</table>
		</div>
		<div class="module">
			<h2>Smart Lookup</h2>
			<table border="0" cellspacing="0" cellpadding="3" class="editForm">
				<tr>
					<td>
						<select name='search_type' id='search_type' onchange="javascript:fncMaxLen(this.value);" class="maketree">
							<option value='1'>Last name</option>
							<option value='2'>First name</option>
							<option value='3'>Phone</option>
							<option value='4'>Email</option>
							<option value='5'>City</option>
							<%
							'For JMB
							if (APPLICATION_OWNER = APPLICATION_OWNER_JMB) then
							%>
								<option value='15'>Charge History ID</option>
								<option value='16'>Impulse ID</option>
							<%
							end if
							%>
							<%
							'For CoAction
							if (APPLICATION_OWNER = APPLICATION_OWNER_COACTION) then
							%>
								<option value='15'>Charge History ID</option>
							<%
							end if
							%>
							<option value='6'>Order ID</option>
							<option value='9'>Sale ID</option>
							<option value='7'>Cancel Code</option>
							<option value='8'>RMA</option>
							<option value='10'>Last 4 of Credit Card</option>
							<option value='11'>Merchant Transaction Number</option>
							<option value='12'>Chargeback Case Number</option>
							<%
								DataCmd.CommandText = "select * from Tag order by TagValue"
								Set objRec = DataCmd.Execute
								if (objRec.RecordCount > 0) then
								%>
								<option value='13' disabled="disabled">User Tag</option>
								<%
									while (not objRec.EOF)
										%>
										<option value='14;<%=objRec("TagID")%>'><%=objRec("TagValue")%></option>
										<%
										objRec.MoveNext
									wend
								end if
							%>
						</select>
					</td>
				</tr>
				<tr>
					<td>
						<input type='text' name='search_query' id='search_query' size='30'>&nbsp;<input type='button' onClick='goRecord();' value='Go'>
					</td>
				</tr>
			</table>
		</div>
		<div class="clear">
		</div>
	</div>
	<a href="#">
	<h1 style="text-align:left;">Refine By Customer</h1>
	</a>
	<div class="hidden">
		<div class="module">
			<h2>Address</h2>
			<table border="0" cellspacing="0" cellpadding="3" class="editForm">
				<tr>
					<td>Last name</td>
					<td><input type="text" name="lastName" id="tbLastName" value="<%= request("lastName") %>" /></td>
				</tr>
				<tr>
					<td>Phone</td>
					<td><input type="text" name="Phone" id="tbPhone" value="<%= request("Phone") %>" /></td>
				</tr>
				<tr>
					<td>City</td>
					<td><input type="text" name="City" id="tbCity" value="<%= request("City") %>" /></td>
				</tr>
				<tr>
					<td>State</td>
					<td>
					    <select name="state" id="state">
						    <option value="" selected="selected">Any</option>
							<% =GetStateDropdownFullName(strState) %>
						</select>
					</td>
				</tr>
				<tr>
					<td>Zip</td>
					<td><input type="text" name="Zip" id="tbZip" value="<%= request("Zip") %>" /></td>
				</tr>
				<tr>
					<td>Address</td>
					<td><input type="text" name="Address" id="tbAddress" value="<%= request("Address") %>" /></td>
				</tr>
				<tr>
					<td>Fields match</td>
					<td>
				        <label>
					        <input type="radio" name="logic" id="logicAnd" value="AND" <% if (request("logic") = "AND" or request("logic") = "") then response.Write("checked = 'checked'")  %> />
					        All
				        </label>
				        <label>
					        <input type="radio" name="logic" id="logicOr" value="OR" <% if (request("logic") = "OR") then response.Write("checked = 'checked'")  %> />
					        Any
				        </label>
					</td>
				</tr>
			</table>
		</div>
		<div class="module">
			<h2>Internal IDs</h2>
			<table border="0" cellspacing="0" cellpadding="3" class="editForm">
				<tr>
					<td>Order ID</td>
					<td><input type="text" name="OrderID" id="tbOrderID" value="<%= request("OrderID") %>" /></td>
				</tr>
				<tr>
					<td>Sale ID</td>
					<td><input type="text" name="SaleID" id="tbSaleID" value="<%= request("SaleID") %>" /></td>
				</tr>
			</table>
		</div>
		<div class="module">
			<h2>User Tag</h2>
			<table border="0" cellspacing="0" cellpadding="3" class="editForm">
				<tr>
					<td>Tag</td>
					<td><input type="text" name="Tag" id="tbTag" value="<%= strTag %>" /></td>
				</tr>
			</table>
		</div>
		<div class="clear">
		</div>
	</div>
	<a href="#">
	<h1 style="text-align:left;">Group Actions</h1>
	</a>
	<div class="hidden">		
		<input type="hidden" name="billingIDs" value="" id="billingIDs" />				
		<div class="module">
			
			<table style="float:left;border:0;">					
				<tr><td colspan="2"><u><b>Applies to all checked records:</b></u></td></tr>
				<tr><td>Scrub From Affiliate Reporting</td><td><input type="checkbox" id="scrubFromAffiliateReporting" name="shippingBlock"></td></tr>
				<tr><td>Exclude From Shipping</td><td><input type="checkbox" id="excludeFromShipping" name="scrub"></td></tr>
				<tr>
					<td></td>
					<td>
						<input type="submit" value="Update" id="Update" name="Action">
						<input type="submit" value="VoidDupe" id="VoidDupe" name="Action">
						<input type="submit" value="Scrub" id="Scrub" name="Action">
						<input type="submit" value="VoidFraud" id="VoidFraud" name="Action">							
					</td>
				</tr>
				<tr><td></td><td><div id="errorMsg" class="data" style="width:500px;"><%= strError %></div></td></tr>
			</table>		
		</div>
	</div>	
	</form>
</div>
<div id="buttons">
	<input type="button" value="Clear Fields" onclick='clearInput();return false;' />
	<input type="submit" value="Generate Report" onclick='formReport.submit();' />
</div>
<%
if (len(request("go")) > 0 or _
	page_action = "VoidDupe" or _
	page_action = "VoidFraud" or _
	page_action = "Scrub") then

    Dim lastName, city, phone, orderId, addSql, address, zip
    lastName = request("lastName")
    city = request("city")
    address = request("address")
    city = request("city")
    orderId = request("orderId")
    saleId = request("saleId")
    zip = request("zip")
    addSql = ""
    if (lastName <> "") then addSql = addSql & " #%% B.LastName LIKE '%" & lastName & "%'"
    if (city <> "") then addSql = addSql & " #%% B.City LIKE '%" & city & "%'"
    if (address <> "") then addSql = addSql & " #%% (B.Address1 LIKE '%" & address & "%' or B.Address2 LIKE '" & address & "%')" 
    if (strState <> "") then addSql = addSql & " #%% B.State = '" & strState & "'"
    if (phone <> "") then addSql = addSql & " #%% B.Phone LIKE '%" & phone & "%'"
    if (orderId <> "") then addSql = addSql & " #%% B.BillingID LIKE '" & orderId & "%'"
    if (saleId <> "") then 
	    addSql = addSql &_
	    " #%% (exists (select SaleID from  BillingSale where SaleID = "& saleId &" and BillingSubscriptionID = BS.BillingSubscriptionID)" &_
	    " OR exists(select SaleID from UpsellSale inner join Upsell on Upsell.UpsellID =UpsellSale.UpsellID"&_
	    " where SaleID = "& saleId &" and Upsell.BillingID = B.BillingID)) "
    end if
    if (zip <> "") then addSql = addSql & " #%% B.Zip LIKE '" & zip & "%'"	
    if (addSql <> "") then
	    addSql = replace(addSql, "#%%", "AND (",1,1)
	    addSql = replace(addSql, "#%%", request("Logic"))
	    addSql = addSql & ")"
	    sql = sql & addSql
    end if	

    strCC = Request("search_query")	

    strCC = Replace(strCC,"0",":")
    strCC = Replace(strCC,"1",".")
    strCC = Replace(strCC,"2","@")
    strCC = Replace(strCC,"3","#")
    strCC = Replace(strCC,"4","*")
    strCC = Replace(strCC,"5","-")
    strCC = Replace(strCC,"6","~")
    strCC = Replace(strCC,"7","!")
    strCC = Replace(strCC,"8","^")
    strCC = Replace(strCC,"9","$")

    Dim nCount
    nCount=0
    if Request("search_Type") = 7 Then
	    sql = " inner join BillingCancelCode as CCC on CCC.BillingID=B.BillingID where CancelCode='" & Request("search_query") & "'"
    ENd If
    if Request("search_Type") = 8 Then
	    sql = " inner join BillingRMA as RMA on RMA.BillingID=B.BillingID where RMA='" & Request("search_query") & "'"
    ENd If

    	
    if Request("search_Type") = 10 Then
	    sql = sql & " and B.creditcard like '%%%%%%%%%%%%" & strCC & "'"
    end if	
end if

if (reportMode = "orders" and ( _
	len(request("go")) > 0 or _
	page_action = "VoidDupe" or _
	page_action = "VoidFraud" or _
	page_action = "Scrub")) then

	ShowLoadingPanel()

	DataCmd.CommandText = "Select distinct BS.StatusTID, ST.DisplayName as StatusName, camp.DisplayName as CampaignName, BS.LastBillDate, B.*, pc.ClickID as CID, fs.FraudScore"&_
		" from Billing as B"&_
	    " Inner Join BillingSubscription as BS on B.BillingID=BS.BillingID"&_
	    " Inner Join StatusType as ST on BS.StatusTID=ST.StatusTypeID"&_
	    " left join FraudScore fs on fs.BillingID = B.BillingID "&_
	    " Inner Join Subscription as S on BS.SubscriptionID=S.SubscriptionID"&_
	    " left join PartnerClick pc on pc.BillingID = B.BillingId"&_
	    " left join Campaign as camp on camp.campaignid=B.campaignid " & sql & " order by B.billingid desc" 

    'DataCmd.CommandText = "Select BS.StatusTID, ST.DisplayName as StatusName, S.DisplayName as CampaignName, BS.LastBillDate, B.*, pc.ClickID as CID from Billing as B Inner Join BillingSubscription as BS on B.BillingID=BS.BillingID Inner Join StatusType as ST on BS.StatusTID=ST.StatusTypeID Inner Join Subscription as S on BS.SubscriptionID=S.SubscriptionID left join PartnerClick pc on pc.BillingID = b.BillingId " & sql & " order by b.createdt desc" 
    Response.write "<!--" & DataCmd.commandText & "-->"
    Set objRec = DataCmd.Execute
    nRecordsFound = objRec.RecordCount
%>

<div class="data">
	<h1>Records Found: <%=nRecordsFound%></h1>
	<table class="process-offets sortable add-csv-export" border="0" cellspacing="1" width="100%" id="table">
		<tr class="header">
			<td>#</td>
			<td class="sorttable_nosort"><input type="checkbox" id="checkall" name="checkall" /></td>
			<td>BID</td>
			<td>Flow</td>
			<td>Fraud Score</td>
			<td>Status</td>
			<td>Order Date</td>
			<td>First Name</td>
			<td>Last Name</td>
			<td>Address</td>
            <td>City</td>
            <td>State</td>
            <td>Zip</td>
            <td>Country</td>
            <td>Phone</td>
            <td>Email</td>
            <!--<td>Last Billed DT</td>-->
            <td>Affiliate</td>
            <td>SubID</td>
		</tr>
<%
	Dim nI
		nI = 0

	While Not objRec.EOF

	nI = nI + 1
%>		
		<tr>
			<td><%= nI %></td>			
			<td><input type="checkbox" id="check_<%=objRec("BillingID")%>" value="<%=objRec("BillingID")%>" class="mark" /></td>
            <td><a href='billing_edit.asp?id=<%=objRec("BillingID")%>'><%=objRec("BillingID")%></a></td>
            <td nowrap><%=Left(objRec("CampaignName"),25)%></td>
            <td nowrap><%=objrec("FraudScore")%></td>
            <td><%=objRec("StatusName")%></td>
            <td nowrap><%=objRec("CreateDT")%></td>
            <td><%=objRec("FirstName")%></td>
            <td><%=objRec("LastName")%></td>
            <td><%=objRec("Address1")%> <%=objRec("Address2")%></td>
            <td><%=objRec("City")%></td>
            <td><%=objRec("State")%></td>
            <td><%=objRec("zip")%></td>
            <td><%=objRec("Country")%></td>
            <td><%=objRec("Phone")%></td>
            <td><%=objRec("Email")%></td>
            <!--<td><%=objRec("LastBillDate")%></td>-->
            <td><%=objRec("Affiliate")%></td>
            <td><%=objRec("SubAffiliate")%></td>
        </tr>
<%
	    objrec.movenext
    wend
%>
	</table>
	</div>
<%
end if
%>
<div class="space"></div>
</div>
<div id="tabs-2">
<div id="toggle" class="section">
	<form name='formReport2' id='formReport2' method='get'>
	<input type='hidden' name='go' id='go' value="go" />
	<input type='hidden' name='reportMode' id='reportMode' value="prospects" />
	<a href="#">
	<h1 style="text-align:left;">Quick View</h1>
	</a>
	<% LoadDateFilter() %>
	<a href="#">
	<h1 style="text-align:left;">Refine By Order Details</h1>
	</a>
	<div class="hidden">
		<input type="hidden" id="hideTest" name="hideTest" value="" />
		<div class="module">
			<h2>Affiliate</h2>
			<select name='affiliate' id='affiliate'>
			    <option <% If strAff="0" Then Response.write " selected " %>value='0'>All</option>
<%
    DataCmd.CommandText = "Select a.* from Affiliate a "&_
        "inner join AffiliateDropDownItem ai on ai.AffiliateID = a.AffiliateID "&_
        "order by a.Code Asc"
    Set objRec = DataCmd.Execute
    While Not objRec.EOF
	    If CInt(obJRec("AffiliateID")) = CInt(strAff) Then
		    Response.write "<option selected value='" & objRec("AffiliateID") & "'>" & objRec("Code") & "</option>" & Vbcrlf
	    Else
		    Response.write "<option value='" & objRec("AffiliateID") & "'>" & objRec("Code") & "</option>" & Vbcrlf
	    End if
	    objRec.movenext
    wend
%>
			</select>
		</div>
		<div class="module">
			<h2>Flow</h2>
			<select name='flow' id='flow'>
			    <option value=''>All</option>
<%
    DataCmd.CommandText = "Select c.* from Campaign c "&_
        "inner join FlowDropDownItem fi on fi.CampaignID = c.CampaignID "&_
        "order by c.DisplayName Asc"
    Set objRec = DataCmd.Execute
    While Not objRec.EOF
	    If CStr(obJRec("CampaignID")) = CStr(strFlow) Then
		    Response.write "<option selected value='" & objRec("CampaignID") & "'>" & objRec("DisplayName") & "</option>" & Vbcrlf
	    Else
		    Response.write "<option value='" & objRec("CampaignID") & "'>" & objRec("DisplayName") & "</option>" & Vbcrlf
	    End if
	    objRec.movenext
    wend
%>
			</select>
		</div>
		<div class="clear">
		</div>
	</div>
	<a href="#">
	<h1 style="text-align:left;">Refine By Customer</h1>
	</a>
	<div class="hidden">
		<div class="module">
			<h2>Address</h2>
			<table border="0" cellspacing="0" cellpadding="3" class="editForm">
				<tr>
					<td>Last name</td>
					<td><input type="text" name="lastName" id="tbLastName" value="<%= request("lastName") %>" /></td>
				</tr>
				<tr>
					<td>Phone</td>
					<td><input type="text" name="Phone" id="tbPhone" value="<%= request("Phone") %>" /></td>
				</tr>
				<tr>
					<td>City</td>
					<td><input type="text" name="City" id="tbCity" value="<%= request("City") %>" /></td>
				</tr>
				<tr>
					<td>State</td>
					<td>
					    <select name="state" id="state">
						    <option value="" selected="selected">Any</option>
							<% =GetStateDropdownFullName(strState) %>
						</select>
					</td>
				</tr>
				<tr>
					<td>Zip</td>
					<td><input type="text" name="Zip" id="tbZip" value="<%= request("Zip") %>" /></td>
				</tr>
				<tr>
					<td>Address</td>
					<td><input type="text" name="Address" id="tbAddress" value="<%= request("Address") %>" /></td>
				</tr>
				<tr>
					<td>Fields match</td>
					<td>
				        <label>
					        <input type="radio" name="logic" id="logicAnd" value="AND" <% if (request("logic") = "AND" or request("logic") = "") then response.Write("checked = 'checked'")  %> />
					        All
				        </label>
				        <label>
					        <input type="radio" name="logic" id="logicOr" value="OR" <% if (request("logic") = "OR") then response.Write("checked = 'checked'")  %> />
					        Any
				        </label>
					</td>
				</tr>
			</table>
		</div>
		<div class="module">
			<h2>User Tag</h2>
			<table border="0" cellspacing="0" cellpadding="3" class="editForm">
				<tr>
					<td>Tag</td>
					<td><input type="text" name="Tag" id="tbTag" value="<%= strTag %>" /></td>
				</tr>
			</table>
		</div>
		<div class="clear">
		</div>
	</div>
	</form>
</div>
<div id="buttons">
	<input type="button" value="Clear Fields" onclick='clearInput();return false;' />
	<input type="submit" value="Generate Report" onclick='formReport2.submit();' />
</div>
<%
if (len(request("go")) > 0 and reportMode = "prospects") then

	ShowLoadingPanel()

	'DataCmd.CommandText = "Select distinct camp.DisplayName as CampaignName, B.*"&_
	'	" from Registration as B"&_
	'    " left join Campaign as camp on camp.campaignid=B.campaignid " & sql & " and not exists (select * from BillingSubscription inner join Billing on Billing.BillingID = BillingSubscription.BillingID where Billing.RegistrationID = B.RegistrationID) order by B.RegistrationID desc" 
		'" left join Billing bil on bil.RegistrationID = B.RegistrationID"&_

	'DataCmd.CommandText = "Select distinct camp.DisplayName as CampaignName, B.*"&_
	'	" from Billing as B"&_
	'    " left join Campaign as camp on camp.campaignid=B.campaignid " & sql & " and not exists (select * from BillingSubscription where BillingSubscription.BillingID = B.BillingID) order by B.BillingID desc" 
	
	Dim rSql
	
	rSql = Replace(sql,"B.","R.")
	
	DataCmd.CommandText = "Select * from (" &_ 
		"Select B.BillingID as BillingID, camp.DisplayName as CampaignName, B.CreateDT, B.FirstName, B.LastName, B.Address1, B.Address2, B.City, B.State, B.Zip, B.Country, B.Phone, B.Email, B.Affiliate, B.SubAffiliate from Billing as B " &_
		"left join Campaign as camp on camp.campaignid=B.campaignid " & sql & " and not exists (select * from BillingSubscription where BillingSubscription.BillingID = B.BillingID)" &_   
		"union all " &_
		"Select null as BillingID, camp.DisplayName as CampaignName, R.CreateDT, R.FirstName, R.LastName, R.Address1, R.Address2, R.City, R.State, R.Zip, null as Country, R.Phone, R.Email, R.Affiliate, R.SubAffiliate from Registration as R " &_
		"left join Campaign as camp on camp.campaignid=R.campaignid " & rSql & "  and R.Email != '' and not exists ( select * from Billing where Billing.RegistrationID = R.RegistrationID ) " &_
		") as q Order By q.CreateDT DESC"		
		
    Response.write "<!--" & DataCmd.commandText & "-->"
    Set objRec = DataCmd.Execute
    nRecordsFound = objRec.RecordCount
%>

<div class="data">
	<h1>Records Found: <%=nRecordsFound%></h1>
	<table class="process-offets sortable add-csv-export" border="0" cellspacing="1" width="100%" id="table">
		<tr class="header">
			<td>#</td>
			<td>BID</td>
			<td>Flow</td>
			<td>Date</td>
			<td>First Name</td>
			<td>Last Name</td>
			<td>Address</td>
            <td>City</td>
            <td>State</td>
            <td>Zip</td>
            <td>Country</td>
            <td>Phone</td>
            <td>Email</td>
            <td>Affiliate</td>
            <td>SubID</td>
		</tr>
<%
		nI = 0

	While Not objRec.EOF

	nI = nI + 1
%>		
		<tr>
			<td><%= nI %></td>			
            <td><a href='billing_edit.asp?id=<%=objRec("BillingID")%>'><%=objRec("BillingID")%></a></td>
            <td nowrap><%=Left(objRec("CampaignName"),25)%></td>
            <td nowrap><%=objRec("CreateDT")%></td>
            <td><%=objRec("FirstName")%></td>
            <td><%=objRec("LastName")%></td>
            <td><%=objRec("Address1")%> <%=objRec("Address2")%></td>
            <td><%=objRec("City")%></td>
            <td><%=objRec("State")%></td>
            <td><%=objRec("zip")%></td>
            <td><%=objRec("Country")%></td>
            <td><%=objRec("Phone")%></td>
            <td><%=objRec("Email")%></td>
            <td><%=objRec("Affiliate")%></td>
            <td><%=objRec("SubAffiliate")%></td>
        </tr>
<%
	    objrec.movenext
    wend
%>
	</table>
	</div>
<%
end if
%>
<div class="space"></div>
</div>
<input type='hidden' name='search_id' id='search_id' />
<script>

//showAllData();
only();

</script>

<!-- #include file="include\footer.inc" -->
