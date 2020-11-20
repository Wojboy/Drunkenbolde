using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrunkenboldeServer.Models;
using Newtonsoft.Json;

namespace DrunkenboldeServer.Packet
{
    [JsonObject(MemberSerialization.OptIn)]
    public class UpdatePlayerListPacket : JsonPacket
    {
        [JsonProperty] public List<PlayerListPlayer> Players { get; set; }

        public override bool IsValid()
        {
            return Players != null && Players.Count > 0;
        }

        public override PacketType GetPacketType()
        {
            return PacketType.UpdatePlayerList;
        }

        public static UpdatePlayerListPacket GenerateFromPlayerList(List<Player> players)
        {
            UpdatePlayerListPacket packet = new UpdatePlayerListPacket();
            packet.Players = new List<PlayerListPlayer>();
            // Sortiere nach Punkten vor
            foreach (Player p in players.OrderByDescending(p => p.OverallPoints))
            {
                packet.Players.Add(new PlayerListPlayer() { DisplayName = p.DisplayName, PlayerId = p.Id });
            }
            return packet;
        }

        // Verpacke Player in eigenes Object, sodass nur gewünschte Felder übertragen werden
        [JsonObject(MemberSerialization.OptIn)]
        public class PlayerListPlayer
        {
            [JsonProperty] public int PlayerId { get; set; }
            [JsonProperty] public string DisplayName { get; set; }
        }
    }
}