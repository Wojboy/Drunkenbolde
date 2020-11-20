using System.Collections.Generic;
using DrunkenboldeServer.Models;
using DrunkenboldeServer.Packet;
using DrunkenboldeServer.Scene;

namespace DrunkenboldeServer.GameLogic.HorseGame
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

        public override void Tick()
        {

        }
    }
}