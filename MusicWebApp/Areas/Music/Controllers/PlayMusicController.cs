using Hangfire;
using MusicWebApp.Areas.Music.Models;
using MusicWebApp.Controllers;
using MusicWebApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace MusicWebApp.Areas.Music.Controllers
{
    public class PlayMusicController : Controller
    {
        // GET: Music/PlayMusic
        public ActionResult Index(int musicid)
        {
            string api = ConfigurationManager.AppSettings["ApiServer"] + "/MusicProject/music/" + musicid;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            MusicWebApp.Models.Music model = null;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                model = JsonConvert.DeserializeObject<MusicWebApp.Models.Music>(json);
            }
            if (model != null)
            {
                BackgroundJob.Enqueue(() => Background.UpdateView((int)EnumProject.MUSIC, model.Id));
            }

            return View(model);
        }

        [Authorize]
        public ActionResult Favorite(int userId)
        {
            MusicEntities en = new MusicEntities();
            var musics = en.Musics.Where(a => a.Favorites.Where(b => b.User.Id == userId).Count() > 0).Count();
            if (musics == 0) return RedirectToAction("Index", "Favorite", new { userId = userId, error = "Your favorites is empty!" });

            ViewBag.UserId = userId;
            return View();
        }

        [Authorize]
        public ActionResult LoadFavoriteSong(int userId)
        {
            MusicEntities en = new MusicEntities();
            var musics = en.Musics
                .Where(a => a.Favorites.Where(b => b.User.Id == userId).Count() > 0)
                .ToList()
                .Select(a => new Track
                {
                    file = a.Link,
                    thumb = a.Image,
                    trackName = a.Name,
                    trackArtist = a.Singer.Fullname,
                    trackAlbum = "Single",
                });

            Playlists list = new Playlists
            {
                playlist = musics,
            };

            return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NextSong(int genresId, int musicId)
        {
            MusicEntities en = new MusicEntities();
            var music = en.Musics
                .Where(a => a.Id != musicId && a.Genre.Id == genresId)
                .OrderBy(a => Guid.NewGuid())
                .ToList()
                .Select(a => new IConvertible[] 
                {
                    a.Image,
                    a.Name,
                    a.Singer.Fullname,
                    a.Id,
                })
                .FirstOrDefault();

            return Json(music, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Album(int albumId)
        {
            MusicEntities en = new MusicEntities();
            var model = en.Albums.FirstOrDefault(a => a.Id == albumId);
            return View(model);
        }

        public ActionResult LoadAlbumSong(int albumId)
        {
            string api = ConfigurationManager.AppSettings["ApiServer"] + "/MusicProject/music/albums/" + albumId;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            List<MusicWebApp.Models.Music> model = null;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                model = JsonConvert.DeserializeObject<List<MusicWebApp.Models.Music>>(json);
            }

            var track = model.Select(a => new Track
            {
                file = a.Link,
                thumb = a.Image,
                trackName = a.Name,
                trackArtist = a.Singer.Fullname,
                trackAlbum = "Single",
            });

            Playlists list = new Playlists
            {
                playlist = track,
            };

            return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult PostComment(int musicId, int userId, string comment)
        {
            MusicEntities en = new MusicEntities();

            var name = System.Web.HttpContext.Current.User.Identity.Name;
            var login = en.Logins.FirstOrDefault(a => a.Username.Equals(name));
            User userLogin = null;
            if (login != null)
            {
                userLogin = login.User;
            }

            Comment com = new Comment
            {
                CommentDate = DateTime.Now,
                MusicId = musicId,
                UserId = userLogin.Id,
                Comment1 = comment,
            };
            en.Comments.Add(com);
            en.SaveChanges();

            return Json(new { success = true });
        }

        public ActionResult GetComments(JQueryDataTableParamModel param, int musicId)
        {
            string api = ConfigurationManager.AppSettings["ApiServer"] + "/MusicProject/comments/music/" + musicId;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            List<Comment> comments = null;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                comments = JsonConvert.DeserializeObject<List<Comment>>(json);
            }

            var c = comments.Count();
            var data = comments
                .OrderByDescending(a => a.CommentDate)
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength)
                .Select(a => new IConvertible[]
                {
                    a.User.FirstName + " " + a.User.LastName,
                    a.Comment1,
                    a.User.Avatar,
                });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = c,
                iTotalDisplayRecords = c,
                aaData = data
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSameSongs(int genresId)
        {
            List<MusicWebApp.Models.Music> test = null;

            string api = ConfigurationManager.AppSettings["ApiServer"] + "/MusicProject/music/genres/" + genresId;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                test = JsonConvert.DeserializeObject<List<MusicWebApp.Models.Music>>(json);
            }

            var data = test
                .OrderBy(a => Guid.NewGuid())
                .Take(10)
                .Select(a => new IConvertible[]
                {
                    a.Name,
                    a.Id,
                    a.Singer.Fullname,
                    a.Image,
                });

            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult Download(int id)
        {
            string api = ConfigurationManager.AppSettings["ApiServer"] + "/MusicProject/music/" + id;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            MusicWebApp.Models.Music model = null;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                model = JsonConvert.DeserializeObject<MusicWebApp.Models.Music>(json);
            }

            return Redirect(model.Link);
        }
    }
}