using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace MusicWebApp.Areas.Music.Models
{
    public class ProgressHub : Hub
    {
        public static void SendMessage(int progress)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();
            hubContext.Clients.All.sendMessage(progress);
        }

        public void GetCountAndMessage()
        {
            Clients.Caller.sendMessage(1);
        }
    }
}