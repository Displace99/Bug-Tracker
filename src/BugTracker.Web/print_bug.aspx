<%@ Page language="C#" CodeBehind="print_bug.aspx.cs" Inherits="BugTracker.Web.print_bug" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<%@ Import Namespace="btnet" %>


<html>
<head>
<title  id="titl" runat="server">btnet edit bug</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<style>
a {text-decoration: underline; }
a:visited {text-decoration: underline; }
a:hover {text-decoration: underline; }
</style>

</head>

<% 
	PrintBug.print_bug(Response,
       dr,
       security,
       false, // include style
       images_inline,
       history_inline,
       true);  // internal_posts 
%>

</html>
