<%@ Page Title="" Language="C#" MasterPageFile="~/LoggedIn.Master" CodeBehind="delete_customfield.aspx.cs" Inherits="BugTracker.Web.delete_customfield" AutoEventWireup="True" %>
<%@ MasterType TypeName="BugTracker.Web.LoggedIn" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headerScripts" runat="server">
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
     <p></p>
    <div class=align>
        <p>&nbsp</p>
        <a href=customfields.aspx>back to custom fields</a>

        <p>or</p>

        <form runat="server" id="frm">
        <a id="confirm_href" runat="server" href="javascript: submit_form()"></a>
        <input type="hidden" id="row_id" runat="server">
        </form>
</div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="footerScripts" runat="server">
    <script>
        function submit_form() {
            var frm = document.getElementById("<%=frm.ClientID%>");
            frm.submit();
            return true;
        }
    </script>
</asp:Content>
