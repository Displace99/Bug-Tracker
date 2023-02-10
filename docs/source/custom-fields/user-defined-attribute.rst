######################
User Defined Attribute
######################

A "User Defined Attribute" is the simplest way to add one custom field. You can use the simple "user defined attribute" dropdown that applies to all projects. Edit these lines in Web.config to use and name this additional dropdown:

.. sourcecode:: xml
    
    <add key="ShowUserDefinedBugAttribute" value="1"/>
    <add key="UserDefinedBugAttributeName" value="YourAttribute"/>