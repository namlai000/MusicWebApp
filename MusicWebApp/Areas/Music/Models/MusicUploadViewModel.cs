using MusicWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicWebApp.Areas.Music.Models
{
    public class MusicUploadViewModel : MusicWebApp.Models.Music
    {
        public List<Singer> listSigner { get; set; }
        public List<Album> listAlbums { get; set; }
        public List<Genre> listGenres { get; set; }
        public HttpPostedFileBase ImageBase { get; set; }
        public HttpPostedFileBase MusicBase { get; set; }
    }
}