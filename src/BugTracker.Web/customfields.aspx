<%@ Page language="C#" CodeBehind="customfields.aspx.cs" Inherits="BugTracker.Web.customfields" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<%@ Import Namespace="btnet" %>

<html>
<head>
<title id="titl" runat="server">btnet custom fields</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="sortable.js"></script>
</head>

<body>
<% security.write_menu(Response, "admin"); %>


<div class=align>
<a href=add_customfield.aspx>add new custom field</a>
<p></p>
<%

if (ds.Tables[0].Rows.Count > 0)
{
	SortableHtmlTable.create_from_dataset(
		Response, ds, "edit_customfield.aspx?id=", "delete_customfield.aspx?id=");
}
else
{
	Response.Write ("No custom fields.");
}

%>
</div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>
