using MusicWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicWebApp.Areas.Music.Controllers
{
    public class FavoriteController : Controller
    {
        // GET: Music/Favorite
        [Authorize]
        public ActionResult Index(int userId)
        {
            ViewBag.UserId = userId;
            return View();
        }

        public ActionResult GetFavorites(int userId)
        {
            MusicEntities en = new MusicEntities();
            var data = en.Musics
                .Where(a => a.Favorites.Where(b => b.User.Id == userId).Count() > 0)
                .ToList()
                .Select(a => new IConvertible[]
                {
                    a.Id,
                    a.Name,
                    a.Image,
                    a.Singer.Fullname,
                    a.C_View,
                });

            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult AddToFavorite(int userId, int musicId)
        {
            MusicEntities en = new MusicEntities();
            var isExist = en.Favorites.Where(a => a.User.Id == userId && a.Music.Id == musicId).FirstOrDefault();
            if (isExist != null)
            {
                return Json(new { success = false, msg = "Music existed in your Favorites!" }, JsonRequestBehavior.AllowGet);
            } else
            {
                var fav = new Favorite
                {
                    MusicId = musicId,
                    UserId = userId,
                };
                en.Favorites.Add(fav);
                en.SaveChanges();

                return Json(new { success = true, msg = "Music has been added to your Favorites!" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult DeleteFavorite(int userId, int musicId)
        {
            MusicEntities en = new MusicEntities();
            var isExist = en.Favorites.Where(a => a.User.Id == userId && a.Music.Id == musicId).FirstOrDefault();
            if (isExist != null)
            {
                en.Favorites.Remove(isExist);
                en.SaveChanges();

                return Json(new { success = true, msg = "Music has been removed from Favorites!" }, JsonRequestBehavior.AllowGet);
            } else
            {
                return Json(new { success = false, msg = "Not exist!" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}