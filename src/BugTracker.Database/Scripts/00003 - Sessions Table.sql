IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sessions]') AND type in (N'U'))
BEGIN

CREATE TABLE sessions
(
	se_id char(37) constraint pk_sessions primary key not null,
	se_date datetime not null default(getdate()),
	se_user int not null
)

END