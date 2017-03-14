using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Diagnostics;

namespace MusicWebApp.Areas.Music.Models
{
    public class ProgressHub : Hub
    {
        public static void SendMessage(string data)
        {
            //IN ORDER TO INVOKE SIGNALR FUNCTIONALITY DIRECTLY FROM SERVER SIDE WE MUST USE THIS
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();

            //PUSHING DATA TO ALL CLIENTS
            hubContext.Clients.All.sendMessage(data);

            // LOGGING
            Debug.WriteLine(data);
        }
    }
}