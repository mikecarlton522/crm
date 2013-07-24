<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Cart.aspx.cs" Inherits="BeautyTruth.Store1.Cart"
    MasterPageFile="~/Controls/Front.Master" %>

<%@ Register TagPrefix="uc" TagName="Footer" Src="~/Controls/Footer.ascx" %>
<%@ Register TagPrefix="uc" TagName="ShoppingCart" Src="~/Controls/ShoppingCart.ascx" %>
<%@ Register TagPrefix="uc" TagName="Checkout" Src="~/Controls/Checkout.ascx" %>
<%@ Register TagPrefix="uc" TagName="ReviewOrder" Src="~/Controls/ReviewOrder.ascx" %>
<%@ Register TagPrefix="uc" TagName="Confirmation" Src="~/Controls/Confirmation.ascx" %>
<asp:Content ID="Content4" ContentPlaceHolderID="cphScript" runat="server">

    <script type="text/javascript" language="javascript" src="js/jquery.validationEngine.validationRules.js"></script>

    <script type="text/javascript" language="javascript" src="js/jquery.validationEngine.js"></script>

    <script type="text/javascript" language="javascript" src="js/mod10.js"></script>

    <script type="text/javascript" language="javascript" src="js/ajax.login.js"></script>

    <script type="text/javascript">
        $(document).ready(function() {
            $("#ShoppingStartButtons").html($("#StartButtons").html());
            $("#StartButtons").hide();
        });
    </script>

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    Beauty &amp; Truth: Anti-Aging Wrinkle Creams &amp; Skin Care for Face and Eye</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <div class="container_12">
        <asp:Wizard ID="wSteps" runat="server" OnActiveStepChanged="StepChanged" OnFinishButtonClick="wSteps_FinishButtonClick">
            <StartNavigationTemplate>
                <div id="StartButtons">
                    <div class="container_12">
                        <div class="grid_12 right">
                            <div class="button inline">
                                <a href="category.aspx" class="buttoncolor1">« Keep Shopping</a>
                            </div>
                            <div class="button inline">
                                <asp:LinkButton ID="lbUpdate" Text="Update Quantities" runat="server" OnClick="ShoppingCart_ProductsChanged" />
                            </div>
                            <div class="button inline">
                                <asp:LinkButton ID="StartNextButton" class="buttoncolor2" runat="server" CommandName="MoveNext"
                                    OnClick="ShoppingCart_ProductsChanged" Text="Checkout" />
                            </div>
                        </div>
                    </div>
                </div>
            </StartNavigationTemplate>
            <StepNavigationTemplate>
                <div class="container_12">
                    <div class="grid_12 right">
                        <div class="button inline">
                            <asp:LinkButton ID="StepPreviousButton" runat="server" CausesValidation="False" CommandName="MovePrevious"
                                class="buttoncolor1" Text="« Back" />
                        </div>
                        <div class="button inline">
                            <asp:LinkButton ID="StepNextButton" runat="server" CommandName="MoveNext" class="buttoncolor2"
                                OnClick="wSteps_FinishButtonClick" Text="Complete Order" />
                        </div>
                    </div>
                </div>
            </StepNavigationTemplate>
            <FinishNavigationTemplate>
                <div class="container_12">
                    <div class="grid_12 right">
                        <div class="button inline">
                            <a href="category.aspx" class="buttoncolor1">Go Back To Shopping</a>
                        </div>
                    </div>
                </div>
            </FinishNavigationTemplate>
            <SideBarTemplate>
                <div class="container_12">
                    <div class="grid_12" id="progress">
                        <asp:DataList runat="server" ID="SideBarList" HorizontalAlign="Justify" RepeatDirection="Horizontal">
                            <SelectedItemStyle CssClass="current" />
                            <ItemTemplate>
                                <div>
                                    <asp:LinkButton runat="server" ID="SideBarButton" Text="Step 1: Shopping Cart" />
                                </div>
                            </ItemTemplate>
                        </asp:DataList>
                    </div>
                </div>
                <asp:PlaceHolder runat="server" ID="phError">
                    <div class="container_12">
                        <div class="grid_12" id="Div1">
                            <div id="error" class="validation-error error">
                                <asp:Literal runat="server" ID="lError" />
                            </div>
                        </div>
                    </div>
                </asp:PlaceHolder>
                </tr><tr>
            </SideBarTemplate>
            <WizardSteps>
                <asp:WizardStep Title="Step 1: Shopping Cart" runat="server">
                    <uc:ShoppingCart ID="ShoppingCart" runat="server" OnProductsChanged="ShoppingCart_ProductsChanged"
                        OnCouponAdded="ShoppingCart_CouponAdded" OnGiftCertificatePopulated="ShoppingCart_GiftCertificatePopulated"
                        OnGiftCertificateRemoved="ShoppingCart_GiftRemoved" OnCouponRemoved="ShoppingCart_CouponRemoved" />
                </asp:WizardStep>
                <asp:WizardStep Title="Step 2: Account Information" runat="server" OnActivate="Step2_Activate">
                    <uc:Checkout ID="Checkout" runat="server" />
                </asp:WizardStep>
                <asp:WizardStep Title="Step 3: Review Order" runat="server" OnActivate="Step3_Activate">
                    <uc:ReviewOrder ID="ReviewOrder" runat="server" />
                </asp:WizardStep>
                <asp:WizardStep Title="Step 4: Complete" runat="server" OnActivate="Step4_Activate">
                    <uc:Confirmation ID="Confirmation" runat="server" />
                </asp:WizardStep>
            </WizardSteps>
        </asp:Wizard>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
    <uc:Footer WithSignIn="false" runat="server" />
</asp:Content>
