using System;
using DrunkenboldeServer.Models;
using DrunkenboldeServer.Packet;

namespace DrunkenboldeServer.Scene
{
    public class SceneManager
    {

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
            CurrentScene = new ShareScene();
            CurrentScene.Init(GameRoom, this);
            CurrentScene.StartTime = DateTime.Now;
        }

        public void Tick()
        {
            CurrentScene.Tick();
            if (CurrentScene.IsDone)
            {
                Type t = CurrentScene.NextScene();
                var instance = (DrunkenboldeServer.Scene.Scene) Activator.CreateInstance(t);
                instance.StartTime = DateTime.Now;
                instance.Init(GameRoom, this);
                ChangeScene(instance);
            }
        }

        public void ChangeScene(DrunkenboldeServer.Scene.Scene scene)
        {
            GameRoom.SendToAllPlayers(new MessagePacket() {Message = " Scene gewechselt: " + scene.GetType().FullName});
            CurrentScene?.OnSceneClosed();
            CurrentScene = scene;
            GameRoom.SendToAllPlayers(new ChangeScenePacket() {SceneDuration =  DisableTimedScenes ? 0 : scene.GetSceneTime(), SceneType = (int)scene.GetSceneType()});

        }

        public void ReceivePacket(Player player, JsonPacket packet)
        {
            CurrentScene.OnPacketReceived(player, packet);    
        }

        public void PlayerConnected(Player player)
        {
            GameRoom.SendToPlayer(player, new ChangeScenePacket() { SceneDuration = DisableTimedScenes ? 0 : CurrentScene.GetSceneTime(), SceneType = (int)CurrentScene.GetSceneType() });
            CurrentScene.OnPlayerConnected(player);
        }

        public void PlayerDisconnected(Player player)
        {
            CurrentScene.OnPlayerDisconnected(player);
        }
    }
}