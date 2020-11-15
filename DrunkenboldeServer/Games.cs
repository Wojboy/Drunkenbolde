using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrunkenboldeServer.Scene;

namespace DrunkenboldeServer
{
    public class Games
    {
        public static Game[] GamesList = new Game[] {
            new Game{Id = 0, Name = "Pferderennen", Image = "pferd.png", SceneType = typeof(HorseGameScene)}
        };
    }

    public class Game
    {
        public int Id;
        public string Name;
        public string Image;
        public Type SceneType;
    }
}