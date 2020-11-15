using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DrunkenboldeServer.Packet
{
    [JsonObject(MemberSerialization.OptIn)]
    public class WaitingGamesListPacket : JsonPacket
    {
        [JsonProperty] public List<Game> GamesList;

        public override bool IsValid()
        {
            return GamesList != null;
        }

        public override PacketType GetPacketType()
        {
            return PacketType.WaitingGamesList;
        }
    }
}