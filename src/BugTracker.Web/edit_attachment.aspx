<%@ Page language="C#" CodeBehind="edit_attachment.aspx.cs" Inherits="BugTracker.Web.edit_attachment" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->


<html>
<head>
<title id="titl" runat="server">btnet edit attachment</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body>
<% security.write_menu(Response, Util.get_setting("PluralBugLabel","bugs")); %>


<div class=align><table border=0><tr><td>
<a href=edit_bug.aspx?id=<% Response.Write(Convert.ToString(bugid));%>>back to <% Response.Write(Util.get_setting("SingularBugLabel","bug")); %></a>
<form class=frm runat="server">
	<table border=0>

	<tr>
	<td class=lbl>Description:</td>
	<td><input runat="server" type=text class=txt id="desc" maxlength=80 size=80></td>
	<td runat="server" class=err id="desc_err">&nbsp;</td>
	</tr>

	<tr>
	<td class=lbl>Filename:</td>
	<td><b><span id=filename runat="server">&nbsp;</span></b></td>
	<td>&nbsp;</td>
	</tr>


	<tr>
	<td colspan=3>
	<asp:checkbox runat="server" class=cb id="internal_only"/>
	<span runat="server" id="internal_only_label">Visible to internal users only</span>
	</td>
	</tr>


	<tr><td colspan=3 align=left>
	<span runat="server" class=err id="msg">&nbsp;</span>
	</td></tr>

	<tr>
	<td colspan=2 align=center>
	<input runat="server" class=btn type=submit id="sub" value="Update">
	<td>&nbsp</td>
	</td>
	</tr>
	</td></tr></table>
</form>
</td></tr></table></div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>


