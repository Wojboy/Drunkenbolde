using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrunkenboldeServer.Packet
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SongGuessingAnswerPacket : JsonPacket
    {
        [JsonProperty]
        public string SongTitle { get; set; }
        [JsonProperty]
        public string SongArtist { get; set; }
        [JsonProperty]
        public string SongGame { get; set; }

        public override PacketType GetPacketType()
        {
            return PacketType.SongGuessingAnswerPacket;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}