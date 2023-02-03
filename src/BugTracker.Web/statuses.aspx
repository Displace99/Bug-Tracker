<%@ Page language="C#" CodeBehind="statuses.aspx.cs" Inherits="BugTracker.Web.statuses" AutoEventWireup="True" %>
<!--
Copyright 2002-2014 Corey Trager
Distributed under the terms of the GNU General Public License
-->

<%@ Import Namespace="btnet" %>
<%@ Import Namespace="System.Data" %>

<html>
<head>

<title id="titl" runat="server">btnet statuses</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="sortable.js"></script>
</head>

<body>
<% security.write_menu(Response, "admin"); %>

<div class=align>
<a href=edit_status.aspx>add new status</a>
</p>
<%


if (ds.Tables[0].Rows.Count > 0)
{
	SortableHtmlTable.create_from_dataset(
		Response, ds, "edit_status.aspx?id=", "delete_status.aspx?id=");

}
else
{
	Response.Write ("No statuses in the database.");
}
%>
</div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>
