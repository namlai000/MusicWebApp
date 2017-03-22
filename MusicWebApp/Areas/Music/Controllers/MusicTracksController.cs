using MusicWebApp.Areas.Music.Models;
using MusicWebApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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

            List<MusicWebApp.Models.Music> test = null;

            string api = ConfigurationManager.AppSettings["ApiServer"] + "/MusicProject/music/genres/" + genresId;
            if (genresId == 0)
            {
                api = ConfigurationManager.AppSettings["ApiServer"] + "/MusicProject/music";
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                test = JsonConvert.DeserializeObject<List<MusicWebApp.Models.Music>>(json);
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

        public ActionResult GetGenresMostPopularSongs(JQueryDataTableParamModel param, int genresId)
        {
            List<MusicWebApp.Models.Music> test = null;

            string api = ConfigurationManager.AppSettings["ApiServer"] + "/MusicProject/music/genres/" + genresId;
            if (genresId == 0)
            {
                api = ConfigurationManager.AppSettings["ApiServer"] + "/MusicProject/music";
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                test = JsonConvert.DeserializeObject<List<MusicWebApp.Models.Music>>(json);
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
                    a.Singer == null ? "" : a.Singer.Fullname,
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

        public ActionResult GetRandomSongs(int genresId)
        {
            List<MusicWebApp.Models.Music> test = null;

            string api = ConfigurationManager.AppSettings["ApiServer"] + "/MusicProject/music/genres/" + genresId;
            if (genresId == 0)
            {
                api = ConfigurationManager.AppSettings["ApiServer"] + "/MusicProject/music";
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                test = JsonConvert.DeserializeObject<List<MusicWebApp.Models.Music>>(json);
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