using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DrunkenboldeServer.Packet
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LoginAnswerPacket : JsonPacket
    {
        [JsonProperty]
        public int PlayerId { get; set; }

        [JsonProperty]
        public string DisplayName { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override PacketType GetPacketType()
        {
            return PacketType.LoginPacketAnswer;
        }
    }
}