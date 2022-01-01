IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[queued_notifications]') AND type in (N'U'))
BEGIN

CREATE TABLE queued_notifications
(
qn_id int identity constraint pk_queued_notificatons primary key not null,
qn_date_created datetime not null,
qn_bug int not null,
qn_user int not null,
qn_status nvarchar(30) not null,
qn_retries int not null,
qn_last_exception nvarchar(1000) not null,
qn_to nvarchar(200) not null,
qn_from nvarchar(200) not null,
qn_subject nvarchar(400) not null,
qn_body ntext not null
)

END