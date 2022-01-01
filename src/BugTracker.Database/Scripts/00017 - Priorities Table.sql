IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[priorities]') AND type in (N'U'))
BEGIN

CREATE TABLE priorities
(
pr_id int identity constraint pk_priorities primary key not null,
pr_name nvarchar(60) not null,
pr_sort_seq int not null default(0),
pr_background_color nvarchar(14) not null,
pr_style nvarchar(30) null,
pr_default int not null default(0)
)

create unique index unique_pr_name on priorities (pr_name)

INSERT INTO priorities (pr_name, pr_sort_seq, pr_background_color, pr_style) values ('high', 1, '#ff9999', 'pr1_')
INSERT INTO priorities (pr_name, pr_sort_seq, pr_background_color, pr_style) values ('med', 2, '#ffdddd', 'pr2_')
INSERT INTO priorities (pr_name, pr_sort_seq, pr_background_color, pr_style) values ('low', 3, '#ffffff', 'pr3_')

END