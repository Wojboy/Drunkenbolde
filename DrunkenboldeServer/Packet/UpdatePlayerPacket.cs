using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrunkenboldeServer.Models;
using Newtonsoft.Json;

namespace DrunkenboldeServer.Packet
{
    [JsonObject(MemberSerialization.OptIn)]
    public class UpdatePlayerPacket : JsonPacket
    {
        [JsonProperty]
        public int PlayerId { get; set; }

        [JsonProperty]
        public int Points { get; set; }

        [JsonProperty]
        public int OverallPoints { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override PacketType GetPacketType()
        {
            return PacketType.UpdatePlayer;
        }

        public static UpdatePlayerPacket GenerateFromPlayer(Player player)
        {
            return new UpdatePlayerPacket() {PlayerId = player.Id, Points = player.Points, OverallPoints = player.OverallPoints};
        }
    }
}