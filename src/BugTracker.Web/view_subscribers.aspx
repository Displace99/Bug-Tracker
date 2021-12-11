<%@ Page language="C#" CodeBehind="view_subscribers.aspx.cs" Inherits="BugTracker.Web.view_subscribers" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->


<html>
<head>
<title id="titl" runat="server">btnet view subscribers</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="sortable.js"></script>
</head>

<body width=600>
<div class=align>
	<% security.write_menu(Response, btnet.Util.get_setting("PluralBugLabel","bugs")); %>
Subscribers for <%= bugid %><br />
	<a href="edit_bug.aspx?id=<%=bugid %>">back to <%= btnet.Util.get_setting("SingularBugLabel","bug") %></a>
<p></p>

<table border=0><tr><td>
<form class=frm runat="server" action="view_subscribers.aspx">
<table>
<tr><td><span class=lbl>add subscriber:</span>

<asp:DropDownList id="userid" runat="server">
</asp:DropDownList>

<tr><td colspan=2><input class=btn type=submit value="Add">
<tr><td colspan=2>&nbsp;<span runat="server" class='err' id="add_err"></span>
</table>
<input type=hidden name="id" value=<% Response.Write(Convert.ToString(bugid));%>>
<input type=hidden name="actn" value="add">

</form>
</td></tr></table>

<%
if (ds.Tables[0].Rows.Count > 0)
{
	btnet.SortableHtmlTable.create_from_dataset(
		Response, ds, "", "", false);

}
else
{
	Response.Write ("No subscribers for this bug.");
}
%>
</div>
</body>
</html>
