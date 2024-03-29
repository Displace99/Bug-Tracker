<%@ Page language="C#" CodeBehind="edit_query.aspx.cs" Inherits="BugTracker.Web.edit_query"  validateRequest="false" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<!DOCTYPE html>

<html lang="en">
	<head>
		<title id="title" runat="server">btnet edit query</title>
		<link rel="StyleSheet" href="btnet.css" type="text/css">
		<link rel="stylesheet" href="css/alert.css" type="text/css" />
		<script language="Javascript" type="text/javascript" src="edit_area/edit_area_full.js"></script>

		<script>
			editAreaLoader.init({
				id: "sql_text"	// id of the textarea to transform
				,start_highlight: true	// if start with highlight
				,toolbar: "search, go_to_line, undo, redo, help"
				,browsers: "all"
				,language: "en"
				,syntax: "sql"
				,allow_toggle: false
				,min_height: 300
				,min_width: 400
			});
		</script>		

	</head>
	<body>
		<% security.write_menu(Response, "queries"); %>

		<div class=align>
			<div class="alert-danger">Due to potential security concerns, this page will be removed in a future update.</div>  
			<table border=0>
				<tr>
					<td>
						<a href=queries.aspx>back to queries</a>
						<form class=frm runat="server">
							<table border=0 cellspacing=8 cellpadding=0>

								<tr>
									<td class=lbl>Description:</td>
									<td><input runat="server" type=text class=txt id="desc" maxlength=80 size=80></td>
									<td runat="server" class=err id="desc_err">&nbsp;</td>
								</tr>

								<tr>
									<td class=lbl runat="server" id="visibility_label">Visibility:</td>
									<td colspan=2>
										<asp:RadioButton text="Everybody" runat="server" GroupName="visibility" id="vis_everybody"/>
										&nbsp;&nbsp;&nbsp;
		
										<asp:RadioButton text="Just User" runat="server" GroupName="visibility" id="vis_user"/>
										&nbsp;&nbsp;&nbsp;
										<asp:DropDownList id="user" runat="server">
										</asp:DropDownList>
										&nbsp;&nbsp;
										<span runat="server" class=err id="user_err">&nbsp;</span>		
		
										<asp:RadioButton text="Users with org" runat="server" GroupName="visibility" id="vis_org"/>
										<asp:DropDownList id="org" runat="server">
										</asp:DropDownList>
										&nbsp;&nbsp;
										<span runat="server" class=err id="org_err">&nbsp;</span>
									</td>

								<tr>
								<td colspan=3>
									<span class=lbl id="sql_text_label" runat="server">SQL:</span><br>
				
									<textarea style="height:300px; width:800px;" runat="server" class=txt name="sql_text" id="sql_text"></textarea>

								</td>


								<tr>
									<td colspan=3 align=center>
										<span runat="server" class=err id="msg">&nbsp;</span>
									</td>

								</tr>

								<tr>
									<td colspan=2 align=center>
										<input runat="server" class=btn type=submit id="sub" value="Create or Edit">
										<td>&nbsp</td>
									</td>
								</tr>

							</table>
						</form>

						<p>&nbsp;</p>

						<div id="explanation" style="width:800px" class=cmt runat="server">
							In order to work with the bugs.aspx page, your SQL must be structured in a particular way.
							The first column must be either a color starting with "#" or a CSS style class.  
							If it starts with "#", it will be interpreted as the background color of the row.
							Otherwise, it will be interpreted as the name of a CSS style class in your CSS file.
							<br>
							<br>
							View this <a target="_blank" href="edit_styles.aspx">example</a> of one way to change the color of your rows.  
							The example uses a combination of priority and status to determine the CSS style, but feel free to come up with your own scheme.
							<br>
							<br>
							The second column must be "bg_id".
							<br><br>
							<b>"$ME"</b> is a magic word you can use in your query that gets replaced by your user ID.
							<br>
							For example:
							<br>
							<ul>
								select isnull(pr_background_color,'#ffffff'), bg_id [id], bg_short_desc<br>
								from bugs<br>
								left outer join priorities on bg_priority = pr_id<br>
								where bg_assigned_to_user = $ME
							</ul>
							<br>
							<b>"$FLAG"</b> is a magic word that controls whether a query shows the "flag" column that lets an individual user flag items for himself.<br>
							To use it, add the SQL shown below to your select columns and do a "left outer join" to the bug_user table.
							<ul>
								Select ...., isnull(bu_flag,0) [$FLAG],...<br>
								from bugs<br>
								left outer join bug_user on bu_bug = bg_id and bu_user = $ME
							</ul>
							<br>
							<b>"$SEEN"</b> is a magic word that controls whether a query shows the "new" column.  The new column works the same as an indicator for unread email.
							To use it, add the SQL shown below to your select columns and do a "left outer join" to the bug_user table.
							<ul>
								Select ...., isnull(bu_seen,0) [$SEEN],...<br>
								from bugs<br>
								left outer join bug_user on bu_bug = bg_id and bu_user = $ME
							</ul>
							<br>
							<b>"$VOTE"</b> is a magic word that controls whether a query shows the "votes" column.  Each user can upvote a bug just once.
							To use it, add the strange looking SQL shown below to your select columns and do the two joins shown below, to votes_view and bug_user.
							<ul>
								Select ...., (isnull(vote_total,0) * 10000) + isnull(bu_vote,0) [$VOTE],...<br>
								from bugs<br>
								left outer join bug_user on bu_bug = bg_id and bu_user = $ME<br>
								left outer join votes_view on vote_bug = bg_id
							</ul>	
						</div>

					</td>
				</tr>
			</table>
		</div>
		<% Response.Write(Application["custom_footer"]); %>
	</body>
</html>


