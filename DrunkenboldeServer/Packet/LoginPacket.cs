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
        public string Room;

        [JsonProperty]
        public string PlayerName;

        [JsonProperty]
        public string Avatar;

        public override bool IsValid()
        {
            return string.IsNullOrEmpty(Room) || string.IsNullOrEmpty(PlayerName);
        }

        public override PacketType GetPacketType()
        {
            return PacketType.LoginPacket;
        }
    }
}