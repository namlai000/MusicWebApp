using MusicWebApp.Areas.Music.Models;
using MusicWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            MusicEntities en = new MusicEntities();
            var model = en.Singers.FirstOrDefault(a => a.Id == singerId);

            return View(model);
        }

        public ActionResult GetArtistsAlbums(JQueryDataTableParamModel param, int singerId)
        {
            MusicEntities en = new MusicEntities();
            var test = en.Albums.Where(a => a.Singer.Id == singerId);

            var t = param.sSearch == null ? "" : param.sSearch;
            var searched = test.AsEnumerable().Where(a => a.Name.ToLower().Contains(t.ToLower()));
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
            MusicEntities en = new MusicEntities();
            var test = en.Musics.Where(a => a.Singer.Id == singerId);

            var t = param.sSearch == null ? "" : param.sSearch;
            var searched = test.AsEnumerable().Where(a => a.Name.ToLower().Contains(t.ToLower()));
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
                });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = c,
                iTotalDisplayRecords = c,
                aaData = data
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetArtistsList(JQueryDataTableParamModel param, int genresId)
        {
            MusicEntities en = new MusicEntities();
            var gen = GenresEntities.InitialModels().FirstOrDefault(a => a.Id == genresId);
            var test = en.Singers.Where(a => true);
            if (gen.Id != 0)
            {
                test = en.Singers.Where(a => a.Genre.Id == genresId);
            }

            var c = test.Count();
            var start = param.iDisplayStart + 1;
            var data = test
                .OrderBy(a => a.Id)
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

        public ActionResult GetArtists(int id)
        {
            MusicEntities en = new MusicEntities();
            var gen = GenresEntities.InitialModels().FirstOrDefault(a => a.Id == id);
            var test = en.Singers.Where(a => true);
            if (gen.Id != 0)
            {
                test = en.Singers.Where(a => a.Genre.Id == id);
            }

            var data = test
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