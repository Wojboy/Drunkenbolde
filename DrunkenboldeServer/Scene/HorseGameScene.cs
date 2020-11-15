using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrunkenboldeServer.Models;
using DrunkenboldeServer.Packet;

namespace DrunkenboldeServer.Scene
{
    public class HorseGameScene : GameScene
    {
        public Dictionary<int, int> VoteCounts;
        public Dictionary<int,bool> PlayerVoted;

        public override void OnPacketReceived(Player player, JsonPacket packet)
        {
            if (packet.GetType() != typeof(WaitingVotePacket))
                return;
            
            var votePacket = (WaitingVotePacket) packet;

            if (!PlayerVoted.ContainsKey(player.Id) || VoteCounts.ContainsKey(votePacket.GameId))
                return;
            
            
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
            return SceneType.HorseScene;
        }

        public override void Tick()
        {

        }
    }
}