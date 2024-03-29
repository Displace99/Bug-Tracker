<%@ Page language="C#" CodeBehind="notifications.aspx.cs" Inherits="BugTracker.Web.notifications" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<html>
<head>
<title id="titl" runat="server">btnet queued notifications</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="sortable.js"></script>
</head>

<body>
<% security.write_menu(Response, "admin"); %>

<div style="width: 600px;"
class=smallnote>

Email notifications are put into a table into the database and then the system attempts to send them.
If the system fails to send the notification, it records the reason for the failure with the row.
<br><br>
The system makes 3 attempts to send the notification.  After the 3rd attempt,
you can either give up and delete the unsent notifications
or you can reset the retry count and let the system continue trying.

</div>

<p>
<div class=align>
<a href=edit_queued_notifications.aspx?actn=delete&ses=<% Response.Write(ses); %> >Delete unsent notifications</a>
<br>
<br>
<a href=edit_queued_notifications.aspx?actn=reset&ses=<% Response.Write(ses); %> >Reset retry count to zero</a>
<br>
<br>
<a href=edit_queued_notifications.aspx?actn=resend&ses=<% Response.Write(ses); %> >Try to resend</a>
<br>
<br>

<%

if (ds.Tables[0].Rows.Count > 0)
{
	SortableHtmlTable.create_from_dataset(
		Response, ds, "", "");

}
else
{
	Response.Write ("No queued email notifications in the database.");
}

%>
</div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>
