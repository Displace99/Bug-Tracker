 <%@ Page language="C#" CodeBehind="queries.aspx.cs" Inherits="BugTracker.Web.queries" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->

<html>
<head>
<title id="titl" runat="server">btnet queries</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="sortable.js"></script>
</head>

<body>
<% security.write_menu(Response, "queries"); %>

<div class=align>

<% if (security.user.is_admin || security.user.can_edit_sql) { %>
	<table border=0 width=80%><tr>
		<td align=left valign=top>
			<a href=edit_query.aspx>add new query</a>
		<td align=right valign=top>
			<form runat="server">
				<span class=lbl>show everybody's private queries:</span>
				<asp:CheckBox id="show_all" class="cb" runat="server" AutoPostback="True" />
			</form>
	</table>
<%

}
else
{
	Response.Write ("<p>");
}

%>


<%

if (ds.Tables[0].Rows.Count > 0)
{
	SortableHtmlTable.create_from_dataset(
		Response, ds, "", "", false);
}
else
{
	Response.Write ("No queries in the database.");
}

%>
</div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>
