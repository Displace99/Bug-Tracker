<%@ Page language="C#" CodeBehind="admin.aspx.cs" Inherits="BugTracker.Web.admin" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->

<!DOCTYPE html>
<html lang="en">
	<head>
		<title id="title" runat="server">btnet admin</title>
		<link rel="StyleSheet" href="btnet.css" type="text/css">
	</head>
	<body>
		
		<% security.write_menu(Response, "admin"); %>

		<div class=align>
			<table border=0>
				<tr>
					<td>
						<ul>
							<p>
								<li class=listitem><a href=users.aspx>Users</a>
							<p>
								<li class=listitem><a href=orgs.aspx>Organizations</a>
							<p>
								<li class=listitem><a href=projects.aspx>Projects</a>
							<p>
								<li class=listitem><a href=categories.aspx>Categories</a>
							<p>
								<li class=listitem><a href=priorities.aspx>Priorities</a>
							<p>
								<li class=listitem><a href=statuses.aspx>Statuses</a>
							<p>
								<li class=listitem><a href=udfs.aspx>User Defined Attribute</a>
								&nbsp;&nbsp;<span class=smallnote>(see "ShowUserDefinedBugAttribute" and "UserDefinedBugAttributeName" in Web.config)</span>
							<p>
								<li class=listitem><a href=customfields.aspx>Custom Fields</a>
								&nbsp;&nbsp;<span class=smallnote>(add custom fields to the bug page)</span>
							<p>
								<li class=listitem><a target=_blank href=query.aspx>Run Ad-hoc Query</a>
								&nbsp;&nbsp;
								<span style="border: solid red 1px; padding: 2px; margin: 3px; color: red; font-size: 9px;">
								This links to query.aspx.&nbsp;&nbsp;Query.aspx is potentially unsafe.&nbsp;&nbsp;Delete it if you are deploying on a public web server.
								</span>
								<br>
							<p>
								<li class=listitem><a href=notifications.aspx>Queued Email Notifications</a>
							<p>
								<li class=listitem><a href=edit_custom_html.aspx>Edit Custom Html</a>
							<p>
								<li class=listitem><a href=edit_web_config.aspx>Edit Web.Config</a>
								&nbsp;&nbsp;
								<span style="border: solid red 1px; padding: 2px; margin: 3px; color: red; font-size: 9px;">
								Many BugTracker.NET features are configurable by editing Web.config, but please be careful!  Web.config is easy to break!
								</span>
								<br>
							<p>
								<li class=listitem><a href=backup_db.aspx>Backup Database</a>
							<p>
								<li class=listitem><a href=manage_logs.aspx>Manage Logs</a>
						</ul>
					</td>
				</tr>
			</table>
		</div>
		
		<% Response.Write(Application["custom_footer"]); %>

	</body>
</html>
