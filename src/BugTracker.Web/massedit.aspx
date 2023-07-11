<%@ Page language="C#" CodeBehind="massedit.aspx.cs" Inherits="BugTracker.Web.massedit" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->


<html>
<head>
<title id="titl" runat="server">btnet mass edit</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body>

<% security.write_menu(Response, "admin"); %>
<div class=align>

	<p>
	<div runat="server" id=msg class=err>&nbsp;</div>

	<p>
	<a href="search.aspx">back to search</a>

<p>or<p>
<script>
function submit_form()
{
	var frm = document.getElementById("frm");
	frm.submit();
	return true;
}

</script>
<form runat="server" id="frm">
<a style="border: 1px red solid; padding: 3px;" id="confirm_href" runat="server" href="javascript: submit_form()"></a>
<input type="hidden" id="bug_list" runat="server">
<input type="hidden" id="update_or_delete" runat="server">
</form>


	<p>&nbsp;<p>
	<p><div class=err>Email notifications are not sent when updates are made via this page.</div>
	<p>This SQL statement will execute when you confirm:
	<pre id="sql_text" runat="server"></pre>

</div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>
