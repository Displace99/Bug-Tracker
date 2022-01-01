IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[users]') AND type in (N'U'))
BEGIN

CREATE TABLE users
(
us_id int identity constraint pk_users primary key not null,
us_username nvarchar(40) not null,
us_salt int null,
us_password nvarchar(64) not null,
us_firstname nvarchar(60) null,
us_lastname nvarchar(60) null,
us_email nvarchar(120) null,
us_admin int not null default(0),
us_default_query int not null default(0),
us_enable_notifications int not null default(1),
us_auto_subscribe int not null default(0),
us_auto_subscribe_own_bugs int null default(0),
us_auto_subscribe_reported_bugs int null default(0),
us_send_notifications_to_self int null default(0),
us_active int not null default(1),
us_bugs_per_page int null,
us_forced_project int null,
us_reported_notifications int not null default(4),
us_assigned_notifications int not null default(4),
us_subscribed_notifications int not null default(4),
us_signature nvarchar(1000) null,
us_use_fckeditor int not null default(0),
us_enable_bug_list_popups int not null default(1),
/* who created this user */
us_created_user int not null default(1),
us_org int not null default(1),
us_most_recent_login_datetime datetime null
)

create unique index unique_us_username on users (us_username)

INSERT INTO users (
us_username, us_firstname, us_lastname, us_password, us_admin, us_default_query, us_org)
values ('admin', 'System', 'Administrator', 'admin', 1, 1, 1)

INSERT INTO users (
us_username, us_firstname, us_lastname, us_password, us_admin, us_default_query, us_org)
values ('developer', 'Al', 'Kaline', 'admin', 0, 2, 2)

INSERT INTO users (
us_username, us_firstname, us_lastname, us_password, us_admin, us_default_query, us_org)
values ('tester', 'Norman', 'Cash', 'admin', 0, 4, 4)

INSERT INTO users (
us_username, us_firstname, us_lastname, us_password, us_admin, us_default_query, us_org)
values ('customer1', 'Bill', 'Freehan', 'admin', 0, 1, 4)

INSERT INTO users (
us_username, us_firstname, us_lastname, us_password, us_admin, us_default_query, us_org)
values ('customer2', 'Denny', 'McClain', 'admin', 0, 1, 5)

INSERT INTO users (
us_username, us_firstname, us_lastname, us_password, us_admin, us_default_query)
values ('email', 'for POP3', 'btnet_service.exe', 'x', 0, 1)

INSERT INTO users (
us_username, us_firstname, us_lastname, us_password, us_admin, us_default_query, us_forced_project)
values ('viewer', 'Read', 'Only', 'admin', 0, 1, 1)

INSERT INTO users (
us_username, us_firstname, us_lastname, us_password, us_admin, us_default_query, us_forced_project)
values ('reporter', 'Report And', 'Comment Only', 'admin', 0, 1, 1)

INSERT INTO users (
us_username, us_firstname, us_lastname, us_password, us_admin, us_default_query, us_forced_project, us_active)
values ('guest', 'Special', 'cannot save searches, settings', 'guest', 0, 1, 1, 0)

END