using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DrunkenboldeServer.Packet
{
    [JsonObject(MemberSerialization.OptIn)]
    public class WaitingVoteSelectionPacket : JsonPacket
    {
        [JsonProperty]
        public int Game { get; set; }

        [JsonProperty]
        public int Count { get; set; }

        [JsonProperty]
        public int OverallCount { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override PacketType GetPacketType()
        {
            return PacketType.WaitingVoteSelection;
        }
    }
}