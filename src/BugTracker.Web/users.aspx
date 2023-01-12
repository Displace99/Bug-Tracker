<%@ Page language="C#" CodeBehind="users.aspx.cs" Inherits="BugTracker.Web.users" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<html>
<head>
<title id="titl" runat="server">btnet users</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="sortable.js"></script>

<script>

function filter_changed()
{
	el = document.getElementById("filter_users")

	if (el.value != "")
	{
		el.style.background = "yellow"
	}
	else
	{
		el.style.background = "white"
	}

}

</script>


</head>

<body onload="filter_changed()">
<% security.write_menu(Response, "admin"); %>

<div class=align>

<table border=0 width=80%><tr>
	<td align=left valign=top>
		<a href=edit_user.aspx>add new user </a>
	<td align=right valign=top>
		<form runat="server">

			<span class=lbl>Show only usernames starting with:</span>
			<input type="text" runat="server" id="filter_users" class="txt" value="" onkeyup="filter_changed()" style="color: red;">

			&nbsp;&nbsp;&nbsp;

			<span class=lbl>hide inactive users:</span>
			<asp:CheckBox id="hide_inactive_users" class="cb" runat="server"/>

			<input type=submit class="btn" value="Refresh User List">
		</form>
</table>

<%

if (ds.Tables[0].Rows.Count > 0)
{

	SortableHtmlTable.create_from_dataset(
		Response, ds, "", "", false);

}
else
{
	Response.Write ("No users to display.");
}
%>
</div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>
