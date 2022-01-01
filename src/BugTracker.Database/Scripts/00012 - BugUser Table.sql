IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[bug_user]') AND type in (N'U'))
BEGIN

CREATE TABLE bug_user
(
bu_bug int not null,
bu_user int not null,
bu_flag int not null,
bu_flag_datetime datetime null,
bu_seen int not null,
bu_seen_datetime datetime null,
bu_vote int not null,
bu_vote_datetime datetime null,
)

create unique clustered index bu_index_1 on bug_user (bu_bug, bu_user)

END