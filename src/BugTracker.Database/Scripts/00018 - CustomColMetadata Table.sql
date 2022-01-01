IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[custom_col_metadata]') AND type in (N'U'))
BEGIN

CREATE TABLE  custom_col_metadata
(
ccm_colorder int constraint pk_custom_col_metadata primary key not null,
ccm_dropdown_vals nvarchar(1000) not null default(''),
ccm_sort_seq int default(0),
ccm_dropdown_type varchar(20) null
)

END