using Firebase.Storage;
using MusicWebApp.Areas.Music.Models;
using MusicWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MusicWebApp.Areas.Music.Controllers
{
    public class UploadController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var music = new MusicUploadViewModel();
            MusicEntities en = new MusicEntities();
            music.listAlbums = en.Albums.ToList();
            music.listGenres = en.Genres.ToList();
            music.listSigner = en.Singers.ToList();

            return View(music);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Upload(MusicUploadViewModel model)
        {
            string storageUrl = "musicproject-9f3c5.appspot.com";
            string message = "Save failed";

            string date = DateTime.Now.ToString("yyyyMMddHHmmssffff");

            try
            {
                var stream1 = model.ImageBase.InputStream;
                var task1 = new FirebaseStorage(storageUrl)
                    .Child("MusicProject")
                    .Child("Images")
                    .Child(date + model.ImageBase.FileName)
                    .PutAsync(stream1);
                string imageUrl = await task1;

                var stream2 = model.MusicBase.InputStream;
                var task2 = new FirebaseStorage(storageUrl)
                    .Child("MusicProject")
                    .Child("Musics")
                    .Child(date + model.MusicBase.FileName)
                    .PutAsync(stream2);
                task2.Progress.ProgressChanged += (s, e) => ProgressHub.SendMessage(e.Percentage);
                string musicUrl = await task2;

                MusicEntities en = new MusicEntities();

                var name = System.Web.HttpContext.Current.User.Identity.Name;
                var login = en.Logins.FirstOrDefault(a => a.Username.Equals(name));
                User userLogin = null;
                if (login != null)
                {
                    userLogin = login.User;
                }

                var music = new MusicWebApp.Models.Music
                {
                    Name = model.Name,
                    AlbumId = model.AlbumId,
                    Image = imageUrl,
                    Link = musicUrl,
                    SingerId = model.SingerId,
                    UserId = userLogin.Id,
                    GenresId = model.GenresId,
                    Pending = false,
                    UploadDate = DateTime.Now,
                };

                en.Musics.Add(music);
                int updateRow = await en.SaveChangesAsync();
                if (updateRow > 0)
                {
                    message = "Save successful";
                }
            }
            catch (Exception ex)
            {
                return Json(new { succress = false, message = ex.Message });
            }

            return Json(new { success = true, message = message });
        }
    }
}