IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[emailed_links]') AND type in (N'U'))
BEGIN

CREATE TABLE emailed_links
(
el_id char(37) constraint pk_emailed_links primary key not null,
el_date datetime not null default(getdate()),
el_email nvarchar(120) not null,
el_action nvarchar(20) not null, -- "registration" or "forgot"
el_username nvarchar(40) null,
el_user_id int null,
el_salt int null,
el_password nvarchar(64) null,
el_firstname nvarchar(60) null,
el_lastname nvarchar(60) null
)

END