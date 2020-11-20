using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrunkenboldeServer.Models;
using Newtonsoft.Json;

namespace DrunkenboldeServer.Packet
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ScoreboardPacket : JsonPacket
    {
        [JsonProperty] public List<ScoreboardPlayer> Players { get; set; }

        public override bool IsValid()
        {
            return Players != null && Players.Count > 0;
        }

        public override PacketType GetPacketType()
        {
            return PacketType.Scoreboard;
        }

        public static ScoreboardPacket GenerateFromPlayerList(List<Player> players)
        {
            ScoreboardPacket packet = new ScoreboardPacket();
            packet.Players = new List<ScoreboardPlayer>();
            // Sortiere nach Punkten vor
            foreach(Player p in players.OrderByDescending(p => p.OverallPoints))
            {
                int movement = 1;
                if (p.LastPoints == -1 || p.LastPoints == p.OverallPoints)
                {
                    movement = 1;
                }
                else if (p.LastPoints > p.OverallPoints)
                {
                    movement = 2;
                }
                else if (p.LastPoints < p.OverallPoints)
                {
                    movement = 0;
                }

                packet.Players.Add(new ScoreboardPlayer() {DisplayName = p.DisplayName, PlayerId = p.Id, OverallPoints = p.OverallPoints, Points = p.Points, Movement = movement, Drunk = p.Drunk});
            }
            return packet;
        }

        // Verpacke Player in eigenes Object, sodass nur gewünschte Felder übertragen werden
        [JsonObject(MemberSerialization.OptIn)]
        public class ScoreboardPlayer
        {
            [JsonProperty] public int PlayerId { get; set; }
            [JsonProperty] public string DisplayName { get; set; }
            [JsonProperty] public int Points { get; set; }
            [JsonProperty] public int OverallPoints { get; set; }
            [JsonProperty] public int Drunk { get; set; }
            [JsonProperty] public int Movement { get; set; }
        }
    }
}