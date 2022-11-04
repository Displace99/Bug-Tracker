IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[hg_revisions]') AND type in (N'U'))
BEGIN

-- Table holds information regarding Mercurial Source Control Management 
CREATE TABLE hg_revisions
(
hgrev_id int identity constraint pk_hg_revisions primary key nonclustered not null,
hgrev_revision int,
hgrev_bug int not null,
hgrev_repository nvarchar(400) not null,
hgrev_author nvarchar(100) not null,
hgrev_hg_date nvarchar(100) not null,
hgrev_btnet_date datetime not null,
hgrev_msg ntext not null
)

create clustered index hg_bug_index on hg_revisions (hgrev_bug)

create unique index hg_unique_revision on hg_revisions (hgrev_revision)

END