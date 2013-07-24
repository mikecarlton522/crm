<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Hosted_Payment_page.aspx.cs"
    Inherits="Payment_test._Default" %>

<%@ Register TagPrefix="gen" Assembly="TrimFuel.Business" Namespace="TrimFuel.Business.Controls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Hosted Payment</title>
    <link href="/css/style.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <div class="wrapper">
        <form id="hostedForm" runat="server">
        <div class="logo" style="border-bottom: 3px solid #b9b9b9; text-align: left; clear: both;">
            <img src="images/logo.png" />
            <h2>
                <span class="label title">Hosted Payment </span>
            </h2>
        </div>
        <div id="DivGateWayError" visible="false" runat="server" style="margin: 10px 0; padding: 10px 0;
            border-top: 3px solid #b9b9b9; border-bottom: 3px solid #a5a5a5; text-align: left;
            clear: both;">
            <asp:Label ID="LabelGateWayError" runat="server" Text=""></asp:Label>
        </div>
        <div style="margin: 0 10px 10px 0; padding: 10px; float: left; font-size: 12px; min-height: 47px;">
            <asp:PlaceHolder ID="phRegistration" runat="server" Visible='<%# string.IsNullOrEmpty(nvc["RegistrationID"]) %>' >
                <div style="margin: 0 10px 10px 0; padding: 10px; border: 1px solid #a5a5a5; float: left;
                    font-size: 12px; min-height: 47px;">
                    <asp:Panel ID="Panel_ship" runat="server">
                        <asp:Label ID="Ship_label" Font-Bold="true" runat="server" CssClass="label underline"
                            Text="Shipping Address"></asp:Label>
                        <asp:Table ID="Table1" runat="server"
                            CssClass="data">
                            <asp:TableRow>
                                <asp:TableCell VerticalAlign="Top"><span class="label">First Name</span></asp:TableCell><asp:TableCell>
                                    <asp:TextBox BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" MaxLength="50"
                                        ID="tbx_ship_FirstName" runat="server"></asp:TextBox>
                                    <br />
                                    <asp:CustomValidator runat="server" ID="cusCustom_ship_FN" ValidateEmptyText="True"
                                        ValidationGroup="GO" Display="Dynamic" ControlToValidate="tbx_ship_FirstName"
                                        OnServerValidate="Two_Letters_Validate" ErrorMessage="Please, enter a valid first name"
                                        Enabled='<%# string.IsNullOrEmpty(nvc["RegistrationID"]) %>' />
                                </asp:TableCell></asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell VerticalAlign="Top"><span class="label">Last Name</span></asp:TableCell><asp:TableCell>
                                    <asp:TextBox BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" MaxLength="50"
                                        ID="tbx_ship_LastName" runat="server"></asp:TextBox>
                                    <br />
                                    <asp:CustomValidator runat="server" ID="CustomValidator1" ValidateEmptyText="True"
                                        ValidationGroup="GO" Display="Dynamic" ControlToValidate="tbx_ship_LastName"
                                        OnServerValidate="Two_Letters_Validate" ErrorMessage="Please, enter a valid last name"
                                        Enabled='<%# string.IsNullOrEmpty(nvc["RegistrationID"]) %>' />
                                </asp:TableCell></asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell VerticalAlign="Top"><span class="label">Address 1</span></asp:TableCell><asp:TableCell>
                                    <asp:TextBox BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" MaxLength="50"
                                        ID="tbx_ship_Addr1" runat="server"></asp:TextBox>
                                    <br />
                                    <asp:CustomValidator runat="server" ID="CustomValidator2" ValidateEmptyText="True"
                                        ValidationGroup="GO" Display="Dynamic" ControlToValidate="tbx_ship_Addr1" OnServerValidate="Two_LettersNumbers_Validate"
                                        ErrorMessage="Please, enter valid a first address" Enabled='<%# string.IsNullOrEmpty(nvc["RegistrationID"]) %>' />
                                </asp:TableCell></asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell><span class="label">Address 2</span></asp:TableCell><asp:TableCell>
                                    <asp:TextBox BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" MaxLength="50"
                                        ID="tbx_ship_Addr2" runat="server"></asp:TextBox>
                                </asp:TableCell></asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell VerticalAlign="Top"><span class="label">City</span></asp:TableCell><asp:TableCell>
                                    <asp:TextBox BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" MaxLength="50"
                                        ID="tbx_ship_City" runat="server"></asp:TextBox>
                                    <br />
                                    <asp:CustomValidator runat="server" ID="CustomValidator3" ValidateEmptyText="True"
                                        ValidationGroup="GO" Display="Dynamic" ControlToValidate="tbx_ship_City" OnServerValidate="Two_LettersNumbers_Validate"
                                        ErrorMessage="Please, enter a valid City" Enabled='<%# string.IsNullOrEmpty(nvc["RegistrationID"]) %>' />
                                </asp:TableCell></asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell><span class="label">State</span></asp:TableCell><asp:TableCell>
                                    <asp:DropDownList BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" ID="DDL_ship_State"
                                        Width="130" runat="server">
                                    </asp:DropDownList>
                                    <asp:TextBox BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" MaxLength="50"
                                        ID="tbx_ship_state" runat="server"></asp:TextBox>
                                </asp:TableCell></asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell><span class="label">Zip</span></asp:TableCell><asp:TableCell>
                                    <asp:TextBox BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" MaxLength="50"
                                        ID="tbx_ship_zip" runat="server"></asp:TextBox>
                                </asp:TableCell></asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell><span class="label">Country</span></asp:TableCell><asp:TableCell>
                                    <asp:DropDownList AutoPostBack="true" OnSelectedIndexChanged="ChangeShipState" BorderStyle="Solid"
                                        BorderWidth="1" BorderColor="Gray" ID="DDL_ship_Country" Width="130" runat="server">
                                    </asp:DropDownList>
                                </asp:TableCell></asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell><span class="label">Phone</span></asp:TableCell><asp:TableCell>
                                    <asp:TextBox BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" MaxLength="50"
                                        ID="tbx_ship_phone" runat="server"></asp:TextBox>
                                </asp:TableCell></asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell VerticalAlign="Top"><span class="label">Email</span></asp:TableCell><asp:TableCell>
                                    <asp:TextBox BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" MaxLength="50"
                                        ID="tbx_ship_Email" runat="server"></asp:TextBox>
                                    <br />
                                    <asp:CustomValidator runat="server" ID="CustomValidator4" ValidateEmptyText="True"
                                        ValidationGroup="GO" Display="Dynamic" ControlToValidate="tbx_ship_Email" OnServerValidate="Email_Validate"
                                        ErrorMessage="Please, enter a valid email" Enabled='<%# string.IsNullOrEmpty(nvc["RegistrationID"]) %>' />
                                </asp:TableCell></asp:TableRow>
                        </asp:Table>
                        <asp:Button ID="CopyShiptoBill" ValidationGroup="asd" runat="server" OnClick="CopyShiptoBill_Click"
                            Text="Copy to Billing Address" />
                    </asp:Panel>
                </div>
            </asp:PlaceHolder>
            <div style="margin: 0 10px 10px 0; padding: 10px; border: 1px solid #a5a5a5; float: left;
                font-size: 12px; min-height: 47px;">
                <asp:Panel ID="Panel_bill" runat="server">
                    <asp:Label ID="label_bill" Font-Bold="true" runat="server" CssClass="label underline"
                        Text="Billing Address"></asp:Label>
                    <asp:Table ID="Table2" border="0" runat="server" CssClass="data">
                        <asp:TableRow>
                            <asp:TableCell VerticalAlign="Top"><span class="label">First Name</span></asp:TableCell><asp:TableCell>
                                <asp:TextBox BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" MaxLength="50"
                                    ID="tbx_bill_FirstName" runat="server"></asp:TextBox>
                                <br />
                                <asp:CustomValidator runat="server" ID="CustomValidator5" ValidateEmptyText="True"
                                    ValidationGroup="GO" Display="Dynamic" ControlToValidate="tbx_bill_FirstName"
                                    OnServerValidate="Two_Letters_Validate" ErrorMessage="Please, enter a valid first name" />
                            </asp:TableCell></asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell VerticalAlign="Top"><span class="label">Last Name</span></asp:TableCell><asp:TableCell>
                                <asp:TextBox BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" MaxLength="50"
                                    ID="tbx_bill_LastName" runat="server"></asp:TextBox>
                                <br />
                                <asp:CustomValidator runat="server" ID="CustomValidator6" ValidateEmptyText="True"
                                    ValidationGroup="GO" Display="Dynamic" ControlToValidate="tbx_bill_LastName"
                                    OnServerValidate="Two_Letters_Validate" ErrorMessage="Please, enter a valid last name" />
                            </asp:TableCell></asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell VerticalAlign="Top"><span class="label">Address 1</span></asp:TableCell><asp:TableCell>
                                <asp:TextBox BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" MaxLength="50"
                                    ID="tbx_bill_Addr1" runat="server"></asp:TextBox>
                                <br />
                                <asp:CustomValidator runat="server" ID="CustomValidator7" ValidateEmptyText="True"
                                    ValidationGroup="GO" Display="Dynamic" ControlToValidate="tbx_bill_Addr1" OnServerValidate="Two_LettersNumbers_Validate"
                                    ErrorMessage="Please, enter a valid first address" />
                            </asp:TableCell></asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell><span class="label">Address 2</span></asp:TableCell><asp:TableCell>
                                <asp:TextBox BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" MaxLength="50"
                                    ID="tbx_bill_Addr2" runat="server"></asp:TextBox>
                            </asp:TableCell></asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell VerticalAlign="Top"><span class="label">City</span></asp:TableCell><asp:TableCell>
                                <asp:TextBox BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" MaxLength="50"
                                    ID="tbx_bill_City" runat="server"></asp:TextBox>
                                <br />
                                <asp:CustomValidator runat="server" ID="CustomValidator8" ValidateEmptyText="True"
                                    ValidationGroup="GO" Display="Dynamic" ControlToValidate="tbx_bill_City" OnServerValidate="Two_LettersNumbers_Validate"
                                    ErrorMessage="Please, enter a valid City" />
                            </asp:TableCell></asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell><span class="label">State</span></asp:TableCell><asp:TableCell>
                                <asp:DropDownList BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" ID="DDL_Bill_State"
                                    Width="130" runat="server">
                                </asp:DropDownList>
                                <asp:TextBox BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" MaxLength="50"
                                    ID="tbx_Bill_State" runat="server"></asp:TextBox>
                            </asp:TableCell></asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell><span class="label">Zip</span></asp:TableCell><asp:TableCell>
                                <asp:TextBox BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" MaxLength="50"
                                    ID="tbx_bill_zip" runat="server"></asp:TextBox>
                            </asp:TableCell></asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell><span class="label">Country</span></asp:TableCell><asp:TableCell>
                                <asp:DropDownList OnSelectedIndexChanged="ChangeBillState" AutoPostBack="true" BorderStyle="Solid"
                                    BorderWidth="1" BorderColor="Gray" ID="DDL_Bill_country" Width="130" runat="server">
                                </asp:DropDownList>
                            </asp:TableCell></asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell><span class="label">Phone</span></asp:TableCell><asp:TableCell>
                                <asp:TextBox BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" MaxLength="50"
                                    ID="tbx_bill_phone" runat="server"></asp:TextBox>
                            </asp:TableCell></asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell VerticalAlign="Top"><span class="label">Email</span></asp:TableCell><asp:TableCell>
                                <asp:TextBox BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" MaxLength="50"
                                    ID="tbx_bill_Email" runat="server"></asp:TextBox>
                                <br />
                                <asp:CustomValidator runat="server" ID="CustomValidator9" ValidateEmptyText="True"
                                    ValidationGroup="GO" Display="Dynamic" ControlToValidate="tbx_bill_Email" OnServerValidate="Email_Validate"
                                    ErrorMessage="Please, enter a valid email" />
                            </asp:TableCell></asp:TableRow>
                    </asp:Table>
                    <asp:Button ID="CopyBilltShip" ValidationGroup="asd" runat="server" OnClick="CopyBilltoShip_Click"
                        Text="Copy to Shipping Address" Visible='<%# string.IsNullOrEmpty(nvc["RegistrationID"]) %>' />
                </asp:Panel>
            </div>
            <div style="margin: 0 10px 10px 0; float: left; font-size: 12px; min-height: 47px;">
                <div style="margin: 0 10px 10px 0; padding: 10px; border: 1px solid #a5a5a5; font-size: 12px;
                    min-height: 47px;">
                    <div>
                        <span class="label underline">Product Info</span>
                        <table class="data">
                            <asp:Repeater runat="server" DataSource="<%# Products %>">
                                <HeaderTemplate>
                                    <tr>
                                        <td style="width: 200px;">
                                            <span class="label">Product Name</span>
                                        </td>
                                        <td>
                                            <span class="label">Product Price</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 5px;" colspan="2">
                                        </td>
                                    </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <span class="label">
                                                <%#Eval("Key") %></span>
                                        </td>
                                        <td>
                                            <span class="label">
                                                <%# String.Format("${0:f2}", Eval("Value"))%></span>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <tr>
                                        <td style="height: 5px;" colspan="2">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span class="label">Total</span>
                                        </td>
                                        <td>
                                            <span class="label">
                                                <%# String.Format("${0:f2}", Total)%></span>
                                        </td>
                                    </tr>
                                </FooterTemplate>
                            </asp:Repeater>
                        </table>
                    </div>
                </div>
                <div style="margin: 0 10px 10px 0; padding: 10px; border: 1px solid #a5a5a5; font-size: 12px;
                    min-height: 47px;">
                    <asp:Panel ID="CC" runat="server">
                        <asp:Label ID="Label1" Font-Bold="true" runat="server"><span class="label underline">Credit Card</span></asp:Label>
                        <asp:Table ID="Table3" runat="server" CssClass="data">
                            <asp:TableRow>
                                <asp:TableCell><span class="label">Card Type</span></asp:TableCell><asp:TableCell>
                                    <asp:DropDownList BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" ID="DDL_CC_CardType"
                                        Width="130" runat="server">
                                        <asp:ListItem Text="Visa" Value="2" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Mastercard" Value="3"></asp:ListItem>
                                        <asp:ListItem Text="Amex" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="Discover" Value="4"></asp:ListItem>
                                    </asp:DropDownList>
                                </asp:TableCell></asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell><span class="label">Credit Card Number</span></asp:TableCell><asp:TableCell>
                                    <asp:TextBox BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" MaxLength="16"
                                        ID="tbx_cc_number" runat="server"></asp:TextBox>
                                    <br />
                                    <asp:CustomValidator runat="server" ID="CustomValidator10" ValidationGroup="GO" Display="Dynamic"
                                        ControlToValidate="tbx_cc_number" OnServerValidate="CredCard_Validate" ErrorMessage="Please, enter a valid Credit Card"
                                        ValidateEmptyText="True" />
                                </asp:TableCell></asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell><span class="label">Security Code</span></asp:TableCell><asp:TableCell>
                                    <asp:TextBox BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" MaxLength="5"
                                        ID="tbx_cc_cvv" runat="server" TextMode="Password"></asp:TextBox>
                                </asp:TableCell></asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell VerticalAlign="Top"><span class="label">Expiration (Month/Year) 	</span></asp:TableCell><asp:TableCell>
                                    <gen:DDLMonth runat="server" ID="ddlMonth" TabIndex="6" Style="width: 100px;" />
                                    <%--<asp:TextBox BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" ID="tbx_cc_month"
                                MaxLength="2" Width="60" runat="server"></asp:TextBox>--%>
                                    /
                                    <gen:DDLYear runat="server" ID="ddlYear" TabIndex="6" Style="width: 100px;" />
                                    <%--<asp:TextBox BorderStyle="Solid" BorderWidth="1" BorderColor="Gray" ID="tbx_cc_year"
                                MaxLength="4" Width="62" runat="server"></asp:TextBox>--%>
                                    <%--<br />
                            <asp:CustomValidator runat="server" ID="CustomValidator11" ValidationGroup="GO" Display="Dynamic"
                                ControlToValidate="tbx_cc_month" OnServerValidate="Month_number_Validate" ErrorMessage="Please, enter a valid month"
                                ValidateEmptyText="True" />
                            <br />
                            <asp:CustomValidator runat="server" ID="CustomValidator12" ValidationGroup="GO" Display="Dynamic"
                                ControlToValidate="tbx_cc_year" OnServerValidate="Year_Validate" ErrorMessage="Please, enter a valid year"
                                ValidateEmptyText="True" />--%>
                                </asp:TableCell></asp:TableRow>
                        </asp:Table>
                    </asp:Panel>
                </div>
            </div>
        </div>
        <div style="margin: 10px 0; padding: 10px 0; border-top: 3px solid #b9b9b9; text-align: right; padding-right:30px;
            clear: both;">
            <asp:Button ID="BILL_USER" runat="server" ValidationGroup="GO" Text="Process Payment"
                OnClick="BILL_USER_Click" />
        </form>
    </div>
</body>
</html>
