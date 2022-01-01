IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[bug_posts]') AND type in (N'U'))
BEGIN

CREATE TABLE bug_posts
(
bp_id int identity constraint pk_bug_posts primary key nonclustered not null,
bp_bug int not null,
bp_type varchar(8) not null,
bp_user int not null,
bp_date datetime not null,
bp_comment ntext not null,
bp_comment_search ntext null,
bp_email_from nvarchar(800) null,
bp_email_to nvarchar(800) null,
bp_file nvarchar(1000) null,
bp_size int null,
bp_content_type nvarchar(200) null,
bp_parent int null,
bp_original_comment_id int null,
bp_hidden_from_external_users int not null default(0),
bp_email_cc nvarchar(800) null
)

create clustered index bp_index_1 on bug_posts (bp_bug)

END