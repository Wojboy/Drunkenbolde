using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrunkenboldeServer.Models;
using DrunkenboldeServer.Packet;

namespace DrunkenboldeServer
{
    public abstract class Scene
    {
        public string Name;
        public int SceneTime;
        protected GameRoom Room;
        protected SceneManager SceneManager;

        public abstract Type NextScene();
        public abstract void OnPacketReceived(Player player, JsonPacket Packet);
        public abstract void OnPlayerConnected(Player player);
        public abstract void OnPlayerDisconnected(Player player);

        public void Init(GameRoom room, SceneManager scene)
        {
            SceneManager = scene;
            Room = room;
        }

        public abstract void OnSceneStarted();
        public abstract void OnSceneClosed();
        public abstract SceneType GetSceneType();
    }
}