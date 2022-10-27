using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace BugTracker.Web.Services
{
    public class EncryptionService
    {
        public static string HashString(string password, string salt)
        {
            Rfc2898DeriveBytes k2 = new Rfc2898DeriveBytes(password, System.Text.Encoding.UTF8.GetBytes(salt));
            var result = System.Text.Encoding.UTF8.GetString(k2.GetBytes(128));
            return result;
        }
    }
}