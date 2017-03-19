﻿using MusicWebApp.Controllers;
using MusicWebApp.Models;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace MusicWebApp.Areas.Music.Models
{
    public class Background
    {
        public static HttpSessionState Session;

        public static void UpdateView(int mode, int id)
        {
            MusicEntities en = new MusicEntities();

            if (mode == (int)EnumProject.MUSIC)
            {
                var music = en.Musics.FirstOrDefault(a => a.Id == id);
                if (music != null)
                {
                    var ses = "MusicView_Id-" + id;
                    var view = Session[ses] as View;
                    if (view == null || view.hasViewed == false)
                    {
                        if (music.C_View == null) music.C_View = 0;
                        music.C_View += 1;
                        en.SaveChanges();
                        Session[ses] = new View { hasViewed = true };
                    }
                }
            }
            else if (mode == (int)EnumProject.ALBUM)
            {
                var album = en.Albums.FirstOrDefault(a => a.Id == id);
                if (album != null)
                {
                    var ses = "AlbumView_Id-" + id;
                    var view = Session[ses] as View;
                    if (view == null || view.hasViewed == false)
                    {
                        if (album.C_View == null) album.C_View = 0;
                        album.C_View += 1;
                        en.SaveChanges();
                        Session[ses] = new View { hasViewed = true };
                    }
                }
            }
            else if (mode == (int)EnumProject.SINGER)
            {
                var singer = en.Singers.FirstOrDefault(a => a.Id == id);
                if (singer != null)
                {
                    var ses = "SingerView_Id-" + id;
                    var view = Session[ses] as View;
                    if (view == null || view.hasViewed == false)
                    {
                        if (singer.C_View == null) singer.C_View = 0;
                        singer.C_View += 1;
                        en.SaveChanges();
                        Session[ses] = new View { hasViewed = true };
                    }
                }
            }
        }
    }
}