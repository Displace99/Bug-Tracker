IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[svn_affected_paths]') AND type in (N'U'))
BEGIN

CREATE TABLE svn_affected_paths
(
svnap_id int identity constraint pk_svn_affected_paths primary key nonclustered not null,
svnap_svnrev_id int not null,
svnap_action nvarchar(8) not null,
svnap_path nvarchar(400) not null
)

create clustered index svn_revision_index on svn_affected_paths (svnap_svnrev_id)

END