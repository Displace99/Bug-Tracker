<%@ Page language="C#" CodeBehind="dashboard.aspx.cs" Inherits="BugTracker.Web.dashboard" AutoEventWireup="True" %>


<html>
	<head>
		
		<title id="titl" runat="server">btnet dashboard</title>
		<link rel="StyleSheet" href="btnet.css" type="text/css">
		<style>

		body {background: #ffffff;}

		.panel {
			background: #ffffff;
			border: 3px solid #cccccc;
			padding: 10px;
			margin-bottom: 10px;
		}

		iframe {
			border: 1px solid white;
			width: 90%;
			height:300px;
		}

		</style>
	</head>
<body>
<% security.write_menu(Response, "reports"); %>

<% if (security.user.is_guest) /* no dashboard */{ %>
<span class="disabled_link">edit dashboard not available to "guest" user</span>
<% } else { %>
<a href=edit_dashboard.aspx>edit dashboard</a>
<% } %>


<table border=0 cellspacing=0 cellpadding=10>
<tr>

<td valign=top>&nbsp;<br>

<% write_column(1); %>

<td valign=top>&nbsp;<br>

<% write_column(2); %>



</table>
<% Response.Write(Application["custom_footer"]); %>
</body>
</html>
