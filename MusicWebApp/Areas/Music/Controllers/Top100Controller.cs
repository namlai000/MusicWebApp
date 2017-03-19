using MusicWebApp.Areas.Music.Models;
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
            string api = "http://fmusicapi.azurewebsites.net/MusicProject/music/top100";
            List<MusicWebApp.Models.Music> test = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                test = JsonConvert.DeserializeObject<List<MusicWebApp.Models.Music>>(json);
            }

            var start = 1;

            var data = test
                .Take(100)
                .Select(a => new IConvertible[]
                {
                    start++,
                    a.Name,
                    a.Id,
                    a.Singer.Fullname,
                    a.Image,
                    a.C_View,
                });

            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }
    }
}