IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[project_user_xref]') AND type in (N'U'))
BEGIN

CREATE TABLE project_user_xref
(
pu_id int identity constraint pk_project_user_xref primary key not null,
pu_project int not null,
pu_user int not null,
pu_auto_subscribe int not null default(0),
pu_permission_level int not null default(2), -- 0=none, 1=view only, 2=edit, 3=reporter 
pu_admin int not null default(0),
)

create unique index pu_index_1 on project_user_xref (pu_project, pu_user)
create unique index pu_index_2 on project_user_xref (pu_user, pu_project)

INSERT INTO project_user_xref (pu_project, pu_user, pu_permission_level) values (1,7,1)
INSERT INTO project_user_xref (pu_project, pu_user, pu_permission_level) values (2,7,1)
INSERT INTO project_user_xref (pu_project, pu_user, pu_permission_level) values (1,8,3)
INSERT INTO project_user_xref (pu_project, pu_user, pu_permission_level) values (2,8,0)
INSERT INTO project_user_xref (pu_project, pu_user, pu_permission_level) values (1,9,1)
INSERT INTO project_user_xref (pu_project, pu_user, pu_permission_level) values (2,9,1)

END