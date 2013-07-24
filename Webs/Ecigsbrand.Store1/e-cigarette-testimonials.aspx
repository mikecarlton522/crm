<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Front.Master" AutoEventWireup="true" CodeBehind="e-cigarette-testimonials.aspx.cs" Inherits="Ecigsbrand.Store1.e_cigarette_testimonials" %>
<%@ Register TagPrefix="uc" TagName="RefillCartridges" Src="~/Controls/RefillCartridges.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">E-Cigs Electronic Cigarette: E-Cigarette Testimonials</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphContent" runat="server">
<div id="content">
  <div class="left">
  	<div style="clear:both">
	    <h1>Rene R.</h1>
	    <h3>I can be more active using my E-Cig.</h3> 
	    <p>"I thought about quitting smoking everyday. Once I received my E-Cigs electronic cigarette and started using it, I had found it was going to be easier to quit smoking regular cigarettes. I found that I can be more active using my E-cig. Best of all is I can enjoy my nicotine without having to miss out on socializing with friends due to the smoking ban for establishments. Thank you so much E-cigs, you really changed my life!"</p>
    </div>
  	<div style="clear:both">
	    <h1>Eric B.</h1>
	    <h3>I instantly fell in love.</h3> 
	    <p>"From years of smoking cigarettes, my health began to take its toll on my life and personality. I never thought I could ever break the cycle of 11 years of smoking when I heard of the Brand E-Cigs. I got very excited and interested to try them. When I began using my E-Cig, I instantly fell in love. Almost a year later, I have finally quit smoking cigarettes. Something I thought I could never do. Giving the E-Cig a chance completely changed my life."</p>
    </div>
    <div style="clear:both">
    	<img src="images/testimonial-2.jpg" width="150" height="150" align="right" style="border:1px solid black; margin:30px 0 0 15px;">
	    <h1>Darren B.</h1>
	    <h3>It smokes like a dream.</h3> 
	    <p>"I received my E-Cigs e-cigarette kit just a few days after my order and I’m more than impressed by the solid feel. It certainly feels like some time went into it’s design. It’s almost like a high end fountain pen. Plus, it smokes like a dream,
	      I’m surprised by the billowy smoke you get, just great."</p>
    </div>
    <div style="clear:both">
    	<img src="images/testimonial-3.jpg" width="150" height="150" align="right" style="border:1px solid black; margin:30px 0 0 15px;">
	    <h1>Kevin S.</h1>
	    <h3>I can smoke anywhere I want without feeling any guilt.</h3>
	    <p>"These E-cigarettes are really amazing. I love being able to smoke anywhere I want, including my own home for once, without worrying about the smoke and smell. Finally, I can smoke anywhere I want without feeling any guilt."</p>
    </div>
    <div style="clear:both">
	    <img src="images/testimonial-1.jpg" width="150" height="150" align="right" style="border:1px solid black; margin:30px 0 0 15px;">
	    <h1>Jane F.</h1>
	    <h3>I love the feeling of of the E-Cigs electronic cigarette.</h3>
	    <p>"I really wanted to give up smoking, but found it hard to give up the nicotine rush throughout the day. This e-cigarette delivers satisfying flavor and feel without all the nasty stuff that comes with smoking. I don't even miss traditional cigarettes
	      and now I enjoy smoking anytime anywhere without bothering anyone around me."</p>
  	</div>
  </div>
  <div class="right">
    <uc:RefillCartridges ID="cRefillCartridges" runat="server" />
  </div>
  <div style="clear:both;"></div>
</div>
</asp:Content>
