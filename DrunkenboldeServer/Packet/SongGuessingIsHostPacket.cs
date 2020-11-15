using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrunkenboldeServer.Packet
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SongGuessingIsHostPacket : JsonPacket
    {
        [JsonProperty]
        public bool IsSongProvider { get; set; }

        public override PacketType GetPacketType()
        {
            return PacketType.SongGuessingIsHostPacket;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}