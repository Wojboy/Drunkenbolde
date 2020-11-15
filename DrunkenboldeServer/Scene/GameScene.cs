using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrunkenboldeServer.Scene
{
    public abstract class GameScene : Scene
    {
        public override Type NextScene()
        {
            return typeof(GambleScene);
        }

        public override int GetSceneTime()
        {
            return -1;
        }
    }
}