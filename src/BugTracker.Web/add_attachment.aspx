<%@ Page language="C#" CodeBehind="add_attachment.aspx.cs" Inherits="BugTracker.Web.add_attachment" AutoEventWireup="True" %>
<%@ Import Namespace="System.IO" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->


<html>
<head>
<title id="titl" runat="server">btnet add attachment</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">

<script>

function set_msg(s)
{
	document.getElementById("msg").innerHTML = s
	document.getElementById("file_input").innerHTML 
		='<input type=file class=txt name="attached_file" id="attached_file" maxlength=255 size=60>'
}

function waiting()
{
	document.getElementById("msg").innerHTML = "Uploading..."
	return true
}

</script>
</head>

<body>
<% security.write_menu(Response, btnet.Util.get_setting("PluralBugLabel","bugs")); %>


<div class=align>

	Add attachment to <%= bugid %> <br />
	<a href="edit_bug.aspx?id=<%=bugid %>">back to <%= btnet.Util.get_setting("SingularBugLabel","bug") %></a>
<p>
	<table border=0><tr><td>
		<form class=frm runat="server" enctype="multipart/form-data" onsubmit="return waiting()">
			<table border=0>

			<tr>
			<td class=lbl>Description:</td>
			<td><input runat="server" type=text class=txt id="desc" maxlength=80 size=80></td>
			<td runat="server" class=err id="desc_err">&nbsp;</td>
			</tr>

			<tr>
			<td class=lbl>File:</td>
			<td><div id="file_input">
			<input runat="server" type=file class=txt id="attached_file" maxlength=255 size=60>
			</div>
			</td>
			<td runat="server" class=err id="attached_file_err">&nbsp;</td>
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
			<input runat="server" class=btn type=submit id="sub" value="Upload">
			</td>
			</tr>
			</table>
		</form>
	</td></tr></table>
</div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>
