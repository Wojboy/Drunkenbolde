using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrunkenboldeServer.Packet;

namespace DrunkenboldeServer.GameLogic.SongGuessing
{
    public static class SongGuesserHelper
    {
        internal static string ProcessLink(string link)
        {
            //var result = Uri.TryCreate(link, UriKind.Absolute, out Uri uriResult);

            var video_id = link.Split('=');

            var idAndRest = video_id[1];
            var idSplitted = idAndRest.Split('&');
            var id = idSplitted[0];
                       
            
            return id;
        }

        internal static bool CheckIfAnswerRight(SongGuessingAnswerPacket songAnswerPacket, Song currentSong)
        {
            if (songAnswerPacket.SongArtist == currentSong.Artist || songAnswerPacket.SongTitle == currentSong.Title)
            {
                return true;
            }
            return false;
        }
    }
}