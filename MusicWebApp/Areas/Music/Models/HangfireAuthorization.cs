using Hangfire.Dashboard;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicWebApp.Areas.Music.Models
{
    public class HangfireAuthorization : IAuthorizationFilter
    {
        public bool Authorize(IDictionary<string, object> owinEnvironment)
        {
            // In case you need an OWIN context, use the next line,
            // `OwinContext` class is the part of the `Microsoft.Owin` package.
            var context = new OwinContext(owinEnvironment);

            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            //return context.Authentication.User.Identity.IsAuthenticated;
            MusicWebApp.Models.MusicEntities en = new MusicWebApp.Models.MusicEntities();
            MusicWebApp.Models.Login login = en.Logins.FirstOrDefault(a => a.Username.Equals(HttpContext.Current.User.Identity.Name));
            if (login != null)
            {
                return true;
            } else
            {
                return false;
            }
            
        }
    }
}