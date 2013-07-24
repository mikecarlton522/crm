<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Front.Master" AutoEventWireup="true" CodeBehind="e-cigarette-how-it-works.aspx.cs" Inherits="Ecigsbrand.Store2.e_cigarette_how_it_works" %>
<%@ Register TagPrefix="uc" TagName="Accessories" Src="~/Controls/Accessories.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">E-Cigs Electronic Cigarette: How Do E-Cigarettes Work?</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<div id="content">
  <div class="left">
    <h1>How E-Cigs Electronic Cigarettes Work</h1>
    <h3>Exploded View of the E-Cigs Electronic Cigarette</h3>
    <p>Your electronic cigarette employs the latest smart technology to provide you consistent and long-lasting operation. When you inhale through the device, air flow is detected by the smart chip controller which activates a heating element that vaporizes
      the nicotine solution stored in the mouthpiece. On the opposite end of your electronic cigarette the LED will light red indicating proper atomization.</p>
    <p><img src="images/how-exploded-view.png" width="586" height="86"></p>
    <p>View our <a href="e-cigarette-customer-service.aspx#faq">Frequently Asked Questions</a> or <a href="e-cigarette-customer-service.aspx#guide">Download our Getting Started Guide (PDF)</a></p>
    <h1>Unboxing</h1>
    <h3>Your Starter Kit Contents</h3>
    <ul>
      <li>1 x Electronic Cigareette Lithium Battery (White End)</li>
      <li>2 x Nicotine Cartridges</li>
      <li>1 x USB Battery Charger</li>
    </ul>
    <img src="images/how-kit-1.jpg" width="250" height="200" style="border:1px solid black; margin:10px 10px 0 0;"><img src="images/how-kit-2.jpg" width="250" height="200" style="border:1px solid black; margin:10px 0 0 0;">
    <h1>First Use</h1>
    <h3>Charging</h3>
    <p>While our electronic cigarette ships with a partial precharge so you can use it straight out of the box, we suggest that you fully charge the battery for 2 hours before use for complete enjoyment. To charge the battery screw the battery end (white
      end) into the black usb charger. Insert the USB end into any computer that is on. Wait 2 hours.</p>
    <img src="images/how-kit-3.jpg" width="250" height="200" style="border:1px solid black; margin:10px 10px 0 0;"><img src="images/how-kit-4.jpg" width="250" height="200" style="border:1px solid black; margin:10px 0 0 0;">
    <h3>Assembly</h3>
    <ol>
      <li>Remove protective rubber caps from a nicotine cartridge (orange end).</li>
      <li>Screw the nicotine cartridge into the battery (white end). Do not overtighten, a snug fit is all you need.</li>
      <li>That's it! Inhale through nicotine cartridge like you would a traditional cigarette.</li>
      <li>When it's time to replace your cartridge just unscrew the existing cartridge and screw a fresh one on.</li>
    </ol>
    <img src="images/how-kit-5.jpg" width="250" height="200" style="border:1px solid black; margin:10px 10px 0 0;"><img src="images/how-kit-6.jpg" width="250" height="200" style="border:1px solid black; margin:10px 0 0 0;">
    <h3>General Instructions</h3>
    <ul>
      <li>When inhaling the red end of the cigarette will light telling your electronic cigarette is working.</li>
      <li>When you notice a decrease in the volume of vapor, it's time to charge the battery or to replace the nicotine cartridge.</li>
      <li>A complete charge takes approximately 2 hours.</li>
    </ul>
  </div>
  <div class="right">
    <uc:Accessories ID="cAccessories" runat="server" />
  </div>
  <div style="clear:both;"></div>
</div>
</asp:Content>
