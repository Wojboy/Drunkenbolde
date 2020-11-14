using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DrunkenboldeServer.Packet
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MessagePacket : JsonPacket
    {
        [JsonProperty]
        public string Message;

        public override bool IsValid()
        {
            return string.IsNullOrEmpty(Message);
        }

        public override PacketType GetPacketType()
        {
            return PacketType.Message;
        }
    }
}