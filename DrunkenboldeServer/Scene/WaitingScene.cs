using System;
using System.Collections.Generic;
using System.Linq;
using DrunkenboldeServer.GameLogic;
using DrunkenboldeServer.Models;
using DrunkenboldeServer.Packet;

namespace DrunkenboldeServer.Scene
{
    public class WaitingScene : Scene
    {
        protected Dictionary<int,int> PlayerVotes;
        protected bool ResultsSent;
        public override Type NextScene()
        {
            return null;
        }

        public override void OnPacketReceived(Player player, JsonPacket packet)
        {
            if (packet.GetType() == typeof(WaitingVotePacket))
            {
                var votePacket = (WaitingVotePacket) packet;
                if (votePacket.GameId < Games.GamesList.Length)
                {
                    if (!PlayerVotes.ContainsKey(player.Id))
                    {
                        PlayerVotes.Add(player.Id, votePacket.GameId);

                        var item = GetHighestVotes();
                        Room.SendToPlayer(player, new WaitingVoteSelectionPacket() { Game = item?.Item1 ?? -1, Count = PlayerVotes.Count, OverallCount = Room.GetActivePlayersCount() });
                    }
                }
            }
        }

        public override void OnPlayerConnected(Player player)
        {
            PlayerVotes = new Dictionary<int, int>();
            Room.SendToPlayer(player, new WaitingGamesListPacket() { GamesList = Games.GamesList.ToList() });

            var item = GetHighestVotes();
            // Schicke höchstes Element zurück um es beim Client zu markieren
            Room.SendToPlayer(player, new WaitingVoteSelectionPacket() { Game = item?.Item1 ?? -1, Count = PlayerVotes.Count, OverallCount = Room.GetActivePlayersCount() });
        }

        public override void OnPlayerDisconnected(Player player)
        {

        }

        public override void Init(GameRoom room, SceneManager scene)
        {
            base.Init(room, scene);
            PlayerVotes = new Dictionary<int, int>();
        }

        public override void OnSceneStarted()
        {
            Room.SendToAllPlayers(new WaitingGamesListPacket() { GamesList = Games.GamesList.ToList() });

            // Schicke höchstes Element zurück um es beim Client zu markieren
            Room.SendToAllPlayers(new WaitingVoteSelectionPacket() { Game = -1, Count = PlayerVotes.Count, OverallCount = Room.GetActivePlayersCount() });
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
            return -1;
        }

        public override void Tick()
        {

            if (PlayerVotes.Count == 0 || ResultsSent)
                return;

            int playerCount = Room.GetActivePlayersCount();

            // hälfte players muss gevotet haben
            if ((PlayerVotes.Count == 1 && playerCount == 1) || (PlayerVotes.Count >= (Room.GetActivePlayersCount() / 2.0)))
            {
                var item = GetHighestVotes();
                SceneManager.ChangeScene((GameScene)Activator.CreateInstance(Games.GamesList[item.Item1].SceneType), Games.GamesList[item.Item1].Id);
                ResultsSent = true;
            }
        }

        protected Tuple<int,int> GetHighestVotes()
        {
            if (PlayerVotes.Count == 0)
                return null;

            int[] data = new int[Games.GamesList.Length];
            foreach (KeyValuePair<int, int> ele in PlayerVotes)
            {
                data[ele.Value] += 1;
            }

            int maxValue = data.Max();
            int maxIndex = data.ToList().IndexOf(maxValue);
            return new Tuple<int, int>(maxIndex,maxValue);
        }
    }
}