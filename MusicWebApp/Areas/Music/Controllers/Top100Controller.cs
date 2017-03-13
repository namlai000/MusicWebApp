using MusicWebApp.Areas.Music.Models;
using MusicWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicWebApp.Areas.Music.Controllers
{
    public class Top100Controller : Controller
    {
        // GET: Music/Top100
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

        public ActionResult GetTop100(int genresId)
        {
            MusicEntities en = new MusicEntities();

            var gen = GenresEntities.InitialModels().FirstOrDefault(a => a.Id == genresId);
            var test = en.Musics.Where(a => true);
            if (gen.Id != 0)
            {
                test = test.Where(a => a.Genre.Id == gen.Id);
            }

            var start = 1;

            var data = test
                .OrderByDescending(a => a.UploadDate)
                .Take(100)
                .ToList()
                .Select(a => new IConvertible[]
                {
                    start++,
                    a.Name,
                    a.Id,
                    a.Singer.Fullname,
                    a.Image,
                });

            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }
    }
}