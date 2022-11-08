<%@ Page language="C#" CodeBehind="projects.aspx.cs" Inherits="BugTracker.Web.projects" AutoEventWireup="True" %>
<%@ Import Namespace="btnet" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->

<!DOCTYPE html>

<html lang="en">
	<head>
		<title id="titl" runat="server">btnet projects</title>
		<link rel="StyleSheet" href="btnet.css" type="text/css">
		<script type="text/javascript" language="JavaScript" src="sortable.js"></script>
	</head>

	<body>
		<% security.write_menu(Response, "admin"); %>

		<div class=align>
			<a href=edit_project.aspx>add new project</a>
			<p></p>
			<%

			if (ds.Tables[0].Rows.Count > 0)
			{
				SortableHtmlTable.create_from_dataset(
					Response, ds, "", "", false);
			}
			else
			{
				Response.Write ("No projects in the database.");
			}
			%>
		</div>
		<% Response.Write(Application["custom_footer"]); %>

	</body>
</html>
