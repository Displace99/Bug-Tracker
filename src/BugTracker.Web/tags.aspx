<%@ Page language="C#" CodeBehind="tags.aspx.cs" Inherits="BugTracker.Web.tags" AutoEventWireup="True" %>

<html>
<head>
<title>Tags</title>
<script>
function do_unload()
{
    opener.done_selecting_tags()
}
</script>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body onunload="do_unload()">

<p>
<% print_tags(); %>

</body>
</html>
