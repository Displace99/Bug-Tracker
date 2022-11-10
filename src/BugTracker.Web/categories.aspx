<%@ Page language="C#" CodeBehind="categories.aspx.cs" Inherits="BugTracker.Web.categories" AutoEventWireup="True" %>
<%@ Import Namespace="btnet" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->

<!DOCTYPE html>

<html lang="en">
	<head>
		<title id="title" runat="server">btnet categories</title>
		<link rel="StyleSheet" href="btnet.css" type="text/css">
		<script type="text/javascript" language="JavaScript" src="sortable.js"></script>
	</head>

	<body>
		<% security.write_menu(Response, "admin"); %>

		<div class=align>
			<a href=edit_category.aspx>add new category</a>
			<p></p>

			<%

			if (ds.Tables[0].Rows.Count > 0)
			{
				SortableHtmlTable.create_from_dataset(
					Response, ds, "edit_category.aspx?id=", "delete_category.aspx?id=");

			}
			else
			{
				Response.Write ("No categories in the database.");
			}

			%>
		</div>
		<% Response.Write(Application["custom_footer"]); %>
	</body>
</html>
