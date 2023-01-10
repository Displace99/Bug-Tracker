<%@ Page language="C#" CodeBehind="delete_query.aspx.cs" Inherits="BugTracker.Web.delete_query" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->


<html>
<head>
<title id="titl" runat="server">btnet delete query</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body>
<% security.write_menu(Response, "queries"); %>
<p>
<div class=align>
<p>&nbsp</p>
<a href=queries.aspx>back to queries</a>

<p>or<p>

<script>
function submit_form()
{
    var frm = document.getElementById("frm");
    frm.submit();
    return true;
}

</script>
<form runat="server" id="frm">
<a id="confirm_href" runat="server" href="javascript: submit_form()"></a>
<input type="hidden" id="row_id" runat="server">
</form>

</div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>


