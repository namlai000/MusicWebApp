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
    public class SingerController : Controller
    {
        // GET: Music/Singer
        public ActionResult Index()
        {
            var models = GenresEntities.InitialModels();
            return View(models);
        }

        public ActionResult SingersList(int genresId)
        {
            var model = GenresEntities.InitialModels().FirstOrDefault(a => a.Id == genresId);
            GenresViewModel mo = new GenresViewModel()
            {
                genre = model,
                list = GenresEntities.InitialModels(),
            };
            return View(mo);
        }

        public ActionResult SingerAbout(int singerId)
        {
            string api = "http://fmusicapi.azurewebsites.net/MusicProject/singer/" + singerId;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            Singer model = null;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                model = JsonConvert.DeserializeObject<Singer>(json);
            }
            if (model != null)
            {
                BackgroundJob.Enqueue(() => Background.UpdateView((int)EnumProject.SINGER, model.Id));
            }

            return View(model);
        }

        public ActionResult GetArtistsAlbums(JQueryDataTableParamModel param, int singerId)
        {
            string api = "http://fmusicapi.azurewebsites.net/MusicProject/album/singer/" + singerId;
            List<Album> test = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                test = JsonConvert.DeserializeObject<List<MusicWebApp.Models.Album>>(json);
            }

            var t = param.sSearch == null ? "" : param.sSearch;
            var searched = test.Where(a => a.Name.ToLower().Contains(t.ToLower()));
            var c = searched.Count();
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

        public ActionResult GetArtistsSongs(JQueryDataTableParamModel param, int singerId)
        {
            string api = "http://fmusicapi.azurewebsites.net/MusicProject/music/singer/" + singerId;
            List<MusicWebApp.Models.Music> test = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                test = JsonConvert.DeserializeObject<List<MusicWebApp.Models.Music>>(json);
            }

            var t = param.sSearch == null ? "" : param.sSearch;
            var searched = test.Where(a => a.Name.ToLower().Contains(t.ToLower()));
            var c = searched.Count();
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

        public ActionResult GetArtistsList(JQueryDataTableParamModel param, int genresId, int mode)
        {
            string api = "http://fmusicapi.azurewebsites.net/MusicProject/singer/genres/" + genresId;
            if (genresId == 0)
            {
                api = "http://fmusicapi.azurewebsites.net/MusicProject/singer";
            }
            List<Singer> test = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                test = JsonConvert.DeserializeObject<List<Singer>>(json);
            }

            var c = test.Count();
            var start = param.iDisplayStart + 1;

            var data = test.OrderByDescending(a => a.C_View);
            if (mode != 0) data = test.OrderByDescending(a => a.Id);
               
            var data2 = data
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength)
                .ToList()
                .Select(a => new IConvertible[]
                {
                    start++,
                    a.Fullname,
                    a.Id,
                    a.Image,
                    a.C_View,
                });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = c,
                iTotalDisplayRecords = c,
                aaData = data2
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetArtists(int id)
        {
            string api = "http://fmusicapi.azurewebsites.net/MusicProject/singer/genres/" + id;
            if (id == 0)
            {
                api = "http://fmusicapi.azurewebsites.net/MusicProject/singer";
            }
            List<Singer> test = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                test = JsonConvert.DeserializeObject<List<Singer>>(json);
            }

            var data = test
                .OrderByDescending(a => a.C_View)
                .Take(5)
                .ToList()
                .Select(a => new IConvertible[]
                {
                    a.Image,
                    a.Fullname,
                    a.Id,
                });

            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }
    }
}