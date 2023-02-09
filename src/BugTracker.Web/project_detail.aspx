<%@ Page Title="" Language="C#" MasterPageFile="~/LoggedIn.Master" AutoEventWireup="true" CodeBehind="project_detail.aspx.cs" Inherits="BugTracker.Web.project_detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headerScripts" runat="server">
    <script type="text/javascript" language="JavaScript" src="sortable.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h2>Project <asp:Label ID="lblBugLabel" runat="server" Text="Bugs"></asp:Label></h2>

    <form method="POST" runat="server">
    <div id=wait class=please_wait>&nbsp;</div>
    <br />
    <div class=click_to_sort>click on column headings to sort</div>
    <div id=myholder>
        

        <table id=mytable border=1 class=datat>
            <tr>
                <td class=datah valign=bottom>
                    <a href='javascript: sort_by_col(0, "str")'>ID</a>
                </td>
                <td class=datah valign=bottom>
                    <a href='javascript: sort_by_col(1, "str")'>Description</a>
                </td>
                <td class=datah valign=bottom>
                    <a href='javascript: sort_by_col(2, "str")'>Organization</a>
                </td>
                <td class=datah valign=bottom>
                    <a href='javascript: sort_by_col(3, "str")'>Category</a>
                </td>
                <td class=datah valign=bottom>
                    <a href='javascript: sort_by_col(4, "str")'>Reported By</a>
                </td>
                <td class=datah valign=bottom>
                    <a href='javascript: sort_by_col(5, "str")'>Reported On</a>
                </td>
                <td class=datah valign=bottom>
                    <a href='javascript: sort_by_col(6, "str")'>Priority</a>
                </td>
                <td class=datah valign=bottom>
                    <a href='javascript: sort_by_col(7, "str")'>Assigned To</a>
                </td>
                <td class=datah valign=bottom>
                    <a href='javascript: sort_by_col(8, "str")'>Status</a>
                </td>
                <td class=datah valign=bottom>
                    <a href='javascript: sort_by_col(9, "str")'>Updated By</a>
                </td>
                <td class=datah valign=bottom>
                    <a href='javascript: sort_by_col(10, "str")'>Updated On</a>
                </td>
            </tr>
            <% if (BugList.Count > 0) { %>
                <%foreach (var bug in BugList) { %>
                <tr>
                    <td class=datad><%= bug.Id %></td>
                    <td class=datad><a href="edit_bug.aspx?id=<%= bug.Id %>"><%: bug.Description %></a></td>
                    <td class=datad><%= bug.Organization %></td>
                    <td class=datad><%= bug.Category %></td>
                    <td class=datad><%= bug.ReportedBy %></td>
                    <td class=datad><%= bug.ReportedDate %></td>
                    <td class=datad><%= bug.Priority %></td>
                    <td class=datad><%= bug.AssignedTo %></td>
                    <td class=datad><%= bug.Status %></td>
                    <td class=datad><%= bug.LastUpdatedBy %></td>
                    <td class=datad><%= bug.LastUpdatedDate %></td>
                </tr>
                <%}
               } else {%>
            <tr><td colspan="11">No results found</td></tr>
                <%} %>
        </table>
    </div>
    <br />
    <div id=sortedby>&nbsp;</div>
        </form>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="footerScripts" runat="server">
</asp:Content>
