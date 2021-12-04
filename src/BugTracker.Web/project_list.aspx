<%@ Page Title="" Language="C#" MasterPageFile="~/BugTracker.Master" AutoEventWireup="true" CodeBehind="project_list.aspx.cs" Inherits="BugTracker.Web.project_list" %>
<asp:Content ID="Content1" ContentPlaceHolderID="headerScripts" runat="server">
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
                    <a href='javascript: sort_by_col(0, \"str\")'>Name</a>
                </td>
                <td class=datah valign=bottom>
                    <a href='javascript: sort_by_col(1, \"str\")'>Description</a>
                </td>
                <td class=datah valign=bottom>
                    <a href='javascript: sort_by_col(2, \"str\")'>Status</a>
                </td>
            </tr>
            <!-- For loop -->
            <tr>
                <td class=datad></td>
                <td class=datad></td>
                <td class=datad></td>
            </tr>
            <!-- End of for loop -->
        </table>
    </div>
    <br />
    <div id=sortedby>&nbsp;</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="footerScripts" runat="server">
</asp:Content>
