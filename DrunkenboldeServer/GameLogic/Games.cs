using System;
using DrunkenboldeServer.GameLogic.HorseGame;
using DrunkenboldeServer.Scene;
using Newtonsoft.Json;

namespace DrunkenboldeServer.GameLogic
{
    public class Games
    {
        public static Game[] GamesList = new Game[] {
            new Game{Id = 0, Name = "Pferderennen", Image = "pferd.svg", SceneType = typeof(HorseGameScene)},
            new Game{Id = 1, Name = "Liederraten", Image = "pferd.svg", SceneType = typeof(SongGuessingScene)}
        };

        public enum GameTypes
        {
            HorseGame = 0,
            SongGuessing = 1
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Game
    {
        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string Image { get; set; }

        public Type SceneType;
    }
}