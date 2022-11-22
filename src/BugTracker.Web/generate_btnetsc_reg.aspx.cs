using btnet;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace BugTracker.Web
{
    public partial class generate_btnetsc_reg : Page
    {
        protected Security security;

        ///////////////////////////////////////////////////////////////////////
        void Page_Load(Object sender, EventArgs e)
        {
            security = new Security();
            security.check_security(HttpContext.Current, Security.ANY_USER_OK);

            Response.ContentType = "text/reg";
            Response.AddHeader("content-disposition", "attachment; filename=\"btnetsc.reg\"");
            Response.Write("Windows Registry Editor Version 5.00");
            Response.Write("\n\n");
            Response.Write("[HKEY_CURRENT_USER\\Software\\BugTracker.NET\\btnetsc\\SETTINGS]" + "\n");

            string url = "http://" + Request.ServerVariables["SERVER_NAME"] + Request.ServerVariables["URL"];
            url = url.Replace("generate_btnetsc_reg", "insert_bug");
            write_variable_value("Url", url);
            write_variable_value("Project", "0");
            write_variable_value("Email", security.user.email);
            write_variable_value("Username", security.user.username);


            NameValueCollection NVCSrvElements = Request.ServerVariables;
            string[] array1 = NVCSrvElements.AllKeys;

        }

        void write_variable_value(string var, string val)
        {
            Response.Write("\"" + var + "\"=\"" + val + "\"\n");
        }

    }
}
