IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[categories]') AND type in (N'U'))
BEGIN

CREATE TABLE categories
(
ct_id int identity constraint pk_categories primary key not null,
ct_name nvarchar(80) not null,
ct_sort_seq int not null default(0),
ct_default int not null default(0)
)

create unique index unique_ct_name on categories (ct_name)


INSERT INTO categories (ct_name) values('bug')
INSERT INTO categories (ct_name) values('enhancement')
INSERT INTO categories (ct_name) values('task')
INSERT INTO categories (ct_name) values('question')
INSERT INTO categories (ct_name) values('ticket')

END