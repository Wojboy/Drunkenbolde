using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DrunkenboldeServer.Packet
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ShareResultPacket : JsonPacket
    {

        [JsonProperty]
        public List<ShareResultElement> Data { get; set; }

        public override bool IsValid()
        {
            return Data != null;
        }

        public override PacketType GetPacketType()
        {
            return PacketType.ShareResult;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class ShareResultElement
    {
        [JsonProperty]
        public string DisplayName { get; set; }

        [JsonProperty]
        public int PlayerId { get; set; }

        [JsonProperty]
        public int DrinkValue { get; set; }

        public Dictionary<int,int> DrinkData { get; set; }
    }


}