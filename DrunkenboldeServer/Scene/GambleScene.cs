using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrunkenboldeServer.Models;
using DrunkenboldeServer.Packet;

namespace DrunkenboldeServer.Scene
{
    public class GambleScene : Scene
    {
        protected bool resultSent;
        protected GambleResultPacket Results;

        public override void Init(GameRoom room, SceneManager scene)
        {
            base.Init(room, scene);
            Results = new GambleResultPacket { States = new List<GambleResultItem>() };

            foreach (Player p in Room.GetActivePlayers())
            {
                Results.States.Add(new GambleResultItem() { PlayerId = p.Id, PlayerName = p.DisplayName });
            }

        }
        public override Type NextScene()
        {
            return typeof(ShareScene);
        }

        public override void OnPacketReceived(Player player, JsonPacket packet)
        {
            if (packet.GetType() != typeof(GambleSetPacket)) return;

            if (resultSent)    // Erlaube kein Gamblen mehr, Ergebnisse bereits raus
                return;
            var gambleSet = (GambleSetPacket)packet;
            var item = Results.States.FirstOrDefault(p => p.PlayerId == player.Id);
            if (item == null)
                return;

            if (gambleSet.AmountBlack > 0)
                item.AmountBlack = gambleSet.AmountBlack;
            else
                item.AmountRed = gambleSet.AmountRed;
        }

        public override void OnPlayerConnected(Player player)
        {
            var item = Results.States.FirstOrDefault(p => p.PlayerId == player.Id);
            if(item == null)
                Results.States.Add(new GambleResultItem() {PlayerId =  player.Id, PlayerName = player.DisplayName});

            if (resultSent)
            {
                // Gamblen abgeschlossen, sende Resultate
                Room.SendToPlayer(player, Results);
            }
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
            return SceneType.Gamble;
        }

        public override int GetSceneTime()
        {
            return 20;
        }

        public override void Tick()
        {
            if (IsDone)
                return;

            if (StartTime.AddSeconds(GetSceneTime() + 3) <= DateTime.Now)
            {
                //Schließe Scene
                IsDone = true;
            }
            else if (!resultSent && StartTime.AddSeconds(GetSceneTime()) <= DateTime.Now)
            {
                resultSent = true;
                var black = Results.Black;
                foreach (var d in Results.States)
                {
                    var player = Room.GetActivePlayers().FirstOrDefault(p => p.Id == d.PlayerId);
                    if (player != null)
                    {
                        int add = 0;
                        if ((black && d.AmountBlack > 0) || (!black && d.AmountRed > 0))
                        {
                            add = 3;
                        }
                        else if ((!black && d.AmountBlack > 0) ||
                                 (black && d.AmountRed > 0))
                        {
                            add = -3;
                        }
                        else
                        {
                            continue;
                        }

                        player.Points += add;
                    }
                }

                Room.SendToAllPlayers(Results);
                Room.SendToAllPlayers(ScoreboardPacket.GenerateFromPlayerList(Room.GetActivePlayers()));
                // Zeige Resultate noch für 3 Sekunden an
            }
        }
    }
}