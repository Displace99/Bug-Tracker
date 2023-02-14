<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MainMenu.ascx.cs" Inherits="BugTracker.Web.Controls.MainMenu" %>
<%@ Import Namespace="btnet" %>
<%@ Import Namespace="btnet.Security" %>
<%= Util.context.Application["custom_header"]%>

<span id=debug style='position:absolute;top:0;left:0;'></span>
<script>
	function dbg(s)
	{
		document.getElementById('debug').innerHTML += (s + '<br>')
	}
	function on_submit_search()
	{
		el = document.getElementById('lucene_input')
		if (el.value == '')
		{
			alert('Enter the words you are search for.');
			el.focus()
			return false;
		}
		else
		{
			return true;
		}

	}
</script>

<table border="0" width="100%" cellpadding="0" cellspacing="0" class="menubar">
	<tr>
		<%= Util.context.Application["custom_logo"] %>
        <td width="20">&nbsp;</td>
        <td class='menu_td'>
			<a href="project_list.aspx"><span class='menu_item warn' style='margin-left: 3px;'>projects</span></a>
		</td>
		<td class='menu_td'>
            <a href="bugs.aspx"><span class='menu_item warn' style='margin-left: 3px;'><%:btnet.Util.get_setting("PluralBugLabel", "bugs") %></span></a>
		</td>
		<% if (user.can_search)
        {
		%>
			<td class='menu_td'>
				<a href="search.aspx"><span class='menu_item warn' style='margin-left: 3px;'>search</span></a>
			</td>
		<% } %>

		 <% if (Util.get_setting("EnableWhatsNewPage", "0") == "1")
           { %>
        <td class='menu_td'>
            <a href="view_whatsnew.aspx"><span class='menu_item warn' style='margin-left: 3px;'>news</span></a>
        </td>
        <% } %>

		<% if (!user.is_guest)
            { %>
                <td class='menu_td'>
					<a href="queries.aspx"><span class='menu_item warn' style='margin-left: 3px;'>queries</span></a>
				</td>
           <% } %>

            <% if (user.is_admin || user.can_use_reports || user.can_edit_reports)
            { %>
                <td class='menu_td'>
					<a href="reports.aspx"><span class='menu_item warn' style='margin-left: 3px;'>reports</span></a>
				</td>
           <% } %>

            <% if (Util.get_setting("CustomMenuLinkLabel", "") != "")
            { %>
                <td class='menu_td'>
					<a href="<%:Util.get_setting("CustomMenuLinkUrl", "") %>"><span class='menu_item warn' style='margin-left: 3px;'><%:Util.get_setting("CustomMenuLinkLabel", "") %></span></a>
				</td>
            <% }%>

            <% if (user.is_admin) 
            { %>
                <td class='menu_td'>
					<a href="admin.aspx"><span class='menu_item warn' style='margin-left: 3px;'>admin</span></a>
				</td>
            <% } 
            else if (user.is_project_admin)
            { %>
                <td class='menu_td'>
					<a href="users.aspx"><span class='menu_item warn' style='margin-left: 3px;'>users</span></a>
				</td>
            <% } %>

			<td nowrap valign="middle">
				<form style='margin: 0px; padding: 0px;' action="edit_bug.aspx" method="get">
					<input class="menubtn" type="submit" value='go to ID'>
					<input class="menuinput" size="4" type="text" class="txt" name="id" accesskey="g">
				</form>
			</td>

			<% if (Util.get_setting("EnableLucene", "1") == "1" && user.can_search)
            { %>
				<td nowrap valign="middle">
					<form style='margin: 0px; padding: 0px;' action="search_text.aspx" method="get" onsubmit='return on_submit_search()'>
						<input class='menubtn' type='submit' value='search text'>
						<input class='menuinput' id='lucene_input' size='24' type='text' class='txt'
							value='<%:(string) HttpContext.Current.Session["query"] %>' name="query" accesskey="s">
						<a href='lucene_syntax.html' target='_blank' style='font-size: 7pt;'>advanced</a>
					</form>
				</td>
        <% }%>

		<td nowrap valign="middle">
            <span class="smallnote">logged in as <%:user.username %><br></span>
		</td>
        <td class='menu_td'>
            <a href="logoff.aspx"><span class='menu_item warn' style='margin-left: 3px;'>logoff</span></a>
        </td>
		
        <% if (!user.is_guest)
            { %>
                <td class="menu_td">
					<a href="edit_self.aspx"><span class="menu_item warn" style="margin-left: 3px;">settings</span></a>
				</td>
           <% } %>

		<td valign="middle" align="left">
			<a target=_blank href=about.html><span class='menu_item' style='margin-left:3px;'>about</span></a>
		</td>
		<td nowrap valign=middle>
			<a target="_blank" href="https://bug-tracker.readthedocs.io/en/latest/">
				<span class="menu_item" style="margin-left:3px;">help</span>
			</a>
		</td>
	</tr>
</table>
<br />