ALTER TABLE users alter column us_salt nvarchar(120);
ALTER TABLE users alter column us_password nvarchar(max);
ALTER TABLE emailed_links alter column el_salt nvarchar(120);
ALTER TABLE emailed_links alter column el_password nvarchar(max);