using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrunkenboldeServer.Models;
using DrunkenboldeServer.Packet;

namespace DrunkenboldeServer.Scene
{
    public class ScoreboardScene : Scene
    {

        public override Type NextScene()
        {
            if (LastSceneType != null)
            {
                if (LastSceneType == typeof(ShareScene))
                    return typeof(WaitingScene);
                else
                    return typeof(GambleScene);
            }
            else
            {
                throw new Exception("Last Scene not set");
            }
        }

        public override void OnPacketReceived(Player player, JsonPacket packet)
        {
        }

        public override void Init(GameRoom room, SceneManager scene)
        {
            base.Init(room, scene);

        }
        public override void OnPlayerConnected(Player player)
        {
            SendMovementPlayerList(null);
        }

        public override void OnPlayerDisconnected(Player player)
        {

        }

        public override void OnSceneStarted()
        {
            SendMovementPlayerList(null);
        }

        private void SendMovementPlayerList(Player onlyPlayer)
        {
            var playerPacket = ScoreboardPacket.GenerateFromPlayerList(Room.GetActivePlayers());

            if(onlyPlayer != null)
                Room.SendToPlayer(onlyPlayer, playerPacket);
            else
                Room.SendToAllPlayers(playerPacket);
        }

        public override void OnSceneClosed()
        {

        }

        public override SceneType GetSceneType()
        {
            return SceneType.Scoreboard;
        }

        public override int GetSceneTime()
        {
            return 15;
        }

        public override void Tick()
        {
            if (StartTime.AddSeconds(GetSceneTime()) < DateTime.Now)
            {
                IsDone = true;
            }
        }
    }
}