===========================================
Setting up outgoing email and notifications
===========================================
You can send email from within BugTracker.NET and these emails are then tracked with the bug, so that you don't end up with emails in one system and bugs in another.

Also, BugTracker.NET can send you email notifications telling you that somebody has added or changed a bug that you are interested in.

You configure email by changing settings in Web.config, according to the requirements of your SMTP service provider. For example, here are my settings for my GMail account:

.. source:: xml
	<mailSettings>
		<smtp> 
			<network
				host="smtp.gmail.com"
				port="465"
				userName="ctrager"
				password="MY PASSWORD"
			/>
		</smtp>
	</mailSettings>

There are some more examples in Web.config.
Getting SMTP settings to work seems to be one of the trickier setup tasks. If you are having trouble, you are going to want to blame BugTracker.NET, but BugTracker.NET is used by thousands of organizations around the world and it has been around for many years. So, if you are having a problem with email, it's probably a mistake in YOUR configuration rather than a bug in BugTracker.NET.

If you are having trouble, first try to get the "send email" link working on "edit_bug.aspx". The "send email" link tests your basic SMTP connectivity, settings, etc. If your "send email" is not working, try setting up Outlook Express (or Thunderbird, Eudora...) to send emails from the IIS box. Maybe the "Settings" dialogs of Outlook Express will be easier to configure for you than my Web.config file. After you have Outlook Express working, if you aren't sure how to translate your Outlook Express settings to Web.config, visit the help forum.  If you want help from me personally (sorry, not free), contact me at ctrager@yahoo.com.

If you can't even get Outlook Express to work, there's a diagnostic tool you can download from Microsoft, SMTPDiag.exe. Run it, and see what it says.


**Notifications**
If the "send email" is working but you or other users are not receiving notifications, first test that YOU can receive notifications, following these steps:

1. Click on the "settings" and then the "email notification settings" button.

2. Check "Auto-subscribe to all items" and make sure that all the other "Auto-subscribe" options are unchecked.

3. Make sure that the three dropdowns at the bottom all say "when anything changes" and that the last checkbox, "Send notifications even for items you add or change" is also checked.

4. Edit your Web.config file to make sure the settings below are ok. NotificationEmailEnabled should be "1" and "FROM EMAIL HERE" should be changed to YOUR email address for this experiment.

<add key="NotificationEmailEnabled" value="1"/>
<add key="NotificationEmailFrom" value="FROM EMAIL HERE"/>

5. Add a bug. You should get a notification.

6. Some people have solved problems with notification emails by adding the following to Web.config:

<add key="SmtpForceReplaceOfBareLineFeeds" value="1"/>

If it works, edit your Web.config so that "FROM EMAIL HERE" is set to the email address you want BugTracker.NET to use. Some SMTP service providers will let you make up an email address, so that you could change "FROM EMAIL HERE" to "no_reply@your_domain.com", but others seem to require a real email address that they already know about.

The user settings page lets you control how noisy the emails are. I suggest you start off letting BugTracker.NET be very noisy. Once you are sure it's working, you can go to your user settings to reduce the number of emails you receive.

 

**Other Troubleshooting Tips**
If your notification emails are going to Gmail or Yahoo accounts and show up blank there, then you are experiencing the `problem described here <https://sourceforge.net/p/btnet/bugs/400/>`_ . To workaround the bug, create "btnet_base_notifications.css" and "btnet_custom_notifications.css" and then the app will use those files as the style sheets for the emails instead of "btnet_base.css" and "btnet_custom.css". You can create those optional files and just leave them empty as a work around for the problems. Unfortunately, these style sheets will apply for all your users that get notifications, not just the ones who use Gmail or Yahoo.

If you are are using GMail's SMTP server, users have reported that GMail limits the number of emails you can send per day and will temporarily disable your outgoing email if you exceed the limit.

IIS on Vista does not come with its own SMTP server. If you want to run your own SMTP server, hMailServer might be a good one to try first.