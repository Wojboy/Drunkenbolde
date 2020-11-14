using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DrunkenboldeServer.Packet
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ShareSetPacket : JsonPacket
    {
        [JsonProperty]
        public int PlayerId;

        [JsonProperty]
        public int Amount;

        public override bool IsValid()
        {
            return PlayerId >= 0 && Amount >= 0;
        }

        public override PacketType GetPacketType()
        {
            return PacketType.ShareSet;
        }
    }
}