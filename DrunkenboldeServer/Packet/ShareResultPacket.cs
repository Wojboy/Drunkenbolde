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
        public List<ShareResultElement> Data;

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
        public string DisplayName;

        [JsonProperty]
        public int PlayerId;

        [JsonProperty]
        public int DrinkValue;

        public Dictionary<int,int> DrinkData;
    }


}