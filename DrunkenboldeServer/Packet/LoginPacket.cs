using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DrunkenboldeServer.Packet
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LoginPacket : JsonPacket
    {
        [JsonProperty]
        public string Room { get; set; }

        [JsonProperty]
        public string DisplayName { get; set; }


        public override bool IsValid()
        {
            return !(string.IsNullOrEmpty(Room) || string.IsNullOrEmpty(DisplayName) || DisplayName.Length > 30);
        }

        public override PacketType GetPacketType()
        {
            return PacketType.LoginPacket;
        }
    }
}