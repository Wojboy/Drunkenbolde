using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrunkenboldeServer.GameLogic.SongGuessing;
using DrunkenboldeServer.Models;
using DrunkenboldeServer.Packet;

namespace DrunkenboldeServer.Scene
{
    public class SongGuessingScene : Scene
    {
        public override void Init(GameRoom room, SceneManager scene)
        {
            base.Init(room, scene);


            var link = SongGuesserHelper.ProcessLink("https://www.youtube.com/watch?v=5ZBgx1NeUWg&ab_channel=WejustmanRecords");

        }

        public override int GetSceneTime()
        {
            return -1;
        }

        public override SceneType GetSceneType()
        {
            return SceneType.SongGuesser;
        }

        public override Type NextScene()
        {
            return typeof(GambleScene);
        }

        /// <summary>
        /// Called when client is sending packet to server.
        /// </summary>
        /// <param name="player">the player</param>
        /// <param name="packet">the packet</param>
        public override void OnPacketReceived(Player player, JsonPacket packet)
        {
            /// Song link paket
            /// 
            if (packet.GetPacketType() == PacketType.SongGuessingSongPacket)
            {
                if (!player.IsSongProvider)
                {
                    return;
                }

                var songLinkPacket = packet as SongGuessingPacket;
                if (songLinkPacket == null)
                {
                    return;
                }

                var link = songLinkPacket.SongLink;
                if (!string.IsNullOrEmpty(SongGuesserHelper.ProcessLink(link)))
                {
                    Room.SendToAllPlayers(new SongGuessingPacket { SongLink = link });
                    /// Send to all clients
                }

            }

            /// Song Antwort paket
            /// 
            if (packet.GetPacketType() == PacketType.SongGuessingAnswerPacket)
            {

            }

            /// Song was right paket
            /// 
            //// Bestätigung -> Gewinner 
            if (packet.GetPacketType() == PacketType.SongGuessingAnswerRightPacket)
            {

            }

            
        }

        public override void OnPlayerConnected(Player player)
        {
            ///
        }

        public override void OnPlayerDisconnected(Player player)
        {
            
        }

        public override void OnSceneClosed()
        {
            throw new NotImplementedException();
        }

        public override void OnSceneStarted()
        {
            throw new NotImplementedException();
        }

        public override void Tick()
        {
            /// If players voted for end
            /// IsDone=true;
        }
    }
}