IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[git_affected_paths]') AND type in (N'U'))
BEGIN

CREATE TABLE git_affected_paths
(
gitap_id int identity constraint pk_git_affected_paths primary key nonclustered not null,
gitap_gitcom_id int not null,
gitap_action nvarchar(8) not null,
gitap_path nvarchar(400) not null
)

create clustered index gitap_gitcom_index on git_affected_paths (gitap_gitcom_id)

END