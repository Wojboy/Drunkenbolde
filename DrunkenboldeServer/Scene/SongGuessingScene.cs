using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrunkenboldeServer.GameLogic.SongGuessing;
using DrunkenboldeServer.Models;
using DrunkenboldeServer.Packet;

namespace DrunkenboldeServer.Scene
{
    public class SongGuessingScene : GameScene
    {
        private Song currentSong { get; set; }

        public override void Init(GameRoom room, SceneManager scene)
        {
            base.Init(room, scene);
            currentSong = new Song();

        }

        /// <summary>
        /// Determines who is SongProvider // Host.
        /// </summary>
        private void DetermineWhoIsSongProvider()
        {
            var rnd = new Random();
            
            var players = Room.GetActivePlayers();
            var r = rnd.Next(players.Count);
            var luckyPlayer = players[r];

            var isHostPacket = new SongGuessingIsHostPacket { IsSongProvider = true };
            var isNotHostPacket = new SongGuessingIsHostPacket { IsSongProvider = false };

            Room.SendToPlayer(luckyPlayer, isHostPacket);
            foreach (var player in players.Except(new List <Player>{ luckyPlayer}))
            {
                 
            }
            
        }

        public override int GetSceneTime()
        {
            return -1;
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
            if (packet.GetPacketType() == PacketType.SongGuessingSongPacket)
            {
                var songLinkPacket = packet as SongGuessingPacket;
                if (songLinkPacket == null)
                {
                    return;
                }

                var link = songLinkPacket.SongLink;
                var processedLink = SongGuesserHelper.ProcessLink(link);
                if (!string.IsNullOrEmpty(processedLink))
                {
                    currentSong.Title = songLinkPacket.SongTitle;
                    currentSong.Artist = songLinkPacket.SongArtist;
                    Room.SendToAllPlayers(new SongGuessingPacket { SongLink = processedLink });
                }

            }

            if (packet.GetPacketType() == PacketType.SongGuessingAnswerPacket)
            {
                var songAnswerPacket = packet as SongGuessingAnswerPacket;
                if (songAnswerPacket == null)
                {
                    return;
                }

                if (SongGuesserHelper.CheckIfAnswerRight(songAnswerPacket, currentSong))
                {
                    Room.SendToAllPlayers(new MessagePacket { Message = $"Spieler {player.DisplayName} hat die richtige Antwort gegeben." });
                    Room.SendToAllPlayers(new SongGuessingPacket { SongLink = "" });
                }
            }
            
        }

        public override void OnPlayerConnected(Player player)
        {
            var isNotHostPacket = new SongGuessingIsHostPacket { IsSongProvider = false };
            Room.SendToPlayer(player, isNotHostPacket);

        }

        public override void OnPlayerDisconnected(Player player)
        {
            
        }

        public override void OnSceneClosed()
        {
            
        }

        public override void OnSceneStarted()
        {
            this.DetermineWhoIsSongProvider();
        }

        public override void Tick()
        {
        }
    }
}