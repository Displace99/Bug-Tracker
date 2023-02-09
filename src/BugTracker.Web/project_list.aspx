<%@ Page Title="" Language="C#" MasterPageFile="~/LoggedIn.Master" AutoEventWireup="true" CodeBehind="project_list.aspx.cs" Inherits="BugTracker.Web.project_list" %>
<%@ MasterType TypeName="BugTracker.Web.LoggedIn" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headerScripts" runat="server">
    <script type="text/javascript" language="JavaScript" src="sortable.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h2>Projects</h2>
    <div id=wait class=please_wait>&nbsp;</div>
    <br />
    <div class=click_to_sort>click on column headings to sort</div>
    <div id=myholder>
        <table id=mytable border=1 class=datat>
            <tr>
                <td class=datah valign=bottom>
                    <a href='javascript: sort_by_col(0, "str")'>Name</a>
                </td>
                <td class=datah valign=bottom>
                    <a href='javascript: sort_by_col(1, "str")'>Description</a>
                </td>
                <td class=datah valign=bottom>
                    <a href='javascript: sort_by_col(2, "str")'>Status</a>
                </td>
            </tr>
            <%foreach (var project in projectList) { %>
            <tr>
                <td class=datad><a href="project_detail.aspx?projectId=<%= project.Id %>"><%: project.Name %></a></td>
                <td class=datad><%= project.Description %></td>
                <td class=datad><%= project.Status %></td>
            </tr>
            <%} %>
        </table>
    </div>
    <br />
    <div id=sortedby>&nbsp;</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="footerScripts" runat="server">
</asp:Content>
