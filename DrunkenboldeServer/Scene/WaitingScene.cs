using System;
using DrunkenboldeServer.Models;
using DrunkenboldeServer.Packet;

namespace DrunkenboldeServer.Scene
{
    public class WaitingScene : Scene
    {
        public override Type NextScene()
        {
            return null;
        }

        public override void OnPacketReceived(Player player, JsonPacket packet)
        {

        }

        public override void OnPlayerConnected(Player player)
        {

        }

        public override void OnPlayerDisconnected(Player player)
        {

        }

        public override void OnSceneStarted()
        {

        }

        public override void OnSceneClosed()
        {

        }

        public override SceneType GetSceneType()
        {
            return SceneType.Waiting;
        }

        public override int GetSceneTime()
        {
            return 30;
        }

        public override void Tick()
        {
        }
    }
}