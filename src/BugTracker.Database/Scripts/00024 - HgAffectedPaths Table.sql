IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[hg_affected_paths]') AND type in (N'U'))
BEGIN

-- Table holds information regarding Mercurial Source Control Management 
CREATE TABLE hg_affected_paths
(
hgap_id int identity constraint pk_hg_affected_paths primary key nonclustered not null,
hgap_hgrev_id int not null,
hgap_action nvarchar(8) not null,
hgap_path nvarchar(400) not null
)

create clustered index hgap_hgrev_index on hg_affected_paths (hgap_hgrev_id)

END