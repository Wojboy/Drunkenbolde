using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DrunkenboldeServer
{
    public class GameConfig
    {

        private static GameConfig instance = null;
        private static readonly object padlock = new object();

        GameConfig()
        {
        }

        public static GameConfig Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new GameConfig();
                        instance.Players = new List<Player>();
                        instance.Players.Add(new Player()
                        {
                            Id = 0, DisplayName = "Test", Key = "asdfasdf", IsAdmin = true, OverallPoints = 7,
                            Points = 5
                        });
                        instance.Players.Add(new Player()
                        {
                            Id = 1, DisplayName = "Franz", Key = "franz", IsAdmin = false, Active = true,
                            OverallPoints = 25, Points = 5
                        });
                        instance.Players.Add(new Player()
                        {
                            Id = 2, DisplayName = "Xavererererer", Key = "xaver", IsAdmin = false, Active = true,
                            OverallPoints = 35, Points = 10
                        });
                        instance.LobbyState = GameHub.LobbyStates.GameEnded;
                        instance.nextStep = DateTime.Now.AddSeconds(10);
                    }

                    return instance;
                }
            }
        }

        public List<Player> Players { get; set; }

        public int DuplicateDrinksDuration = 30;
        public int DuplicateDrinksMaxAmount = 5;

        public DateTime nextStep;
        public GameHub.LobbyStates LobbyState = GameHub.LobbyStates.Waiting;
        public DuplicateDrinksResult DDResult;

        public List<ShareDrinksPlayerData> PlayerData;


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

        [JsonObject(MemberSerialization.OptIn)]
        public class ShareDrinksPlayerData
        {
            public int PlayerId;
            public int Availaible;
            public int Used;
            [JsonProperty]
            public List<ShareDrinksPlayer> ShareDrinks { get; set; }
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class ShareDrinksPlayer
        {
            [JsonProperty]
            public int PlayerId { get; set; }

            [JsonProperty]
            public int Amount { get; set; }
        }
    }
}