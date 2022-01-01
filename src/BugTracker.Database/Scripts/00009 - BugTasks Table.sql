IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[bug_tasks]') AND type in (N'U'))
BEGIN

CREATE TABLE bug_tasks
(
tsk_id int identity constraint pk_bug_tasks primary key nonclustered not null,
tsk_bug int not null,
tsk_created_user int not null,
tsk_created_date datetime not null,
tsk_last_updated_user int not null,
tsk_last_updated_date datetime not null,

tsk_assigned_to_user int null,
tsk_planned_start_date datetime null,
tsk_actual_start_date datetime null,
tsk_planned_end_date datetime null,
tsk_actual_end_date datetime null,
tsk_planned_duration decimal(6,2) null,
tsk_actual_duration decimal(6,2) null,
tsk_duration_units nvarchar(20) null,
tsk_percent_complete int null,
tsk_status int null,
tsk_sort_sequence int null,
tsk_description nvarchar(400) null,
)

create clustered index tsk_index_1 on bug_tasks (tsk_bug)

END