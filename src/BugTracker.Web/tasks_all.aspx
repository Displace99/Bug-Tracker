<%@ Page language="C#" CodeBehind="tasks_all.aspx.cs" Inherits="BugTracker.Web.tasks_all" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->

<html>
	<head>
		<title id="titl" runat="server">btnet all tasks</title>
		<link rel="StyleSheet" href="btnet.css" type="text/css">
		<script type="text/javascript" language="JavaScript" src="sortable.js"></script>
	</head>
	<body>
		<% security.write_menu(Response, btnet.Util.get_setting("PluralBugLabel","bugs")); %>
		<div class=align>

			All Tasks

			<p></p>


			<%
			if (ds_tasks.Tables[0].Rows.Count > 0)
			{
				btnet.SortableHtmlTable.create_from_dataset(Response, ds_tasks, "", "", false); 
			}
			else
			{
				Response.Write ("No tasks.");
			}

			%>
		</div>
		
		<% Response.Write(Application["custom_footer"]); %>

	</body>
</html>


