<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductReviews.ascx.cs"
    Inherits="BeautyTruth.Store1.Controls.ProductReviews" %>
<%@ Register TagPrefix="uc" TagName="Rating" Src="~/Controls/RatingControl.ascx" %>
<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha" %>
<div class="grid_9 alpha">
    <asp:HiddenField ID="hdnWebStoreProductID" Value="<%# WebStoreProductID %>" runat="server" />
    <div id="tabsreview">
        <ul>
            <% if (ProductReviewList.Count > 0)
               { %>
            <li><a href="#reviews">Customer Testimonials</a></li>
            <% } %>
            <li><a href="#reviewssubmit">Submit Your Own Testimonial</a></li>
        </ul>
        <% if (ProductReviewList.Count > 0)
           { %>
        <div id="reviews">
            <h1>
                <%# ProductReviewList.Count %>
                Customer Testimonials</h1>
            <div class="sort">
                <div>
                    <h1>
                        Sort By:</h1>
                    <asp:DropDownList ID="dpdSort" OnSelectedIndexChanged="dpdSort_SelectedIndexChanged"
                        runat="server" AutoPostBack="true">
                        <asp:ListItem Value="0" Text="Choose Sort" />
                        <%--<asp:ListItem Value="1" Text="Most Useful First" />--%>
                        <asp:ListItem Value="2" Text="Rating - Highest to Lowest" />
                        <asp:ListItem Value="3" Text="Rating - Lowest to Highest" />
                        <asp:ListItem Value="4" Text="Date - New to Old" />
                        <asp:ListItem Value="5" Text="Date - Old to New" />
                        <asp:ListItem Value="6" Text="Length - Longest to Shortest" />
                        <asp:ListItem Value="7" Text="Length - Shortest to Longest" />
                    </asp:DropDownList>
                </div>
            </div>
            <asp:Repeater ID="rReviews" runat="server" DataSource="<% #ProductReviewList %>">
                <ItemTemplate>
                    <div class="review">
                        <div class="grid_2 alpha reviewer">
                            <h1>
                                <%#Eval("DisplayName") %></h1>
                            <h2>
                                Lives in</h2>
                            <p>
                                <%#Eval("Address") %></p>
                            <h2>
                                Skin Type</h2>
                            <p>
                                <%#Eval("SkinType") %></p>
                            <h2>
                                Skin Tone</h2>
                            <p>
                                <%#Eval("SkinTone") %></p>
                            <h2>
                                Sex and Age</h2>
                            <p>
                                <%#Eval("Sex") %>&nbsp;<%#Eval("AgeRange")%></p>
                        </div>
                        <div class="grid_6 text">
                            <h1>
                                <%#Eval("Title") %></h1>
                            <p>
                                <uc:Rating Rating='<%#Eval("Rating") %>' runat="server" />
                                <%--                                <img src="images/star.png" width="16"
                                    height="16" /><img src="images/star.png" width="16" height="16" /><img src="images/star.png"
                                        width="16" height="16" /><img src="images/star.png" width="16" height="16" />--%>
                            </p>
                            <h2>
                                <%#Eval("Summary") %></h2>
                            <p>
                                <%#Eval("Review") %></p>
                            <p>
                                Submitted
                                <%# Convert.ToDateTime(Eval("CreateDT")).ToString("MMMM d, yyyy") %></p>
                            <%--<h3>
                                <strong>3</strong> of <strong>4</strong> People found this review useful.</h3>
                            <h3>
                                Was this review useful to you?&nbsp;&nbsp;<a href="#">Yes</a>&nbsp;&nbsp;<a href="#">No</a></h3>--%>
                        </div>
                        <div class="clear">
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    <%-- <div class="index">
                        <ul>
                            <li>1-2 of 12 Reviews(s)</li>
                            <li class="current">1</li>
                            <li>|</li>
                            <li><a href="#">2</a></li>
                            <li>|</li>
                            <li><a href="#">3</a></li>
                            <li>|</li>
                            <li><a href="#">4</a></li>
                            <li>|</li>
                            <li>...</li>
                            <li class="underline"><a href="#">Next</a></li>
                            <li>|</li>
                            <li><a href="#">&raquo;</a></li>
                            <li>|</li>
                            <li class="underline"><a href="#">View All</a></li>
                        </ul>
                    </div>--%>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <% } %>
        <div id="reviewssubmit">
            <asp:PlaceHolder ID="phReviewSubmited" runat="server" Visible="false">
                <h1>
                    Your review was submitted.</h1>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phSubmitReview" runat="server">
                <h1>
                    Submit Your Own Review</h1>
                <h2>
                    Display Name *</h2>
                <p>
                    <asp:TextBox ID="txtDisplayName" runat="server" MaxLength="50" />
                    <asp:RequiredFieldValidator EnableClientScript="true" ErrorMessage="Required field"
                        ControlToValidate="txtDisplayName" runat="server" />
                </p>
                <h2>
                    Email Address *</h2>
                <p>
                    <asp:TextBox ID="txtEmail" runat="server" MaxLength="255" />
                    <asp:RequiredFieldValidator EnableClientScript="true" ErrorMessage="Required field"
                        ControlToValidate="txtEmail" runat="server" />
                </p>
                <p>
                    Your email will not be shown online in your review. A working email address is required.
                    We respect your privacy and will not share your email address. See our <a href="#">Privacy
                        Policy.</a></p>
                <h2>
                    Where Do You Live?</h2>
                <p>
                    <asp:TextBox ID="txtAddress" runat="server" MaxLength="24" />
                </p>
                <h2>
                    Skin Type</h2>
                <p>
                    <label for="select">
                    </label>
                    <select name="ddlSkinType" id="ddlSkinType" runat="server">
                        <option value="">Choose Skin Type</option>
                        <option value="Normal">Normal</option>
                        <option value="Combination">Combination</option>
                        <option value="Oily">Oily</option>
                        <option value="Dry">Dry</option>
                    </select>
                </p>
                <h2>
                    Skin Tone</h2>
                <p>
                    <select name="ddlSkinTone" id="ddlSkinTone" runat="server">
                        <option value="">Choose Skin Tone</option>
                        <option value="Fair">Fair</option>
                        <option value="Medium">Medium</option>
                        <option value="Dark">Dark</option>
                    </select>
                </p>
                <h2>
                    Sex</h2>
                <p>
                    <select name="ddlSex" id="ddlSex" runat="server">
                        <option value="">Choose Sex</option>
                        <option value="Female">Female</option>
                        <option value="Male">Male</option>
                    </select>
                </p>
                <h2>
                    Age Range</h2>
                <p>
                    <select name="ddlAge" id="ddlAge" runat="server">
                        <option value="">Choose Age</option>
                        <option value="21 and Under">21 and Under</option>
                        <option value="19-24">19-24</option>
                        <option value="35-44">35-44</option>
                        <option value="45-54">45-54</option>
                        <option value="55-64">55-64</option>
                        <option value="65 and Over">65 and Over</option>
                    </select>
                </p>
                <h2>
                    Product Rating *</h2>
                <p>
                    <select name="ddlRating" id="ddlRating" runat="server">
                        <option value="">Choose Rating</option>
                        <option value="5">5 Stars</option>
                        <option value="4">4 Stars</option>
                        <option value="3">3 Stars</option>
                        <option value="2">2 Stars</option>
                        <option value="1">1 Stars</option>
                    </select>
                    <asp:RequiredFieldValidator EnableClientScript="true" ErrorMessage="Required field"
                        ControlToValidate="ddlRating" runat="server" />
                </p>
                <h2>
                    Title of Review *</h2>
                <p>
                    <asp:TextBox ID="txtTitle" runat="server" MaxLength="100" Width="250" />
                    <asp:RequiredFieldValidator EnableClientScript="true" ErrorMessage="Required field"
                        ControlToValidate="txtTitle" runat="server" />
                </p>
                <h2>
                    One Sentence Summary</h2>
                <p>
                    <asp:TextBox ID="txtSummary" runat="server" MaxLength="400" Width="400" />
                </p>
                <h2>
                    Review *</h2>
                <p>
                    <asp:TextBox ID="txtReview" TextMode="MultiLine" runat="server" Columns="80" Rows="10" />
                    <asp:RequiredFieldValidator EnableClientScript="true" ErrorMessage="Required field"
                        ControlToValidate="txtReview" runat="server" />
                </p>
<%--                <p>
                    Please note, all submissions are first reviewed by our editors for profanity or
                    foul language.</p>--%>
                <p>
                    <recaptcha:RecaptchaControl ID="recaptcha" runat="server" PublicKey="6LcT7coSAAAAAJoR5Fmh4CXXEAVe7pM6_qr7OboW"
                        PrivateKey="6LcT7coSAAAAAAEYsoL3HBu8KlYBEghHQvpkQDGx" Language="en" />
                </p>
                <asp:PlaceHolder ID="phCaptchaError" runat="server" Visible="false">
                    <p>
                        <span style="color: Red;">Code you have entered is invalid. Please try again.</span>
                    </p>
                </asp:PlaceHolder>
                <p>
                    <asp:Button ID="sbmt" OnClick="sbmt_Click" runat="server" Text="Submit" />
                </p>
            </asp:PlaceHolder>
        </div>
    </div>
</div>
