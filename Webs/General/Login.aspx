<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="General.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Secure Trial Offers Login</title>
    <style type="text/css">
        body {
	        font-family:"Lucida Sans Unicode", "Lucida Grande", sans-serif;
	        margin:0;
        }
        #wrapper {
          position:absolute;
	        top: 50%;
	        width:100%;
	        margin-top:-80px;
	        }
        #wrapper td {
	        font-size:10px;
	        }
        #inner {
	        margin:auto;
	        }
        input.button {
	        width:120px;
        }
        input.field {
	        width:200px;
	        padding:4px;
	        margin:0px;
	        background:url(images/topfade.gif) repeat-x top;
	        vertical-align:middle;
	        border:1px solid #999999;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div id="wrapper">
		        <div id="inner">
			        <table cellpadding="5" cellspacing="0" align="center">
				        <tr>
					        <td align="right">Username</td>
					        <td><asp:TextBox runat="server" ID="tbUsername" CssClass="field" size="25"></asp:TextBox></td>
				        </tr>
				        <tr>
					        <td align="right">Password</td>
					        <td><asp:TextBox runat="server" ID="tbPassword" CssClass="field" size="25" TextMode="Password"></asp:TextBox></td>
				        </tr>
				        <tr>
					        <td align="right" colspan="2" ><asp:Button runat="server" ID="bLogin" Text="Log In" 
                                    CssClass="button" onclick="bLogin_Click" /></td>
				        </tr>
				        <tr>
					        <td></td>
					        <td align="right">
					            <asp:CustomValidator runat="server" ID="vIncorrectLogin" Display="Dynamic">
					                <font color="red">Incorrect login.  Please try again.</font>
					            </asp:CustomValidator>						        
					        </td>
			        </table>

		        </div>
	        </div>
    </form>
</body>
</html>
