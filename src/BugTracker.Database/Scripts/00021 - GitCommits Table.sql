IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[git_commits]') AND type in (N'U'))
BEGIN

CREATE TABLE git_commits
(
gitcom_id int identity constraint pk_git_commits primary key nonclustered not null,
gitcom_commit char(40),
gitcom_bug int not null,
gitcom_repository nvarchar(400) not null,
gitcom_author nvarchar(100) not null,
gitcom_git_date nvarchar(100) not null,
gitcom_btnet_date datetime not null,
gitcom_msg ntext not null
)

create clustered index git_bug_index on git_commits (gitcom_bug)

create unique index git_unique_commit on git_commits (gitcom_commit)

END