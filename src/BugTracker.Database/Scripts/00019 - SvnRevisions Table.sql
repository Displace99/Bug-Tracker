IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[svn_revisions]') AND type in (N'U'))
BEGIN

CREATE TABLE svn_revisions
(
svnrev_id int identity constraint pk_svn_revisions primary key nonclustered not null,
svnrev_revision int not null,
svnrev_bug int not null,
svnrev_repository nvarchar(400) not null,
svnrev_author nvarchar(100) not null,
svnrev_svn_date nvarchar(100) not null,
svnrev_btnet_date datetime not null,
svnrev_msg ntext not null
)

create clustered index svn_bug_index on svn_revisions (svnrev_bug)

END