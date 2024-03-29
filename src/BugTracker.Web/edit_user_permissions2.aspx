<%@ Page language="C#" CodeBehind="edit_user_permissions2.aspx.cs" Inherits="BugTracker.Web.edit_user_permissions2" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<!DOCTYPE html>
<html lang="en">
<head>
<title id="titl" runat="server">btnet edit user</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body>
<% security.write_menu(Response, "admin"); %>

<div class=align><table border=0><tr><td>
<a id="back_href" runat="server" href="">back</a>

<p>

<form class=frm runat="server">
	<table border=0 cellpadding=3>

	<tr>
	<td colspan=2 class=lbl><span id="this_page_title" runat="server" class=smallnote></span></td>
	</tr>

	<tr>
	<td colspan=2>
	<ASP:DataGrid id="MyDataGrid" runat="server" BorderColor="black" CssClass="datat" CellPadding="3" AutoGenerateColumns="false">
		<HeaderStyle cssclass="datah"></HeaderStyle>
		<ItemStyle cssclass="datad"></ItemStyle>
		<Columns>

		<asp:BoundColumn HeaderText="User" DataField="us_username"/>

		<asp:BoundColumn HeaderText="User" DataField="us_id" Visible="False"/>

		<asp:TemplateColumn HeaderText="Permissions">
			<ItemTemplate>
				<asp:RadioButton GroupName="permissions" text="none" value=0 ID="none" runat="server"
				Checked=<%# ((int)(( DataRowView ) Container.DataItem ) [ "pu_permission_level" ] == 0 ) %>/>

				<asp:RadioButton GroupName="permissions" text="view only" value=1 ID="readonly" runat="server"
				Checked=<%# ((int)(( DataRowView ) Container.DataItem ) [ "pu_permission_level" ] == 1 ) %>/>

				<asp:RadioButton GroupName="permissions" text="report (add and comment only)" value=3 ID="reporter" runat="server"
				Checked=<%# ((int)(( DataRowView ) Container.DataItem ) [ "pu_permission_level" ] == 3 ) %>/>

				<asp:RadioButton GroupName="permissions" text="all (add and edit)" value=2 ID="edit" runat="server"
				Checked=<%# ((int)(( DataRowView ) Container.DataItem ) [ "pu_permission_level" ] == 2 ) %>/>

			</ItemTemplate>
		</asp:TemplateColumn>

		</Columns>
	</ASP:DataGrid>



	<tr>
	<td colspan=2 align=left>
	<span runat="server" class=err id="msg">&nbsp;</span>


	<tr>
	<td colspan=2 align=center>
	<input runat="server" class=btn type=submit id="sub" value="Update">
	<td>&nbsp</td>
	</td>
	</tr>

	</table>
</form>
</td></tr></table></div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>


