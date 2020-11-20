using Newtonsoft.Json;

namespace DrunkenboldeServer.Models
{
    /// <summary>
    /// Model which holds player data.
    /// </summary>
    public class Player
    {
        public Player(string connectionId, string displayName)
        {
            ConnectionId = connectionId;
            DisplayName = displayName;
        }
        public string ConnectionId;
        public string DisplayName { get; set; }

        public int Points { get; set; }
        public int OverallPoints { get; set; }
        public int Drunk { get; set; }
        public int LastPoints = -1;

        public bool Active { get; set; }

        public int Id { get; set; }
    }
}