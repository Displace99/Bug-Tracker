IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[bugs]') AND type in (N'U'))
BEGIN

CREATE TABLE bugs 
(
bg_id int identity constraint pk_bugs primary key not null,
bg_short_desc nvarchar(200) not null,
bg_reported_user int not null,
bg_reported_date datetime not null,
bg_status int not null,
bg_priority int not null,
bg_org int not null,
bg_category int not null,
bg_project int not null,
bg_assigned_to_user int null,
bg_last_updated_user int null,
bg_last_updated_date datetime null,
bg_user_defined_attribute int null,
bg_project_custom_dropdown_value1 nvarchar(120) null,
bg_project_custom_dropdown_value2 nvarchar(120) null,
bg_project_custom_dropdown_value3 nvarchar(120) null,
bg_tags nvarchar(200) null
)

END