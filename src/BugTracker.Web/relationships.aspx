<%@ Page language="C#" CodeBehind="relationships.aspx.cs" Inherits="BugTracker.Web.relationships" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->

<!DOCTYPE html>
<html lang="en">
<head>
<title id="titl" runat="server">btnet related bugs</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="sortable.js"></script>

<script>

var asp_form_id = '<% Response.Write(btnet.Util.get_form_name()); %>';

function remove(bugid2_arg)
{
    var frm =  document.getElementById(asp_form_id)
    var actn = document.getElementById("actn")
    actn.value = "remove"
    document.getElementById("bugid2").value = bugid2_arg
    frm.submit()
}

function body_on_load()
{

	opener.set_relationship_cnt(
	<%
		Response.Write(Convert.ToString(bugid));
		Response.Write(",");
		Response.Write(Convert.ToString(ds.Tables[0].Rows.Count));
	%>
	)
}

</script>
</head>

<body onload="body_on_load()">
	<% security.write_menu(Response, btnet.Util.get_setting("PluralBugLabel","bugs")); %>
<div class=align>
Relationships for 
<% 
	Response.Write(btnet.Util.get_setting("SingularBugLabel","bug") 
	+ " " 
	+ Convert.ToString(bugid)); 
%>
<br />
	<a href="edit_bug.aspx?id=<%=bugid %>">back to <%= btnet.Util.get_setting("SingularBugLabel","bug") %></a>
<p></p>
<table border=0><tr><td>

<%
if (permission_level != btnet.Security.PERMISSION_READONLY)
{
%>
<p>
<form class=frm runat="server" action="relationships.aspx">

<table>
	<tr>
		<td>Related ID:</td><td><input type="text" class="txt" id="bugid2" name="bugid2" size=8></td>
	</tr>
	<tr>
		<td>Comment:</td><td><input type="text" class="txt" id="type" name="type" size=90 maxlength=500></td>
	</tr>
	<tr>
		<td colspan=2>
			Related ID is sibling<asp:RadioButton runat="server" checked="true" GroupName="direction" value="0" id="siblings"/>
			&nbsp;&nbsp;&nbsp;
			Related ID is child<asp:RadioButton runat="server" GroupName="direction" value="1" id="child_to_parent"/>
			&nbsp;&nbsp;&nbsp;
			Related ID is parent<asp:RadioButton runat="server" GroupName="direction" value="2" id="parent_to_child"/>
			&nbsp;&nbsp;&nbsp;
		</td>
	</tr>
	<tr>
		<td colspan=2><input class="btn" type="submit" value="Add"></td>
	</tr>
	<tr>
		<td colspan=2>&nbsp;</td>
	</tr>
	<tr>
		<td colspan=2>&nbsp;<span runat="server" class="err" id="add_err"></span></td>
	</tr>
</table>

<input runat="server" id="bgid" type=hidden name="bgid" value="">
<input id="actn" type=hidden name="actn" value="add">
<input id="ses" type=hidden name="ses" value="<% Response.Write(ses); %>">

</form>
<% } %>

</td></tr></table>

</p>
<%

if (ds.Tables[0].Rows.Count > 0)
{
	btnet.SortableHtmlTable.create_from_dataset(
		Response, ds, "", "", false);

	display_hierarchy();
}
else
{
	Response.Write ("No related " + btnet.Util.get_setting("PluralBugLabel","bugs"));
}

%>
</div>
</body>
</html>
