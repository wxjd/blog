using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace NiitBlog.Common
{
    public class FormAuthService
    {
        public static void SignIn(string userName, bool createPersistentCookie, IEnumerable<string> roles)
        {
            var str = string.Join(",", roles);
            var authTicket = new FormsAuthenticationTicket(
                1,
                userName,  //user id
                DateTime.Now,
                DateTime.Now.AddDays(30),  // expiry
                createPersistentCookie,
                str,
                "/");
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
         
            if (authTicket.IsPersistent){
                cookie.Expires = authTicket.Expiration;
            }
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}