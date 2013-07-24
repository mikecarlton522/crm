<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Footer.ascx.cs" Inherits="BeautyTruth.Store1.Controls.Footer" %>
<asp:PlaceHolder ID="phSignIn" runat="server">
    <div id="newsletter">
        <div class="container_12">
            <div class="grid_12">
                <asp:TextBox ID="txtEmailAddress" rel="Enter your email for exclusive member-only deals..." Text=""
                    runat="server" size="25" />
                <span class="button">
                    <asp:LinkButton OnClick="lbAddEmail_Click" ID="lbddEmail" OnClientClick="if (ValidateEmail()) return true; else return false;"
                        runat="server">Sign Up</asp:LinkButton></span>
                <asp:Label ID="lblResult" runat="server" Style="font: normal 11px/16px arial;" />
            </div>

            <script type="text/javascript">

                $('#<%#txtEmailAddress.ClientID %>').focus(function() {
                    var email = $('#<%#txtEmailAddress.ClientID %>').val();
                    if (email == $('#<%#txtEmailAddress.ClientID %>').attr("rel"))
                        $('#<%#txtEmailAddress.ClientID %>').val("");
                });

                $('#<%#txtEmailAddress.ClientID %>').focusout(function() {
                    var email = $('#<%#txtEmailAddress.ClientID %>').val();
                    if (email == "")
                        $('#<%#txtEmailAddress.ClientID %>').val($('#<%#txtEmailAddress.ClientID %>').attr("rel"));
                });

                function ValidateEmail() {
                    var email = $("#<%#txtEmailAddress.ClientID %>").val();
                    if (email != 0) {
                        if (!isValidEmailAddress(email)) {
                            $("#<%#lblResult.ClientID %>").html("Invalid email address. Please enter a valid email address.");
                            return false;
                        }
                    } else {
                        $("#<%#lblResult.ClientID %>").html("Invalid email address. Please enter a valid email address.");
                        return false;
                    }
                    return true;
                }

                function isValidEmailAddress(emailAddress) {
                    var pattern = new RegExp(/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i);
                    return pattern.test(emailAddress);
                }
  
            </script>

        </div>
    </div>
</asp:PlaceHolder>
<div id="footer">
    <div class="container_12">
        <asp:PlaceHolder ID="phTextColumns" runat="server">
            <div class="grid_4">
                <h1>
                    The Beauty &amp; Truth Story</h1>
                <p>
                    Since the beginning, we’ve been dedicated to developing skincare products that reveal
                    your true beauty and pursuing the highest quality ingredients that honor your skin.
                    No false promises, just potent products to restore your gorgeous glow!
                </p>
            </div>
            <div class="grid_4">
                <h1>
                    Our Guarantee</h1>
                <p>
                    If you are not totally satisfied with your Beauty &amp; Truth purchase, simply return
                    the unused portion to the place of purchase within 30 days for a full and easy refund
                    (less shipping and handling charges).</p>
            </div>
            <div class="grid_4">
                <h1>
                    Customer Care</h1>
                <p>
                    Monday to Saturday, 8AM - 6PM EST<br />
                    1-800-967-9211</p>
                <%--<h1>
                    <a href="#">Shipping Information</a></h1>
                <h1>
                    <a href="#">Order Status</a></h1>
                <h1>
                    <a href="#">Gift Cards</a></h1>--%>
            </div>
        </asp:PlaceHolder>
        <div class="grid_12">

            <script type="text/javascript">
                $(document).ready(function() {
                    $("a#terms").fancybox();
                    $("a#pp").fancybox();
                });
            </script>

            <h2>
                Copyright &copy; 2011 Beauy &amp; Truth. All Rights Reserved. <a id="terms" href="tc.aspx?type=tc&id=1067">
                    Terms</a> <a id="pp" href="tc.aspx?type=pp&id=1067">Privacy Policy</a>
            </h2>
        </div>
    </div>
</div>
