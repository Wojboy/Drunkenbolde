using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrunkenboldeServer.Models;
using DrunkenboldeServer.Packet;

namespace DrunkenboldeServer.Scene
{
    public class ShareScene : Scene
    {
        protected Dictionary<int,int> PlayedSharedValues = new Dictionary<int, int>();
        protected ShareResultPacket Result;
        protected bool ResultsSent;

        public ShareScene()
        {
        }

        public override void Init(GameRoom room, SceneManager manager)
        {
            base.Init(room, manager);

            Result = new ShareResultPacket();
            Result.Data = new List<ShareResultElement>();

            foreach (var p in Room.GetPlayers())
            {
                if (!p.Active)
                    continue;

                Result.Data.Add(new ShareResultElement() {DisplayName = p.DisplayName, PlayerId = p.Id, DrinkData = new Dictionary<int, int>()});
                PlayedSharedValues.Add(p.Id, 0);
            }
        }
        public override Type NextScene()
        {
            return typeof(WaitingScene);
        }

        public override void OnPacketReceived(Player player, JsonPacket packet)
        {
            if (packet.GetType() == typeof(ShareSetPacket))
            {
                var setPacket = (ShareSetPacket) packet;
                var item = Result.Data.FirstOrDefault(i => i.PlayerId == setPacket.PlayerId);
                if (item != null)
                {
                    if (item.DrinkData.ContainsKey(player.Id))
                        item.DrinkData[player.Id] = setPacket.Amount;
                    else
                        item.DrinkData.Add(player.Id, setPacket.Amount);
                }
            }
        }

        public override void OnPlayerConnected(Player player)
        {
            var item = Result.Data.FirstOrDefault(r => r.PlayerId == player.Id);
            if (item == null)
            {
                Result.Data.Add(new ShareResultElement() { PlayerId = player.Id, DisplayName = player.DisplayName, DrinkData = new Dictionary<int, int>() });
                PlayedSharedValues.Add(player.Id, 0);
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
            return SceneType.Share;
        }

        public override int GetSceneTime()
        {
            return 15;
        }

        public override void Tick()
        {
            if (StartTime.AddSeconds(GetSceneTime() + 10) <= DateTime.Now)
            {
                IsDone = true;
            }
            else if (StartTime.AddSeconds(GetSceneTime()) <= DateTime.Now)
            {
                if (ResultsSent)
                    return;
                ResultsSent = true;
                foreach (ShareResultElement ele in Result.Data)
                {
                    foreach (KeyValuePair<int, int> drinkData in ele.DrinkData)
                    {
                        // Suche austeilenden Spieler
                        var player = Room.GetPlayers().FirstOrDefault(p => p.Id == drinkData.Key);

                        if (player == null)
                            continue;

                        // Kann Spieler überhaupt noch verteilen
                        if (PlayedSharedValues[player.Id] + drinkData.Value <= player.Points)
                        {
                            PlayedSharedValues[player.Id] += drinkData.Value;
                            ele.DrinkValue += drinkData.Value;
                        }
                    }
      
                }
                Room.SendToAllPlayers(Result);
            }
            else
            {

            }
        }
    }
}