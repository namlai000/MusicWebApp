using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicWebApp.Areas.Music.Models
{
    public class Playlists
    {
        public IEnumerable<Track> playlist { get; set; }
    }

    public class Track
    {
        public string file { get; set; }
        public string thumb { get; set; }
        public string trackName { get; set; }
        public string trackArtist { get; set; }
        public string trackAlbum { get; set; }
    }
}