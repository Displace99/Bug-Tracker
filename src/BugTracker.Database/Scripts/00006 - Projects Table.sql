IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[projects]') AND type in (N'U'))
BEGIN

CREATE TABLE projects
(
pj_id int identity constraint pk_projects primary key not null,
pj_name nvarchar(80) not null,
pj_active int not null default(1),
pj_default_user int null,
pj_auto_assign_default_user int null,
pj_auto_subscribe_default_user int null,
pj_enable_pop3 int null,
pj_pop3_username varchar(50) null,
pj_pop3_password nvarchar(20) null,
pj_pop3_email_from nvarchar(120) null,
pj_enable_custom_dropdown1 int not null default(0),
pj_enable_custom_dropdown2 int not null default(0),
pj_enable_custom_dropdown3 int not null default(0),
pj_custom_dropdown_label1 nvarchar(80) null,
pj_custom_dropdown_label2 nvarchar(80) null,
pj_custom_dropdown_label3 nvarchar(80) null,
pj_custom_dropdown_values1 nvarchar(800) null,
pj_custom_dropdown_values2 nvarchar(800) null,
pj_custom_dropdown_values3 nvarchar(800) null,
pj_default int not null default(0),
pj_description nvarchar(200) null
)

create unique index unique_pj_name on projects (pj_name)

INSERT INTO projects (pj_name) values('project 1')
INSERT INTO projects (pj_name) values('project 2')

END