using Newtonsoft.Json;

namespace DrunkenboldeServer.Models
{
    /// <summary>
    /// Model which holds player data.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Player
    {
        public string ConnectionId;

        [JsonProperty] public bool IsAdmin { get; set; }
        public string Key { get; set; }

        [JsonProperty] public string DisplayName { get; set; }

        [JsonProperty] public int Points { get; set; }
        [JsonProperty] public int OverallPoints { get; set; }

        public bool Active { get; set; }

        [JsonProperty] public int Id { get; set; }
    }
}