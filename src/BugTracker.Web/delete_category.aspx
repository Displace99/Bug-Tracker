<%@ Page language="C#" CodeBehind="delete_category.aspx.cs" Inherits="BugTracker.Web.delete_category" AutoEventWireup="True" %>
<%@ Import Namespace="btnet" %>

<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->

<!DOCTYPE html>

<html lang="en">
    <head>
        <title id="title" runat="server">btnet delete category</title>
        <link rel="StyleSheet" href="btnet.css" type="text/css">
    </head>
    <body>
        <% security.write_menu(Response, "admin"); %>
        
        <p></p>
        <div class=align>
            <p>&nbsp</p>
            <a href=categories.aspx>back to categories</a>

            <p>or<p>

            <form runat="server" id="frm">
                <a id="confirm_href" runat="server" href="javascript: submit_form()"></a>
                <input type="hidden" id="row_id" runat="server">
            </form>

        </div>
        <script>
            function submit_form() {
                var frm = document.getElementById("frm");
                frm.submit();
                return true;
            }
        </script>
        <% Response.Write(Application["custom_footer"]); %>

    </body>
</html>


