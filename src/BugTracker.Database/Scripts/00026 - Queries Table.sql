IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[queries]') AND type in (N'U'))
BEGIN

CREATE TABLE queries
(
qu_id int identity constraint pk_queries primary key not null,
qu_desc nvarchar(200) not null,
qu_sql ntext not null,
qu_default int null,
qu_user int null,
qu_org int null
)

create unique index unique_qu_desc on queries (qu_desc, qu_user, qu_org)

END