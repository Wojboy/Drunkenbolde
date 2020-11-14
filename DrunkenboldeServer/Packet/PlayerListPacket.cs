using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrunkenboldeServer.Models;
using Newtonsoft.Json;

namespace DrunkenboldeServer.Packet
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PlayerListPacket : JsonPacket
    {
        [JsonProperty] public List<PlayerListPlayer> Players = new List<PlayerListPlayer>();

        public override bool IsValid()
        {
            return Players != null && Players.Count > 0;
        }

        public override PacketType GetPacketType()
        {
            return PacketType.PlayerList;
        }

        public static PlayerListPacket GenerateFromPlayerList(List<Player> players)
        {
            PlayerListPacket packet = new PlayerListPacket();

            // Sortiere nach Punkten vor
            foreach(Player p in players.OrderByDescending(p => p.OverallPoints))
            {
                packet.Players.Add(new PlayerListPlayer() {DisplayName = p.DisplayName, PlayerId = p.Id, OverallPoints = p.OverallPoints, Points = p.Points});
            }
            return packet;
        }

        // Verpacke Player in eigenes Object, sodass nur gewünschte Felder übertragen werden
        [JsonObject(MemberSerialization.OptIn)]
        public class PlayerListPlayer
        {
            [JsonProperty] public int PlayerId { get; set; }
            [JsonProperty] public string DisplayName { get; set; }
            [JsonProperty] public int Points { get; set; }
            [JsonProperty] public int OverallPoints { get; set; }
        }
    }
}