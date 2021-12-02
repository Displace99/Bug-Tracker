<%@ Page language="C#" CodeBehind="delete_task.aspx.cs" Inherits="BugTracker.Web.delete_task" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->


<html>
	<head>
	<title id="titl" runat="server">btnet delete task</title>
	<link rel="StyleSheet" href="btnet.css" type="text/css">
	</head>
	<body>
		<% security.write_menu(Response, btnet.Util.get_setting("PluralBugLabel","bugs")); %>
		<p></p>
		<div class=align>
			<p>&nbsp</p>

			<a id="back_href" runat="server" href="">back to tasks</a>

			<p>or</p><p></p>

			<script>
			function submit_form()
			{
				var frm = document.getElementById("frm");
				frm.submit();
				return true;
			}

			</script>
			<form runat="server" id="frm">
				<a id="confirm_href" runat="server" href="javascript: submit_form()"></a>
			</form>

		</div>
	</body>
</html>

