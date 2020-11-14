using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DrunkenboldeServer.Packet
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class JsonPacket
    {
        public abstract bool IsValid();
        public abstract PacketType GetPacketType();
    }
}