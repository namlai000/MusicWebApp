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
    public class SearchController : Controller
    {
        // GET: Music/Search
        public ActionResult Index(string searchText)
        {
            ViewBag.Search = searchText;
            return View();
        }

        public ActionResult SearchSongs(JQueryDataTableParamModel param, string search)
        {
            string api = "http://fmusicapi.azurewebsites.net/MusicProject/music/name/" + search;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            List<MusicWebApp.Models.Music> musics = null;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                musics = JsonConvert.DeserializeObject<List<MusicWebApp.Models.Music>>(json);
            }

            var c = musics.Count();
            var start = param.iDisplayStart + 1;
            var data = musics
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

        public ActionResult SeachSingers(JQueryDataTableParamModel param, string search)
        {
            string api = "http://fmusicapi.azurewebsites.net/MusicProject/singer/name/" + search;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            List<Singer> singers = null;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                singers = JsonConvert.DeserializeObject<List<Singer>>(json);
            }

            var c = singers.Count();
            var start = param.iDisplayStart + 1;
            var data = singers
                        .Skip(param.iDisplayStart)
                        .Take(param.iDisplayLength)
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
                aaData = data
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchAlbums(JQueryDataTableParamModel param, string search)
        {
            string api = "http://fmusicapi.azurewebsites.net/MusicProject/album/name/" + search;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            List<Album> albums = null;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                albums = JsonConvert.DeserializeObject<List<Album>>(json);
            }

            var c = albums.Count();
            var start = param.iDisplayStart + 1;
            var data = albums
                        .Skip(param.iDisplayStart)
                        .Take(param.iDisplayLength)
                        .Select(a => new IConvertible[]
                        {
                            start++,
                            a.Name,
                            a.Id,
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
    }
}