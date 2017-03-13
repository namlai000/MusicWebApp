using MusicWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicWebApp.Areas.Music.Models
{
    public class GenresViewModel
    {
        public Genre genre { get; set; }
        public List<Genre> list { get; set; }
    }
}