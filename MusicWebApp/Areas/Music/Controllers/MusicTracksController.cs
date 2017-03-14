﻿using MusicWebApp.Areas.Music.Models;
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
    public class MusicTracksController : Controller
    {
        // GET: Music/MusicTracks
        public ActionResult Index(int id)
        {
            var gen = GenresEntities.InitialModels().FirstOrDefault(a => a.Id == id);
            if (gen == null) gen = GenresEntities.InitialModels().First();
            GenresViewModel models = new GenresViewModel()
            {
                genre = gen,
                list = GenresEntities.InitialModels(),
            };

            return View(models);
        }

        public ActionResult GetGenresMostRecentSongs(JQueryDataTableParamModel param, int genresId)
        {
            MusicEntities en = new MusicEntities();

            var gen = GenresEntities.InitialModels().FirstOrDefault(a => a.Id == genresId);
            var test = en.Musics.Where(a => true);
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

        public ActionResult GetGenresMostPopularSongs(JQueryDataTableParamModel param, int genresId)
        {
            MusicEntities en = new MusicEntities();

            var gen = GenresEntities.InitialModels().FirstOrDefault(a => a.Id == genresId);
            var test = en.Musics.Where(a => true);
            if (gen.Id != 0)
            {
                test = test.Where(a => a.Genre.Id == gen.Id);
            }

            //string api = "http://fmusicapi.azurewebsites.net/MusicProject/music/genres/" + gen.Id;
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            //WebResponse response = request.GetResponse();
            //List<MusicWebApp.Models.Music> test = null;
            //using (Stream responseStream = response.GetResponseStream())
            //{
            //    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
            //    var json = reader.ReadToEnd();
            //    test = JsonMapper.ToObject<List<MusicWebApp.Models.Music>>(json);
            //}

            var c = test.Count();
            var start = param.iDisplayStart + 1;
            var data = test
                .OrderBy(a => a.UploadDate)
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength)
                .ToList()
                .Select(a => new IConvertible[]
                {
                    start++,
                    a.Name,
                    a.Id,
                    a.Singer == null ? "" : a.Singer.Fullname,
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

        public ActionResult GetRandomSongs(int genresId)
        {
            MusicEntities en = new MusicEntities();

            var gen = GenresEntities.InitialModels().FirstOrDefault(a => a.Id == genresId);
            var test = en.Musics.Where(a => true);
            if (gen.Id != 0)
            {
                test = test.Where(a => a.Genre.Id == gen.Id);
            }

            var data = test
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

            return Json( new{ data = data }, JsonRequestBehavior.AllowGet);
        }
    }

    public class JQueryDataTableParamModel
    {
        /// <summary>
        /// Request sequence number sent by DataTable,
        /// same value must be returned in response
        /// </summary>       
        public string sEcho { get; set; }

        /// <summary>
        /// Text used for filtering
        /// </summary>
        public string sSearch { get; set; }

        /// <summary>
        /// Number of records that should be shown in table
        /// </summary>
        public int iDisplayLength { get; set; }

        /// <summary>
        /// First record that should be shown(used for paging)
        /// </summary>
        public int iDisplayStart { get; set; }

        /// <summary>
        /// Number of columns in table
        /// </summary>
        public int iColumns { get; set; }

        /// <summary>
        /// Number of columns that are used in sorting
        /// </summary>
        public int iSortingCols { get; set; }

        /// <summary>
        /// Comma separated list of column names
        /// </summary>
        public string sColumns { get; set; }

        /// <summary>
        /// Sort column
        /// </summary>
        public int iSortCol_0 { get; set; }

        /// <summary>
        /// Asc or Desc
        /// </summary>
        public string sSortDir_0 { get; set; }
    }
}