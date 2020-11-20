using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DrunkenboldeServer.Packet
{
    [JsonObject(MemberSerialization.OptIn)]
    public class WaitingVotePacket : JsonPacket
    {
        [JsonProperty] public int GameId { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override PacketType GetPacketType()
        {
            return PacketType.WaitingVote;
        }
    }
}