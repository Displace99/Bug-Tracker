<%@ Page Title="" Language="C#" MasterPageFile="~/LoggedIn.Master" CodeBehind="customfields.aspx.cs" Inherits="BugTracker.Web.customfields" AutoEventWireup="True" %>
<%@ MasterType TypeName="BugTracker.Web.LoggedIn" %>

<%@ Import Namespace="btnet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headerScripts" runat="server">
	<!--
	Copyright 2002-2011 Corey Trager
	Distributed under the terms of the GNU General Public License
	-->
    <script type="text/javascript" language="JavaScript" src="sortable.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
	<div class=align>
		<a href=add_customfield.aspx>add new custom field</a>
		<p></p>
		<%

		if (ds.Tables[0].Rows.Count > 0)
		{
			SortableHtmlTable.create_from_dataset(
				Response, ds, "edit_customfield.aspx?id=", "delete_customfield.aspx?id=");
		}
		else
		{
			Response.Write ("No custom fields.");
		}

		%>
	</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="footerScripts" runat="server">
</asp:Content>

