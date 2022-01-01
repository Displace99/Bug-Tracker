IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[bug_post_attachments]') AND type in (N'U'))
BEGIN

/* This table is for database storage of attachments */
CREATE TABLE bug_post_attachments
(
bpa_id int identity constraint pk_bug_post_attachements primary key not null,
bpa_post int not null,
bpa_content image not null
)

create unique index bpa_index on bug_post_attachments (bpa_post)

END