using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrunkenboldeServer.GameLogic.SongGuessing
{
    public static class SongGuesserHelper
    {
        public static string ProcessLink(string link)
        {
            //var result = Uri.TryCreate(link, UriKind.Absolute, out Uri uriResult);

            var video_id = link.Split('=');

            var idAndRest = video_id[1];
            var idSplitted = idAndRest.Split('&');
            var id = idSplitted[0];
                       
            
            return id;
        }

    }
}