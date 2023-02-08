<%@ Page language="C#" CodeBehind="udfs.aspx.cs" Inherits="BugTracker.Web.udfs" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<%@ Import Namespace="btnet" %>

<html>
<head>
<title id="title" runat="server">btnet user defined attributes</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="sortable.js"></script>
</head>

<body>
	<% security.write_menu(Response, "admin"); %>

	<div class=align>
		<a href=edit_udf.aspx>add new user defined attribute value</a>
		<p></p>
		<%


		if (ds.Tables[0].Rows.Count > 0)
		{
			SortableHtmlTable.create_from_dataset(
				Response, ds, "edit_udf.aspx?id=", "delete_udf.aspx?id=");

		}
		else
		{
			Response.Write ("No user defined attributes in the database.");
		}
		%>
	</div>
	<% Response.Write(Application["custom_footer"]); %>

</body>
</html>
