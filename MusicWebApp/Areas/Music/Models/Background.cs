using MusicWebApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace MusicWebApp.Areas.Music.Models
{
    public class Background
    {
        public static void UpdateView(int musicId)
        {
            MusicEntities en = new MusicEntities();
            var music = en.Musics.FirstOrDefault(a => a.Id == musicId);
            if (music != null)
            {
                if (music.C_View == null) music.C_View = 0;
                music.C_View += 1;
                en.SaveChanges();
            }
        }
    }
}