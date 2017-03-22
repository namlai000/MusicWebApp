using Hangfire;
using MusicWebApp.Areas.Music.Models;
using MusicWebApp.Controllers;
using MusicWebApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
            string api = ConfigurationManager.AppSettings["ApiServer"] +  "/MusicProject/album/" + albumId;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            Album album = null;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                album = JsonConvert.DeserializeObject<Album>(json);
            }
            if (album != null)
            {
                BackgroundJob.Enqueue(() => Background.UpdateView((int)EnumProject.ALBUM, album.Id));
            }
            return View(album);
        }

        public ActionResult GetTopAlbumsByGenres(JQueryDataTableParamModel param, int genresId)
        {
            List<Album> test = null;

            string api = ConfigurationManager.AppSettings["ApiServer"] + "/MusicProject/album/genres/" + genresId;
            if (genresId == 0)
            {
                api = ConfigurationManager.AppSettings["ApiServer"] + "/MusicProject/album";
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                test = JsonConvert.DeserializeObject<List<Album>>(json);
            }

            var c = test.Count();
            var start = param.iDisplayStart + 1;
            var data = test
                .OrderByDescending(a => a.C_View)
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength)
                .Select(a => new IConvertible[]
                {
                    start++,
                    a.Name,
                    a.Id,
                    a.Singer.Fullname,
                    a.Image,
                    a.C_View,
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
            List<Album> test = null;

            string api = ConfigurationManager.AppSettings["ApiServer"] + "/MusicProject/album/genres/" + genresId;
            if (genresId == 0)
            {
                api = ConfigurationManager.AppSettings["ApiServer"] + "/MusicProject/album";
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                test = JsonConvert.DeserializeObject<List<Album>>(json);
            }

            var c = test.Count();
            var start = param.iDisplayStart + 1;
            var data = test
                .OrderByDescending(a => a.UploadDate)
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength)
                .Select(a => new IConvertible[]
                {
                    start++,
                    a.Name,
                    a.Id,
                    a.Singer.Fullname,
                    a.Image,
                    a.C_View,
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
            List<MusicWebApp.Models.Music> musics = null;

            string api = ConfigurationManager.AppSettings["ApiServer"] + "/MusicProject/music/albums/" + albumId;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                musics = JsonConvert.DeserializeObject<List<MusicWebApp.Models.Music>>(json);
            }

            var t = param.sSearch == null ? "" : param.sSearch;
            var searched = musics.Where(a => a.Name.ToLower().Contains(t.ToLower()));

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
                    a.C_View,
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
            string api = ConfigurationManager.AppSettings["ApiServer"] + "/MusicProject/album/genres/" + genresId;
            List<Album> test = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                test = JsonConvert.DeserializeObject<List<Album>>(json);
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