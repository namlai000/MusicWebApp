using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MusicWebApp.Areas.Music.Controllers
{
    public class AuthController : Controller
    {
        [Authorize]
        public ActionResult Login()
        {
            return RedirectToAction("Index", "Homepage");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Homepage");
        }
    }
}