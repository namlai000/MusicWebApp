using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using MusicWebApp.Areas.Music.Models;
using System.Web.Script.Serialization;
using MusicWebApp.Models;

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
            MusicEntities en = new MusicEntities();
            var data = en.Musics
                .OrderByDescending(a => a.UploadDate)
                .Take(10)
                .ToList()
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
            MusicEntities en = new MusicEntities();
            var data = en.Musics
                .OrderByDescending(a => a.UploadDate)
                .Take(4)
                .ToList()
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
            MusicEntities en = new MusicEntities();
            var data = en.Singers
                .OrderByDescending(a => a.Birthday)
                .Take(12)
                .ToList()
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
            MusicEntities en = new MusicEntities();
            var data = en.Musics
                .OrderByDescending(a => a.UploadDate.Value)
                .Take(10)
                .ToList()
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
            MusicEntities en = new MusicEntities();
            var data = en.Albums
                .OrderByDescending(a => a.UploadDate)
                .Take(10)
                .ToList()
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
            MusicEntities en = new MusicEntities();
            var data = en.Comments
                .OrderByDescending(a => a.CommentDate)
                .Take(10)
                .ToList()
                .Select(a => new IConvertible[]
                {
                    a.Music.Name,
                    a.Music.Id,
                    a.User.FirstName + " " + a.User.LastName,
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