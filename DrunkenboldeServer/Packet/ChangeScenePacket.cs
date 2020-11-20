using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DrunkenboldeServer.Packet
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ChangeScenePacket : JsonPacket
    {
        [JsonProperty]
        public int SceneType { get; set; }

        [JsonProperty]
        public int SceneDuration { get; set; }

        [JsonProperty]
        public int GameType { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override PacketType GetPacketType()
        {
            return PacketType.ChangeScene;
        }
    }
}