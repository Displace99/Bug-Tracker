<%@ Page Title="" Language="C#" MasterPageFile="~/LoggedIn.Master" CodeBehind="edit_udf.aspx.cs" Inherits="BugTracker.Web.edit_udf" AutoEventWireup="True" %>
<%@ MasterType TypeName="BugTracker.Web.LoggedIn" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headerScripts" runat="server">
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
	<div class=align>
		<table border=0>
			<tr>
				<td>
					<a href=udfs.aspx>back to user defined attribute values</a>
					<form class=frm runat="server">
						<table border=0>

							<tr>
								<td class=lbl>Description:</td>
								<td><input runat="server" type=text class=txt id="name" maxlength=20 size=20></td>
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
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="footerScripts" runat="server">
</asp:Content>
