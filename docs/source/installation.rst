###############
Getting Started
###############

**************
Installation
**************
I try to make BugTracker.NET easy to install. For people with an ASP.NET development background, it is easy. If you do run into a problem, open an issue in the GitHub Repo.

Ok, the installation instructions:

Prerequisites:

* IIS
* ASP.NET Framework, 4.7.2 or better
* MS SQL Server


Installing the database
========================

.. info:: 
    Currently the BugTracker.NET database **cannot** be installed in the cloud due to how custom columns work.

BugTracker.NET uses a library called DbUp to manage the database. This includes both the initial installation as well as future upgrades. For more information see `DbUp on GitHub <https://github.com/DbUp/DbUp>`_.

1. Create a database on your database server. 
2. Open Visual Studio (2019 or later).
3. In the BugTracker.Database project, open the appsettings.json file and update the connection string with the location of your newly created database.
4. Run the BugTracker.Database console application. 


Installing the Web App
=======================

If you already have a CI/CD pipeline setup, you can continue to use that, otherwise follow the below instructions to use the Publish tool that is part of Visual Studio. For more detailed instructions on using the Publish tool see the `following documentation <https://docs.microsoft.com/en-us/visualstudio/deployment/quickstart-deploy-aspnet-web-app?view=vs-2022&tabs=folder>`_.

.. info:: 
    Although the database cannot currently be installed in the cloud, the web application can. 

1. Open Visual Studio (2019 or later).
2. Open the web.config file in the BugTracker.Web project and update the connection string with the location of your database from the above steps.
3. Build the application (Build > Build Solution).
4. Use the Publish option from within Visual Studio (Build > Publish BugTracker.Web), and follow the onscreen instructions. For detailed instructions see this documentation.
5. Navigate to the address you deployed your application to and you should see a login page. Login as user "admin", password "admin". 
6. Change the password to the admin account.

.. warning::
    If you are planning to use Bugtracker.NET on a public web server, an inTERnet, not an inTRAnet, then after you install you should:
    
    1. Change the "admin" password. Of course.
    
    2. Delete the "query.aspx" page.

At this point you should be able to add bugs by clicking on "add a bug". Please take some time to look at the rest of this documentation. Also, please read the comments in the Web.config to learn what else you can do with BugTracker.NET. BugTracker.NET is deceptively simple when you first install it, but by doing a quick read of the comments in Web.config and a quick pass through the documentation pages, you'll get some idea of the things you can customize.

Backing up BugTracker.NET
===========================
If you want to backup BugTracker.NET data, then backup the following:

1. Your SQL Server database (outside the scope of this documentation).
2. The files you have in the folder you have configured as your "UploadFolder" in Web.config. By default, BugTracker.NET saves uploads in the DB, so maybe you can skip this step.
3. Your configuration and customized files:
    a) Web.config
    b) btnet_service.exe.config
    c) any files you have changed in the www\custom folder
    d) hook scripts, for svn, git, hg
    e) any other files you have customized

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

Read the release notes to see what has changed, so you understand the impact the release will have to anything you have modified.

Usually you don't need to make any changes to your Web.config, but once in a while you do. The RELEASE_NOTES.TXT file will tell you when must make a change. Sometimes you might want to change your Web.config in order to turn on new features.

If there are updates to the database run the BugTracker.Database console app, which will update the databasee to the most recent changes.

.. warning::
    Don't accidentally re-copy query.aspx to your public website.