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

namespace MusicWebApp.Areas.Music.Controllers
{
    public class PlayMusicController : Controller
    {
        // GET: Music/PlayMusic
        public ActionResult Index(int musicid)
        {
            string api = "http://fmusicapi.azurewebsites.net/MusicProject/music/" + musicid;
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

        public ActionResult Album(int albumId)
        {
            MusicEntities en = new MusicEntities();
            var model = en.Albums.FirstOrDefault(a => a.Id == albumId);
            return View(model);
        }

        public ActionResult LoadAlbumSong(int albumId)
        {
            string api = "http://fmusicapi.azurewebsites.net/MusicProject//music/albums/" + albumId;
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
            string api = "http://fmusicapi.azurewebsites.net/MusicProject/comments/music/" + musicId;
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

            string api = "http://fmusicapi.azurewebsites.net/MusicProject/music/genres/" + genresId;
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
    }
}