using System;
using DrunkenboldeServer.Models;
using DrunkenboldeServer.Packet;

namespace DrunkenboldeServer.Scene
{
    public abstract class Scene
    {
        public DateTime StartTime;
        
        protected GameRoom Room;
        protected SceneManager SceneManager;
        public bool IsDone { get; set; }

        public abstract Type NextScene();
        public abstract void OnPacketReceived(Player player, JsonPacket packet);
        public abstract void OnPlayerConnected(Player player);
        public abstract void OnPlayerDisconnected(Player player);

        public virtual void Init(GameRoom room, SceneManager scene)
        {
            SceneManager = scene;
            Room = room;
        }

        public abstract void OnSceneStarted();
        public abstract void OnSceneClosed();
        public abstract SceneType GetSceneType();
        public abstract int GetSceneTime();
        public abstract void Tick();
    }
}