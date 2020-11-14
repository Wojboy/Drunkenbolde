using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrunkenboldeServer.Packet
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SongGuessingPacket : JsonPacket
    {
        [JsonProperty]
        public string SongLink { get; set; }
        [JsonProperty]
        public string SongTitle { get; set; }
        [JsonProperty]
        public string SongArtist { get; set; }


        public override PacketType GetPacketType()
        {
            return PacketType.SongGuessingSongPacket;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}