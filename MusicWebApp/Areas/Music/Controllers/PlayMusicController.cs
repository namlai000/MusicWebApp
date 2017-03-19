using Hangfire;
using MusicWebApp.Areas.Music.Models;
using MusicWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicWebApp.Areas.Music.Controllers
{
    public class PlayMusicController : Controller
    {
        // GET: Music/PlayMusic
        public ActionResult Index(int musicid)
        {
            MusicEntities en = new MusicEntities();
            var model = en.Musics.FirstOrDefault(a => a.Id == musicid);
            if (model != null)
            {
                BackgroundJob.Enqueue(() => Background.UpdateView(model.Id));
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
            MusicEntities en = new MusicEntities();
            var musics = en.Albums.FirstOrDefault(a => a.Id == albumId).Musics;
            var track = musics.Select(a => new Track
            {
                file = a.Link,
                thumb = a.Image,
                trackName = a.Name,
                trackArtist = a.Singer.Fullname,
                trackAlbum = "Single",
            }).ToList();
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
            MusicEntities en = new MusicEntities();
            var model = en.Comments.Where(a => a.MusicId == musicId);
            var c = model.Count();
            var data = model
                .OrderByDescending(a => a.CommentDate)
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength)
                .ToList()
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
            MusicEntities en = new MusicEntities();
            var genres = GenresEntities.InitialModels().FirstOrDefault(a => a.Id == genresId);
            var data = en.Musics
                .Where(a => a.Genre.Id == genresId)
                .OrderBy(a => Guid.NewGuid())
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
    }
}