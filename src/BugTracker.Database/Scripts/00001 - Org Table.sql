IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[orgs]') AND type in (N'U'))
BEGIN

CREATE TABLE orgs
(
og_id int identity constraint pk_orgs primary key not null,
og_name nvarchar(80) not null,
og_domain nvarchar(80),
og_non_admins_can_use int not null default(0),
og_external_user int not null default(0), /* external user can't view post marked internal */
og_can_be_assigned_to int not null default(1),
og_can_only_see_own_reported int not null default(0),
og_can_edit_sql int not null default(0),
og_can_delete_bug int not null default(0),
og_can_edit_and_delete_posts int not null default(0),
og_can_merge_bugs int not null default(0),
og_can_mass_edit_bugs int not null default(0),
og_can_use_reports int not null default(0),
og_can_edit_reports int not null default(0),
og_can_view_tasks int not null default(0),
og_can_edit_tasks int not null default(0),
og_can_search int not null default(1),
og_other_orgs_permission_level int not null default(2), -- 0=none, 1=read, 2=edit
og_can_assign_to_internal_users int not null default(0),

og_category_field_permission_level int not null default(2), -- 0=none, 1=read, 2=edit
og_priority_field_permission_level int not null default(2),
og_assigned_to_field_permission_level int not null default(2),
og_status_field_permission_level int not null default(2),
og_project_field_permission_level int not null default(2),
og_org_field_permission_level int not null default(2),
og_udf_field_permission_level int not null default(2),
og_tags_field_permission_level int not null default(2),
og_active int not null default(1)
)

create unique index unique_og_name on orgs (og_name)

INSERT INTO orgs (og_name, og_external_user, og_can_be_assigned_to, og_other_orgs_permission_level) values ('org1',0,1,2)
INSERT INTO orgs (og_name, og_external_user, og_can_be_assigned_to, og_other_orgs_permission_level) values ('developers',0,1,2)
INSERT INTO orgs (og_name, og_external_user, og_can_be_assigned_to, og_other_orgs_permission_level) values ('testers',0,1,2)
INSERT INTO orgs (og_name, og_external_user, og_can_be_assigned_to, og_other_orgs_permission_level) values ('client one',1,0,0)
INSERT INTO orgs (og_name, og_external_user, og_can_be_assigned_to, og_other_orgs_permission_level) values ('client two',1,0,0)

END