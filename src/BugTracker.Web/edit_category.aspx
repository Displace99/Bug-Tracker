<%@ Page language="C#" CodeBehind="edit_category.aspx.cs" Inherits="BugTracker.Web.edit_category" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->

<!DOCTYPE html>

<html lang="en">
	<head>
		<title id="title" runat="server">btnet edit category</title>
		<link rel="StyleSheet" href="btnet.css" type="text/css">
	</head>
	<body>
		<% security.write_menu(Response, "admin"); %>

		<div class=align>
			<table border=0>
				<tr>
					<td>
						<a href=categories.aspx>back to categories</a>
						<form class=frm runat="server">
							<table border=0>
								<tr>
									<td class=lbl>Description:</td>
									<td><input runat="server" type=text class=txt id="name" maxlength=30 size=30></td>
									<td runat="server" class=err id="name_err">&nbsp;</td>
								</tr>

								<tr>
									<td colspan=3>
										<span class=smallnote>Sort Sequence controls the sort order in the dropdowns.</span>
									</td>
								</tr>

								<tr>
									<td class=lbl>Sort Sequence:</td>
									<td><input runat="server" type=text class=txt id="sort_seq" maxlength=2 size=2></td>
									<td runat="server" class=err id="sort_seq_err">&nbsp;</td>
								</tr>

								<tr>
									<td class=lbl>Default Selection:</td>
									<td><asp:checkbox runat="server" class=cb id="default_selection"/></td>
									<td>&nbsp</td>
								</tr>

								<tr>
									<td colspan=3 align=left>
										<span runat="server" class=err id="msg">&nbsp;</span>
									</td>
								</tr>

								<tr>
									<td colspan=2 align=center>
										<input runat="server" class=btn type=submit id="sub" value="Create or Edit">
									</td>
									<td>&nbsp</td>
								</tr>
							 </table>
						</form>
					</td>
				</tr>
		    </table>
		</div>
		<% Response.Write(Application["custom_footer"]); %>
	</body>
</html>


