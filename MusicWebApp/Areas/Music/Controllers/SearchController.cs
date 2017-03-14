using MusicWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            MusicEntities en = new MusicEntities();

            var start = param.iDisplayStart + 1;
            var test = en.Musics
                .OrderByDescending(a => a.Id)
                .Where(a => a.Name.Contains(search));

            var c = test.Count();
            var data = test
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

        public ActionResult SeachSingers(JQueryDataTableParamModel param, string search)
        {
            MusicEntities en = new MusicEntities();

            var start = param.iDisplayStart + 1;
            var test = en.Singers
                .OrderByDescending(a => a.Id)
                .Where(a => a.Fullname.Contains(search));

            var c = test.Count();
            var data = test
                        .Skip(param.iDisplayStart)
                        .Take(param.iDisplayLength)
                        .ToList()
                        .Select(a => new IConvertible[]
                        {
                            start++,
                            a.Fullname,
                            a.Id,
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

        public ActionResult SearchAlbums(JQueryDataTableParamModel param, string search)
        {
            MusicEntities en = new MusicEntities();

            var start = param.iDisplayStart + 1;
            var test = en.Albums
                .OrderByDescending(a => a.Id)
                .Where(a => a.Name.Contains(search));

            var c = test.Count();
            var data = test
                        .Skip(param.iDisplayStart)
                        .Take(param.iDisplayLength)
                        .ToList()
                        .Select(a => new IConvertible[]
                        {
                            start++,
                            a.Name,
                            a.Id,
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
    }
}