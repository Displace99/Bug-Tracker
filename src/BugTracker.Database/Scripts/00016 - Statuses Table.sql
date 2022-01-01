IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[statuses]') AND type in (N'U'))
BEGIN

CREATE TABLE statuses
(
st_id int identity constraint pk_statuses primary key not null,
st_name nvarchar(60) not null,
st_sort_seq int not null default(0),
st_style nvarchar(30) null,
st_default int not null default(0)
)

create unique index unique_st_name on statuses (st_name)

INSERT INTO statuses (st_name, st_sort_seq, st_style, st_default) values ('new', 1, 'st1', 1)
INSERT INTO statuses (st_name, st_sort_seq, st_style, st_default) values ('in progress', 2, 'st2', 0)
INSERT INTO statuses (st_name, st_sort_seq, st_style, st_default) values ('checked in', 3, 'st3', 0)
INSERT INTO statuses (st_name, st_sort_seq, st_style, st_default) values ('re-opened', 4, 'st4', 0)
INSERT INTO statuses (st_name, st_sort_seq, st_style, st_default) values ('closed', 5, 'st5', 0)

END