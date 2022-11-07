<%@ Page language="C#" CodeBehind="change_password.aspx.cs" Inherits="BugTracker.Web.change_password" validateRequest="false" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->

<html>
<head>
	<title id="title" runat="server">btnet change password</title>
	<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body onload="document.forms[0].password.focus()">
<table border=0><tr>

<%

Response.Write (Application["custom_logo"]);

%>

</table>

<div align="center">
<table border=0><tr><td>
<form class=frm runat="server">
	<table border=0>

	<tr>
	<td class=lbl>New Password:</td>
	<td><input runat="server" type=password class=txt id="password" size=20 maxlength=20 autocomplete=off ></td>
	</tr>

	<tr>
	<td class=lbl>Reenter Password:</td>
	<td><input runat="server" type=password class=txt id="confirm" size=20 maxlength=20 autocomplete=off ></td>
	</tr>

	<tr><td colspan=2 align=left>
	<span runat="server" class=err id="msg">&nbsp;</span>
	</td></tr>

	<tr><td colspan=2 align=center>
	<input class=btn type=submit value="Change password" runat="server">
	</td></tr>

	</table>
</form>

<a href="default.aspx">Go to login page</a>

</td></tr></table>

</div>
</body>
</html>
