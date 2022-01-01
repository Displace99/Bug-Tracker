IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[bug_subscriptions]') AND type in (N'U'))
BEGIN

CREATE TABLE bug_subscriptions
(
bs_bug int not null,
bs_user int not null,
)

create unique index bs_index_1 on bug_subscriptions (bs_user, bs_bug)
create unique clustered index bs_index_2 on bug_subscriptions (bs_bug, bs_user)

END