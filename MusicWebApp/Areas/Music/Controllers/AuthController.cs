using Firebase.Storage;
using MusicWebApp.Areas.Music.Models;
using MusicWebApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

        public ActionResult Signup()
        {
            var user = new RegisterUserViewModel();
            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> Signup(RegisterUserViewModel model)
        {
            MusicEntities en = new MusicEntities();
            var login = en.Logins.FirstOrDefault(a => a.Username.Equals(model.Username));
            if (login != null)
            {
                return Json(new { success = false, message = "Username exist, Please choose another username!" });
            }
            else
            {
                try
                {
                    string storageUrl = "musicproject-9f3c5.appspot.com";
                    string date = DateTime.Now.ToString("yyyyMMddHHmmssffff");

                    var stream2 = model.ImageBase.InputStream;
                    var task2 = new FirebaseStorage(storageUrl)
                        .Child("MusicProject")
                        .Child("Images")
                        .Child(date + "-" + model.ImageBase.FileName)
                        .PutAsync(stream2);
                    task2.Progress.ProgressChanged += (s, e) => ProgressHub.SendMessage("Uploading User Avatar ... (" + Math.Round((e.Position * 1.0 / e.Length * 100), 0) + "%)");
                    string imageUrl = await task2;

                    Login newUser = new Login
                    {
                        Username = model.Username,
                        Password = model.Password,
                        User = new User
                        {
                            Avatar = imageUrl,
                            FirstName = model.Firstname,
                            LastName = model.Lastname,
                            Phone = model.Phone,
                            Gender = model.Gender,
                            Email = model.Email,
                        }
                    };
                    en.Logins.Add(newUser);
                    en.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }

            }

            return Json(new { success = true });
        }
    }
}