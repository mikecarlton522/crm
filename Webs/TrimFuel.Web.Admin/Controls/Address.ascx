<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Address.ascx.cs" Inherits="TrimFuel.Web.Admin.Controls.Address" %>
<%@ Register TagPrefix="gen" Assembly="TrimFuel.Business" Namespace="TrimFuel.Business.Controls" %>
<tr><td>First Name:</td><td><asp:TextBox runat="server" ID="tbFirstName" MaxLength="100" CssClass="validate[custom[FirstName]]"></asp:TextBox></td></tr>
<tr><td>Last Name:</td><td><asp:TextBox runat="server" ID="tbLastName" MaxLength="100" CssClass="validate[custom[LastName]]"></asp:TextBox></td></tr>
<tr><td>Address 1:</td><td><asp:TextBox runat="server" ID="tbAddress1" MaxLength="50" CssClass="validate[custom[Address]]"></asp:TextBox></td></tr>
<tr><td>Address 2:</td><td><asp:TextBox runat="server" ID="tbAddress2" MaxLength="50"></asp:TextBox></td></tr>
<tr><td>City:</td><td><asp:TextBox runat="server" ID="tbCity" MaxLength="50" CssClass="validate[custom[City]]"></asp:TextBox></td></tr>
<tr><td>Country:</td><td><gen:DDLCountry runat="server" ID="ddlCountry" style="width:150px;"></gen:DDLCountry></td></tr>
<tr class="US"><td>State:</td><td><gen:DDLStateFullName runat="server" ID="ddlState" style="width:150px;"></gen:DDLStateFullName></td></tr>
<tr class="Int"><td>State/Province:</td><td><asp:TextBox runat="server" ID="tbStateEx" MaxLength="50"></asp:TextBox></td></tr>
<tr class="US"><td>Zip:</td><td><asp:TextBox runat="server" ID="tbZip" MaxLength="5" CssClass="validate[custom[Zip]] x-short"></asp:TextBox></td></tr>
<tr class="Int"><td>Postal Code:</td><td><asp:TextBox runat="server" ID="tbZipEx" CssClass="validate[required]" MaxLength="50"></asp:TextBox></td></tr>
<tr class="US"><td>Phone:</td><td>( <asp:TextBox runat="server" ID="tbPhone1" style="width: 33px;" MaxLength="3"></asp:TextBox> ) <asp:TextBox runat="server" ID="tbPhone2" style="width: 33px;" MaxLength="3"></asp:TextBox> - <asp:TextBox runat="server" ID="tbPhone3" style="width: 40px;" MaxLength="4"></asp:TextBox></td></tr>
<tr class="Int"><td>Phone:</td><td><asp:TextBox runat="server" ID="tbPhoneEx" MaxLength="50"></asp:TextBox></td></tr>
<tr><td>Email:</td><td><asp:TextBox runat="server" ID="tbEmail" MaxLength="50" CssClass="validate[custom[Email]]"></asp:TextBox></td></tr>
<script type="text/javascript">
    function Address(containerId) {
        this.container = $(containerId);
        this.getFirstName = function () {
            return this.container.find("#<%# tbFirstName.ClientID %>").val();
        }
        this.setFirstName = function (val) {
            this.container.find("#<%# tbFirstName.ClientID %>").val(val);
        }
        this.getLastName = function () {
            return this.container.find("#<%# tbLastName.ClientID %>").val();
        }
        this.setLastName = function (val) {
            this.container.find("#<%# tbLastName.ClientID %>").val(val);
        }
        this.getAddress1 = function () {
            return this.container.find("#<%# tbAddress1.ClientID %>").val();
        }
        this.setAddress1 = function (val) {
            this.container.find("#<%# tbAddress1.ClientID %>").val(val);
        }
        this.getAddress2 = function () {
            return this.container.find("#<%# tbAddress2.ClientID %>").val();
        }
        this.setAddress2 = function (val) {
            this.container.find("#<%# tbAddress2.ClientID %>").val(val);
        }
        this.getCity = function () {
            return this.container.find("#<%# tbCity.ClientID %>").val();
        }
        this.setCity = function (val) {
            this.container.find("#<%# tbCity.ClientID %>").val(val);
        }
        this.getCountry = function () {
            return this.container.find("#<%# ddlCountry.ClientID %>").val();
        }
        this.setCountry = function (val) {
            this.container.find("#<%# ddlCountry.ClientID %>").val(val);
        }
        this.getStateUS = function () {
            return this.container.find("#<%# ddlState.ClientID %>").val();
        }
        this.setStateUS = function (val) {
            this.container.find("#<%# ddlState.ClientID %>").val(val);
        }
        this.getStateInt = function () {
            return this.container.find("#<%# tbStateEx.ClientID %>").val();
        }
        this.setStateInt = function (val) {
            this.container.find("#<%# tbStateEx.ClientID %>").val(val);
        }
        this.getZipUS = function () {
            return this.container.find("#<%# tbZip.ClientID %>").val();
        }
        this.setZipUS = function (val) {
            this.container.find("#<%# tbZip.ClientID %>").val(val);
        }
        this.getZipInt = function () {
            return this.container.find("#<%# tbZipEx.ClientID %>").val();
        }
        this.setZipInt = function (val) {
            this.container.find("#<%# tbZipEx.ClientID %>").val(val);
        }
        this.getPhoneUS = function () {
            return this.container.find("#<%# tbPhone1.ClientID %>").val() +
                "-" + this.container.find("#<%# tbPhone2.ClientID %>").val() +
                "-" + this.container.find("#<%# tbPhone3.ClientID %>").val();
        }
        this.setPhoneUS = function (val) {
            var phoneArr = val.split('-');
            this.container.find("#<%# tbPhone1.ClientID %>").val(phoneArr[0]);
            this.container.find("#<%# tbPhone2.ClientID %>").val(phoneArr[1]);
            this.container.find("#<%# tbPhone3.ClientID %>").val(phoneArr[2]);
        }
        this.getPhoneInt = function () {
            return this.container.find("#<%# tbPhoneEx.ClientID %>").val();
        }
        this.setPhoneInt = function (val) {
            this.container.find("#<%# tbPhoneEx.ClientID %>").val(val);
        }
        this.getEmail = function () {
            return this.container.find("#<%# tbEmail.ClientID %>").val();
        }
        this.setEmail = function (val) {
            this.container.find("#<%# tbEmail.ClientID %>").val(val);
        }
        this.copyTo = function (address) {
            address.setFirstName(this.getFirstName());
            address.setLastName(this.getLastName());
            address.setAddress1(this.getAddress1());
            address.setAddress2(this.getAddress2());
            address.setCity(this.getCity());
            address.setCountry(this.getCountry());
            address.setStateUS(this.getStateUS());
            address.setStateInt(this.getStateInt());
            address.setZipUS(this.getZipUS());
            address.setZipInt(this.getZipInt());
            address.setPhoneUS(this.getPhoneUS());
            address.setPhoneInt(this.getPhoneInt());
            address.setEmail(this.getEmail());
        }
        this.onChangeCountry = function () {
            if (this.getCountry() == "US") {
                this.container.find(".Int").hide();
                this.container.find(".Int input").addClass("donotvalidate");
                this.container.find(".Int select").addClass("donotvalidate");
                this.container.find(".US").show();
                this.container.find(".US input").removeClass("donotvalidate");
                this.container.find(".US select").removeClass("donotvalidate");
            }
            else {
                this.container.find(".US").hide();
                this.container.find(".US input").addClass("donotvalidate");
                this.container.find(".US select").addClass("donotvalidate");
                this.container.find(".Int").show();
                this.container.find(".Int input").removeClass("donotvalidate");
                this.container.find(".Int select").removeClass("donotvalidate");
            }
        }
    }

    $(document).ready(function () {
        var address = new Address($("#<%# ddlCountry.ClientID %>").parent().parent().parent());
        address.onChangeCountry();
        $("#<%# ddlCountry.ClientID %>").change(function () {
            var address = new Address($("#<%# ddlCountry.ClientID %>").parent().parent().parent());
            address.onChangeCountry();
        });
    });
</script>