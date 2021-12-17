===============
Getting Started
===============

Installation
===========================
I try to make BugTracker.NET easy to install. For people with an ASP.NET development background, it is easy. If you do run into a problem, look for an answer in the forum, or ask a question and I'll usually respond the same day. But, if you simply would rather not be bothered, you can hire me to do the installation, including installation of IIS, SQL Server Express, and, if you like, integration with email, Subversion, Git, Mercurial, Active Directory, etc, either on your own hardware or at your shared or VPS host, for as little as $300. Contact me at ctrager@yahoo.com
Ok, the installation instructions:

Prerequisites:

IIS
ASP.NET Framework, 4.x or better
MS SQL Server
Installation Steps:

1. Extract the files from the zip file.
2. Create the IIS app. Launch IIS Manager, navigate to Default Web Site, Add Application. The Alias can be anything. The physical path should be the www subdirectory. For example, if you extracted the zip to c:\btnet_377, then the physical path of the application should be c:\btnet_377\www.
3. Create a database. The best way is to use SQL Server Management Studo.
4. Update Web.confg. Open the wwww\Web.config file in your favorite editor and follow the "Quick Start" instructions.
5. Log in. If you chose, for example, "btnet" as your Alias, then point your browser to http://localhost/btnet. You should see a login page. Login as user "admin", password "admin".
You will be prompted to run the www\setup.sql script, which creates the database tables, from a web page form, or you could also do it in your favorite db admin tool.

6. Read this warning:

.. warning::
    If you are planning to use Bugtracker.NET on a public web server, an inTERnet, not an inTRAnet, then after you install you should:
    1. Change the "admin" password. Of course.
    2. Delete the files "query.aspx" and "install.aspx".

At this point you should be able to add bugs by clicking on "add a bug". Please take some time to look at the rest of this documentation. Also, please read the comments in the Web.config to learn what else you can do with BugTracker.NET. BugTracker.NET is deceptively simple when you first install it, but by doing a quick read of the comments in Web.config and a quick pass through the documentation pages, you'll get some idea of the things you can customize.

Backing up BugTracker.NET
===========================
If you want to backup BugTracker.NET data, then backup the following:

1. Your SQL Server database

2. The files you have in the folder you have configured as your "UploadFolder" in Web.config. By default, BugTracker.NET saves uploads in the DB, so maybe you can skip this step.

3. Your configuration and customized files:
    a) Web.config
    b) btnet_service.exe.config
    c) any files you have changed in the www\custom folder
    d) hook scripts, for svn, git, hg

Creating a SQL Server Login/User
================================
If are using SQL Server authentication and if you want to create a new SQL Server Login/User with just the minimum permissions, below is the script to do it. The db_ddladmin permission is only needed for creating custom columns via the BugTracker.NET "admin" page. The db_backupoperator permission is only needed to do backups via the BugTracker.NET "admin" page.

.. sourcecode:: sql
    
    CREATE LOGIN user1 WITH PASSWORD = N'pass1', DEFAULT_DATABASE = btnet
    CREATE USER user1 FOR LOGIN user1
    EXEC sp_addrolemember N'db_datawriter', N'user1'
    EXEC sp_addrolemember N'db_datareader', N'user1'
    -- to create custom columns
    EXEC sp_addrolemember N'db_ddladmin', N'user1'
    -- to backup the db
    EXEC sp_addrolemember N'db_backupoperator', N'user1'


Upgrading
===========================
If you are currently running an old version of BugTracker.NET and you have downloaded the most recent version of BugTracker.NET and want to upgrade to it, here are general instructions:

Make sure you have a backup of your database, your Web.config file, and any other files that you have customized, such as btnet_custom.css or custom_header.html. Maybe look at the last modified dates of your files to double check which ones you have customized.

Read RELEASE_NOTES.TXT and follow the instructions for each individual release. You only need to download the latest release, but you do need to follow the instructions in RELEASE_NOTES.TXT for each intermediate release.

Often, RELEASE_NOTES.TXT directs you to run SQL in the file "upgrade.sql". Run the SQL a little bit at a time, one release at a time.

Usually you don't need to make any changes to your Web.config, but once in a while you do. The RELEASE_NOTES.TXT file will tell you when must make a change. See for example the release notes for release 2.7.5. Sometimes you might want to change your Web.config in order to turn on new features.

Overlay the files in your virtual directory with the new files from the .zip EXCEPT Web.config and the files in your "custom" folder. Don't overlay your Web.config file.

.. warning::
    Don't accidentally re-copy install.aspx and query.aspx to your public website.