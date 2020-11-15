using DrunkenboldeServer.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrunkenboldeServer.Packet;
using DrunkenboldeServer.Scene;

namespace DrunkenboldeServer
{
    /// <summary>
    /// Class which contains game room data
    /// </summary>
    public class GameRoom
    {
        public string RoomName { get; set; }
        protected List<Player> Players { get; set; }
        protected SceneManager SceneManager;
        protected GameSettings Settings;
        protected GameLoop GameLoop;

        public GameRoom(string name)
        {
            this.RoomName = name;
            this.Players = new List<Player>();

            //Players.Add(new Player(null, "Franz") { Active = true, Id = 5, OverallPoints = 40, Points = 5 });
            //Players.Add(new Player(null, "Xaverasdfsdfsdfdf") { Active = true, Id = 6, OverallPoints = 20, Points = 7 });

            GameLoop = new GameLoop(this);
            GameLoop.Start();
            Settings = new GameSettings();
        }

        public void InitGameRoom()
        {
            SceneManager = new SceneManager(GameLoop, Settings, this);
        }

        public void ReceivePacket(string connectionId, LoginPacket loginPacket)
        {
            var p = Players.FirstOrDefault(pl => pl.DisplayName == loginPacket.DisplayName);
            if (p == null)
            {
                p = new Player(connectionId, loginPacket.DisplayName);
                p.Active = true;
                p.Points = 7;
                Players.Add(p);

                PlayerConnected(p);
            }
            else
            {
                p.ConnectionId = connectionId;

                PlayerConnected(p);

                // sende an alten Spieler disconnect aufruf
            }
        }

        protected void PlayerConnected(Player player)
        {
            SendToPlayer(player, new LoginAnswerPacket() {PlayerId = player.Id});
            SendToPlayer(player, PlayerListPacket.GenerateFromPlayerList(Players));
            SendToAllPlayers(new MessagePacket() { Message = "Spieler '" + player.DisplayName + "' verbunden." });
            SceneManager?.PlayerConnected(player);
        }

        public void SendToPlayers(List<Player> players, JsonPacket packet)
        {
            var data = PacketHandler.EncodePacket(packet);
            if (data == null)
                throw new Exception("Packet is not valid");
            var connectionIds = (from p in players where p.Active select p.ConnectionId).ToList();

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
            hubContext.Clients.Clients(connectionIds).Post((int)packet.GetPacketType(), data);
        }

        public void SendToPlayer(Player p, JsonPacket packet)
        {
            var data = PacketHandler.EncodePacket(packet);
            if (data == null)
                throw new Exception("Packet is not valid");
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
            hubContext.Clients.Client(p.ConnectionId).Post((int)packet.GetPacketType(), data);
        }

        public void PlayerDisconnected(Player player)
        {
            player.Active = false;
            SendToAllPlayers(new MessagePacket() { Message = "Spieler '" + player.DisplayName + "' ist weg." });
            SendToPlayer(player, PlayerListPacket.GenerateFromPlayerList(Players));
            SceneManager.PlayerDisconnected(player);
        }

        public void ReceivePacket(Player player, JsonPacket packet)
        {
            SceneManager.ReceivePacket(player, packet);
        }

        public List<Player> GetPlayers()
        {
            return Players;
        }

        public Player GetPlayer(string conString)
        {
            return Players.FirstOrDefault(p => p.ConnectionId == conString);
        }

        public void Tick()
        {
            SceneManager?.Tick();
        }

        public void Stop()
        {
            GameLoop.Stop();
        }

        public void SendToAllPlayers(JsonPacket packet)
        {
            if (Players.Count == 0)
                return;
            SendToPlayers(Players, packet);
        }

        public void Init()
        {
            SceneManager = new SceneManager(GameLoop, Settings, this);
        }
    }
}