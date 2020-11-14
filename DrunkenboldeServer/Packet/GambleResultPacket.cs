using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrunkenboldeServer.Scene;
using Newtonsoft.Json;

namespace DrunkenboldeServer.Packet
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GambleResultPacket : JsonPacket
    {
        [JsonProperty]
        public bool Black { get; set; }

        [JsonProperty]
        public List<GambleResultItem> States { get; set; }

        public override bool IsValid()
        {
            return States != null;
        }

        public override PacketType GetPacketType()
        {
            return PacketType.GambleResult;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class GambleSetPacket : JsonPacket
    {
        [JsonProperty]
        public int AmountBlack { get; set; }

        [JsonProperty]
        public int AmountRed { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override PacketType GetPacketType()
        {
            return PacketType.GambleSet;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class GambleResultItem
    {
        [JsonProperty]
        public int PlayerId { get; set; }

        [JsonProperty]
        public string PlayerName { get; set; }

        [JsonProperty]
        public int AmountRed { get; set; }
        [JsonProperty]
        public int AmountBlack { get; set; }
    }
}