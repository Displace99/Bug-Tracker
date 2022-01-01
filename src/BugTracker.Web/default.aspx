<%@ Page language="C#" CodeBehind="default.aspx.cs" Inherits="BugTracker.Web.default" validateRequest="false" AutoEventWireup="True" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="btnet" %>

<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->


<html>
<head>
<title id="titl" runat="server">btnet logon</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<link rel="shortcut icon" href="favicon.ico">
</head>
<body onload="document.forms[0].user.focus()">
<div style="float: right;">
<span>
	<a target=_blank style=" font-size: 7pt; font-family: arial; letter-spacing: 1px;" href="https://bug-tracker.readthedocs.io/en/latest/">Help</a>
	<br>
	<a target=_blank style=" font-size: 7pt; font-family: arial; letter-spacing: 1px;" href="https://github.com/Displace99/Bug-Tracker/issues">Feedback</a>
	<br>
	<a target=_blank style=" font-size: 7pt; font-family: arial; letter-spacing: 1px;" href="about.html">About</a>
</span>
</div>
<table border=0><tr>

<%

Response.Write (Application["custom_logo"]);

%>

</table>


<div align="center">
<table border=0><tr><td>
<form class=frm runat="server">
	<table border=0>

	<% if (Util.get_setting("WindowsAuthentication", "0") != "0") { %>
		<tr><td colspan="2" class="smallnote">To login using your Windows ID, leave User blank</td></tr>
	<% } %>
	<tr>
	<td class=lbl>User:</td>
	<td><input runat="server" type=text class=txt id="user"></td>
	</tr>

	<tr>
	<td class=lbl>Password:</td>
	<td><input runat="server" type=password class=txt id="pw"></td>
	</tr>

	<tr><td colspan=2 align=left>
	<span runat="server" class=err id="msg">&nbsp;</span>
	</td></tr>

	<tr><td colspan=2 align=center>
	<input class=btn type=submit value="Logon" runat="server">
	</td></tr>

	</table>
</form>

<span>

<% if (Util.get_setting("AllowGuestWithoutLogin","0") == "1") { %>
<p>
<a style="font-size: 8pt;"href="bugs.aspx">Continue as "guest" without logging in</a>
<p>
<% } %>

<% if (Util.get_setting("AllowSelfRegistration","0") == "1") { %>
<p>
<a style="font-size: 8pt;"href="register.aspx">Register</a>
<p>
<% } %>

<% if (Util.get_setting("ShowForgotPasswordLink","1") == "1") { %>
<p>
<a style="font-size: 8pt;"href="forgot.aspx">Forgot your username or password?</a>
<p>
<% } %>


</span>

</td></tr></table>

<% Response.Write (Application["custom_welcome"]); %>
</div>
</body>
</html>
