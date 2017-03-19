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
    public class AlbumController : Controller
    {
        // GET: Music/Album
        public ActionResult Index(int genresId)
        {
            var model = GenresEntities.InitialModels().FirstOrDefault(a => a.Id == genresId);
            if (model == null) model = GenresEntities.InitialModels().First();
            GenresViewModel mo = new GenresViewModel()
            {
                genre = model,
                list = GenresEntities.InitialModels(),
            };
            return View(mo);
        }

        public ActionResult Album(int albumId)
        {
            MusicEntities en = new MusicEntities();
            var album = en.Albums.FirstOrDefault(a => a.Id == albumId);
            if (album != null)
            {
                BackgroundJob.Enqueue(() => Background.UpdateView((int)EnumProject.ALBUM, album.Id));
            }
            return View(album);
        }

        public ActionResult GetTopAlbumsByGenres(JQueryDataTableParamModel param, int genresId)
        {
            MusicEntities en = new MusicEntities();
            List<Album> test = null;

            string api = "http://fmusicapi.azurewebsites.net/MusicProject/album/genres/" + genresId;
            if (genresId == 0)
            {
                test = en.Albums.Where(a => true).ToList();
            }
            else
            {
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
                //WebResponse response = request.GetResponse();
                //using (Stream responseStream = response.GetResponseStream())
                //{
                //    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                //    var json = reader.ReadToEnd();
                //    test = JsonConvert.DeserializeObject<List<Album>>(json);
                //}
                test = en.Albums.Where(a => a.Genre.Id == genresId).ToList();
            }

            var c = test.Count();
            var start = param.iDisplayStart + 1;
            var data = test
                .OrderBy(a => a.UploadDate)
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength)
                .Select(a => new IConvertible[]
                {
                    start++,
                    a.Name,
                    a.Id,
                    a.Singer.Fullname,
                    a.Image,
                });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = c,
                iTotalDisplayRecords = c,
                aaData = data
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRecentAlbumsByGenres(JQueryDataTableParamModel param, int genresId)
        {
            MusicEntities en = new MusicEntities();

            var gen = GenresEntities.InitialModels().FirstOrDefault(a => a.Id == genresId);
            var test = en.Albums.Where(a => true);
            if (gen.Id != 0)
            {
                test = test.Where(a => a.Genre.Id == gen.Id);
            }

            var c = test.Count();
            var start = param.iDisplayStart + 1;
            var data = test
                .OrderByDescending(a => a.UploadDate)
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength)
                .ToList()
                .Select(a => new IConvertible[]
                {
                    start++,
                    a.Name,
                    a.Id,
                    a.Singer.Fullname,
                    a.Image,
                });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = c,
                iTotalDisplayRecords = c,
                aaData = data
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetsSongsByAlbum(JQueryDataTableParamModel param, int albumId)
        {
            MusicEntities en = new MusicEntities();
            var test = en.Albums.FirstOrDefault(a => a.Id == albumId);

            var musics = test.Musics;

            var t = param.sSearch == null ? "" : param.sSearch;
            var searched = musics.AsEnumerable().Where(a => a.Name.ToLower().Contains(t.ToLower()));

            var c = musics.Count();
            var start = param.iDisplayStart + 1;
            var data = searched
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength)
                .Select(a => new IConvertible[]
                {
                    start++,
                    a.Name,
                    a.Id,
                    a.Singer.Fullname,
                    a.Image,
                });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = c,
                iTotalDisplayRecords = c,
                aaData = data
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSameAlbums(int genresId)
        {
            MusicEntities en = new MusicEntities();
            var genres = GenresEntities.InitialModels().FirstOrDefault(a => a.Id == genresId);
            var data = en.Albums
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