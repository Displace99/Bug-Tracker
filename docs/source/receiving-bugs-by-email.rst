=======================================
Receiving email using btnet_service.exe
=======================================
One of the powerful features of BugTracker.NET is its ability to receive an email and treat it as a bug entry. Once it's in the database, you can respond to the email from within BugTracker.NET and your response will be tracked. The apps that receive the emails are btnet_console.exe and btnet_service.exe. Btnet_console.exe and btnet_service.exe are POP3 clients that fetch email and stuff them in the database as bugs. You configure the email accounts on the project pages - you can use a different email account per project. You can also respond to emails from within btnet and the responses are saved.

Btnet_console.exe and btnet_service.exe do the same thing, but one is just a console app and the other is a service. Try running the console app first because it is verbose and will help you debug your configuration.

Both of them get their settings from the "btnet_service.exe.config" file.

Running btnet_console:

Enter "btnet_console.exe btnet_service.exe.config" when you are in btnet's directory.

Running btnet_service.exe:

Use "installutil.exe" to install btnet_service.exe as a Windows service. Here's the command I typed, all on one line:

c:\Windows\Microsoft.NET\Framework\v4.0.30319>installutil c:\btnet\btnet_service\bin\release\btnet_service.exe

And if it works, this the output I see:

.. source::
    
    Microsoft (R) .NET Framework Installation utility Version 4.0.30319.1
    Copyright (c) Microsoft Corporation.  All rights reserved.

    Running a transacted installation.

    Beginning the Install phase of the installation.
    See the contents of the log file for the c:\btnet\btnet_service\bin\release\
    btnet_service.exe assembly's progress.
    The file is located at c:\btnet\btnet_service\bin\release\btnet_service.Inst
    allLog.
    Installing assembly 'c:\btnet\btnet_service\bin\release\btnet_service.exe'.
    Affected parameters are:
    logtoconsole =
    logfile = c:\btnet\btnet_service\bin\release\btnet_service.InstallLog
    assemblypath = c:\btnet\btnet_service\bin\release\btnet_service.exe
    Installing service btnet_service...
    Service btnet_service has been successfully installed.
    Creating EventLog source btnet_service in log Application...

    The Install phase completed successfully, and the Commit phase is beginning.
    See the contents of the log file for the c:\btnet\btnet_service\bin\release\
    btnet_service.exe assembly's progress.
    The file is located at c:\btnet\btnet_service\bin\release\btnet_service.Inst
    allLog.
    Committing assembly 'c:\btnet\btnet_service\bin\release\btnet_service.exe'.
    Affected parameters are:
    logtoconsole =
    logfile = c:\btnet\btnet_service\bin\release\btnet_service.InstallLog
    assemblypath = c:\btnet\btnet_service\bin\release\btnet_service.exe

    The Commit phase completed successfully.

    The transacted install has completed.

Btnet_service.exe looks for btnet_service.exe.config in its own folder. (Older versions were hard-coded to look in the c:\ folder).

Read the comments in btnet_service.exe.config. Note that one service instance can support multiple websites.

The service communicates with the website via the page insert_bug.aspx. It cannot use windows security.

Fetch email without service
===========================

Instead of running btnet_service.exe and configuring it via btnet_service.exe.config, you can specify in Web.config that the ASP.NET application itself creates a worker thread that does the same thing.   On the one hand, this new way is easier to setup and configure than btnet_service.exe.   On the other hand, this new way won't work if you are running BugTracker.NET on a shared host with ASP.NET "Medium Trust", because "Medium Trust" won't allow an ASP.NET to create an outgoing TCP/IP connection to the POP3 server.  See the "Pop3..." settings in Web.config for more info.