using Newtonsoft.Json;

namespace DrunkenboldeServer.Models
{
    /// <summary>
    /// Model which holds player data.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Player
    {
        public Player(string connectionId, string displayName)
        {
            ConnectionId = connectionId;
            DisplayName = displayName;
        }
        public string ConnectionId;
        [JsonProperty] public string DisplayName { get; set; }

        [JsonProperty] public int Points { get; set; }
        [JsonProperty] public int OverallPoints { get; set; }

        public bool Active { get; set; }

        [JsonProperty] public int Id { get; set; }

        [JsonProperty] public bool IsSongProvider { get; set; }
    }
}