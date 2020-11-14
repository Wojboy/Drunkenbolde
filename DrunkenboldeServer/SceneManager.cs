using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrunkenboldeServer.Models;
using DrunkenboldeServer.Packet;

namespace DrunkenboldeServer
{
    public class SceneManager
    {
        protected DateTime NextSceneTimer;
        public Scene CurrentScene;

        public bool DisableTimedScenes { get; set; }
        protected GameLoop GameLoop;
        protected GameSettings GameSettings;
        protected GameRoom GameRoom;
        public SceneManager(GameLoop loop, GameSettings settings, GameRoom room)
        {
            GameLoop = loop;
            GameSettings = settings;
            GameRoom = room;
        }

        public void Tick()
        {
            if (DisableTimedScenes)
                return;

            if (NextSceneTimer >= DateTime.Now)
            {
                Type t = CurrentScene.NextScene();
                var instance = (Scene)Activator.CreateInstance(t);
                ChangeScene(instance);
            }
        }

        public void ChangeScene(Scene scene)
        {
            CurrentScene.OnSceneClosed();
            if (!DisableTimedScenes)
                GameRoom.SendToAllPlayers(new ChangeScenePacket()
                    {SceneDuration = scene.SceneTime, SceneType = scene.GetSceneType()});
            CurrentScene = scene;
        }

        public void ReceivePacket(Player player, JsonPacket packet)
        {
            CurrentScene.OnPacketReceived(player, packet);    
        }

        public void PlayerConnected(Player p)
        {
            CurrentScene.OnPlayerConnected(p);
        }

        public void PlayerDisconnected(Player player)
        {
            CurrentScene.OnPlayerDisconnected(player);
        }
    }
}