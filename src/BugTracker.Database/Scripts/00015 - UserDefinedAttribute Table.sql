IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[user_defined_attribute]') AND type in (N'U'))
BEGIN

CREATE TABLE user_defined_attribute
(
udf_id int identity constraint pk_user_defined_attribute primary key not null,
udf_name nvarchar(60) not null,
udf_sort_seq int not null default(0),
udf_default int not null default(0)
)

create unique index unique_udf_name on user_defined_attribute (udf_name)

INSERT INTO user_defined_attribute (udf_name) values ('whatever')
INSERT INTO user_defined_attribute (udf_name) values ('anything')

END