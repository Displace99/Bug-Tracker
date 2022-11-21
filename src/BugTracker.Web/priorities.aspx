<%@ Page language="C#" CodeBehind="priorities.aspx.cs" Inherits="BugTracker.Web.priorities" AutoEventWireup="True" %>
<%@ Import Namespace="btnet" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->

<html>
	<head>
		<title id="title" runat="server">btnet priorities</title>
		<link rel="StyleSheet" href="btnet.css" type="text/css">
		<script type="text/javascript" language="JavaScript" src="sortable.js"></script>
	</head>

	<body>
		<% security.write_menu(Response, "admin"); %>

		<div class=align>
			<a href=edit_priority.aspx>add new priority</a>
			<p></p>
			<%


			if (ds.Tables[0].Rows.Count > 0)
			{
				SortableHtmlTable.create_from_dataset(
					Response, ds, "edit_priority.aspx?id=", "delete_priority.aspx?id=", false);
			}
			else
			{
				Response.Write ("No priorities in the database.");
			}
			%>
		</div>
		<% Response.Write(Application["custom_footer"]); %>

	</body>
</html>
