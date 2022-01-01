IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dashboard_items]') AND type in (N'U'))
BEGIN

CREATE TABLE dashboard_items
(
ds_id int identity constraint pk_dashboard_items primary key nonclustered not null,
ds_user int not null,
ds_report int not null,
ds_chart_type varchar(8) not null,
ds_col int not null,
ds_row int not null
)

create clustered index ds_user_index on dashboard_items (ds_user)

END