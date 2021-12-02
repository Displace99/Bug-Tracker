<%@ Page language="C#" CodeBehind="tasks.aspx.cs" Inherits="BugTracker.Web.tasks" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->


<html>
<head>
<title id="titl" runat="server">btnet tasks</title>

<link rel="StyleSheet" href="btnet.css" type="text/css">

<script type="text/javascript" language="JavaScript" src="sortable.js"></script>


<script>

function body_on_load()
{
	parent.set_task_cnt(<% Response.Write(Convert.ToString(ds.Tables[0].Rows.Count)); %>)
}

</script>

</head>
<body onload="body_on_load()">
	<% security.write_menu(Response, btnet.Util.get_setting("PluralBugLabel","bugs")); %>

<div class=align>

Tasks for 
<% 
	Response.Write(btnet.Util.get_setting("SingularBugLabel","bug") 
	+ " " 
	+ Convert.ToString(bugid)); 
%>
	<br />
	<a href="edit_bug.aspx?id=<%=bugid %>">back to <%= btnet.Util.get_setting("SingularBugLabel","bug") %></a>
<p>

<% if (permission_level == btnet.Security.PERMISSION_ALL && (security.user.is_admin || security.user.can_edit_tasks)) { %>
<a href="edit_task.aspx?id=0&bugid=<% Response.Write(Convert.ToString(bugid)); %>">add new task</a>
&nbsp;&nbsp;&nbsp;&nbsp;
<a href="tasks_all.aspx">view all tasks</a>
&nbsp;&nbsp;&nbsp;&nbsp;
<a target=_blank href="tasks_all_excel.aspx">export all tasks to excel</a>
<p>

<% } %>

<%
if (ds.Tables[0].Rows.Count > 0)
{
	btnet.SortableHtmlTable.create_from_dataset(
		Response, ds, "", "", false); 
}
else
{
	Response.Write ("No tasks.");
}

%>
</div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>


