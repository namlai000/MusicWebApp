using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using MusicWebApp.Areas.Music.Models;
using System.Web.Script.Serialization;
using MusicWebApp.Models;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Text;

namespace MusicWebApp.Areas.Music.Controllers
{
    public class HomepageController : Controller
    {
        // GET: Music/Homepage
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetTop10()
        {
            string api = "http://fmusicapi.azurewebsites.net/MusicProject/music/top10";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            List<MusicWebApp.Models.Music> musics = null;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                musics = JsonConvert.DeserializeObject<List<MusicWebApp.Models.Music>>(json);
            }

            var data = musics
                .Take(10)
                .Select(a => new IConvertible[]
                {
                    a.Name,
                    a.Id,
                    a.Singer != null ? a.Singer.Fullname : "None",
                    a.Image,
                });

            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMostPopularMusic()
        {
            string api = "http://fmusicapi.azurewebsites.net/MusicProject/music/top10";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            List<MusicWebApp.Models.Music> musics = null;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                musics = JsonConvert.DeserializeObject<List<MusicWebApp.Models.Music>>(json);
            }

            var data = musics
                .Take(4)
                .Select(a => new IConvertible[]
                {
                    a.Image,
                    a.Id,
                    a.Name + " - " + a.Singer.Fullname,
                });

            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetArtistYouMayLike()
        {
            string api = "http://fmusicapi.azurewebsites.net/MusicProject/singer";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            List<Singer> singers = null;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                singers = JsonConvert.DeserializeObject<List<Singer>>(json);
            }

            var data = singers
                .OrderBy(a => Guid.NewGuid())
                .Take(10)
                .Select(a => new IConvertible[]
                {
                    a.Image,
                    a.Id,
                    a.Fullname,
                });

            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNewestTracks()
        {
            string api = "http://fmusicapi.azurewebsites.net/MusicProject/music/recent10";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            List<MusicWebApp.Models.Music> musics = null;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                musics = JsonConvert.DeserializeObject<List<MusicWebApp.Models.Music>>(json);
            }

            var data = musics
                .OrderByDescending(a => a.UploadDate)
                .Take(10)
                .Select(a => new IConvertible[]
                {
                    a.Name,
                    a.Id,
                    a.Singer != null ? a.Singer.Fullname : "None",
                    a.Image,
                });

            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNewestAlbums()
        {
            string api = "http://fmusicapi.azurewebsites.net/MusicProject/album/recent10";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            List<Album> albums = null;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                albums = JsonConvert.DeserializeObject<List<Album>>(json);
            }

            var data = albums
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

        public ActionResult GetNewestComments()
        {
            string api = "http://fmusicapi.azurewebsites.net/MusicProject/comments/recent10";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            List<Comment> comments = null;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                comments = JsonConvert.DeserializeObject<List<Comment>>(json);
            }

            var data = comments
                .Take(10)
                .Select(a => new IConvertible[]
                {
                    a.Music.Name,
                    a.Music.Id,
                    !string.IsNullOrEmpty(a.User.FirstName) && !string.IsNullOrEmpty(a.User.LastName) ? (a.User.FirstName + " " + a.User.LastName) : a.User.Logins.FirstOrDefault().Username,
                    a.User.Avatar,
                    a.Comment1,
                });

            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }
    }
}

public static class GenresEntities
{
    private static List<Genre> list = null;

    public static List<Genre> InitialModels()
    {
        if (list == null)
        {
            MusicEntities en = new MusicEntities();
            list = en.Genres.ToList();
            list.Insert(0, new Genre { Id = 0, Name = "All" });
        }

        return list;
    }
}