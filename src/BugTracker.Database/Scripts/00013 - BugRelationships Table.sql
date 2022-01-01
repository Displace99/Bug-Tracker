IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[bug_relationships]') AND type in (N'U'))
BEGIN

CREATE TABLE bug_relationships
(
re_id int identity constraint pk_bug_relationships primary key not null,
re_bug1 int not null,
re_bug2 int not null,
re_type nvarchar(500) null,
re_direction int not null
)

create unique index re_index_1 on bug_relationships (re_bug1, re_bug2)
create unique index re_index_2 on bug_relationships (re_bug2, re_bug1)

END