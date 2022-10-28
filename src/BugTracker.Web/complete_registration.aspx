<%@ Page language="C#" CodeBehind="complete_registration.aspx.cs" Inherits="BugTracker.Web.complete_registration" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->

<html>
<head>
<title id="titl" runat="server">btnet change password</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body>
<table border=0><tr>

<%

Response.Write (Application["custom_logo"]);

%>

</table>


<div align="center">
<table border=0><tr><td>

<div runat="server" class=err id="msg">&nbsp;</div>
<p>
<a href="default.aspx">Go to login page</a>

</td></tr></table>

</div>
</body>
</html>
