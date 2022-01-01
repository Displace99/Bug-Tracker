IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[reports]') AND type in (N'U'))
BEGIN

CREATE TABLE reports
(
rp_id int identity constraint pk_reports primary key not null,
rp_desc nvarchar(200) not null,
rp_sql ntext not null,
rp_chart_type varchar(8) not null
)

create unique index unique_rp_desc on reports (rp_desc)

END