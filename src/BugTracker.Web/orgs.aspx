<%@ Page language="C#" CodeBehind="orgs.aspx.cs" Inherits="BugTracker.Web.orgs" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<%@ Import Namespace="btnet" %>

<!DOCTYPE html>

<html lang="en">
	<head>
		<title id="title" runat="server">btnet orgs</title>
		<link rel="StyleSheet" href="btnet.css" type="text/css">
		<script type="text/javascript" language="JavaScript" src="sortable.js"></script>
	</head>

	<body>
		<% security.write_menu(Response, "admin"); %>

		<div class=align>
			<a href=edit_org.aspx>add new org</a>
			<p></p>
			<%

				if (ds.Tables[0].Rows.Count > 0)
				{
					SortableHtmlTable.create_from_dataset(
						Response, ds, "", "", false);

				}
				else
				{
					Response.Write ("No orgs in the database.");
				}

			%>
		</div>
		<% Response.Write(Application["custom_footer"]); %>

	</body>
</html>
