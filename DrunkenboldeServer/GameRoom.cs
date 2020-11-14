using DrunkenboldeServer.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrunkenboldeServer.Packet;

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
            GameLoop = new GameLoop(this);
            Settings = new GameSettings();
            SceneManager = new SceneManager(GameLoop, Settings);
        }


        public void ReceivePacket(string connectionId, LoginPacket loginPacket)
        {
            var p = Players.FirstOrDefault(pl => pl.DisplayName == loginPacket.PlayerName);
            if (p == null)
            {
                p = new Player(connectionId, loginPacket.PlayerName);
                Players.Add(p);

                SceneManager.PlayerConnected(p);
            }
            else
            {
                p.ConnectionId = connectionId;

                if(!p.Active)
                    SceneManager.PlayerConnected(p);
                // Spieler bereits Aktiv, schmeiße alten Spieler raus
                //if (p.Active)
                // Sende disconnect Packet

            }

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

        public void PlayerDisconnected(Player player)
        {
            SceneManager.PlayerDisconnected(player);
            player.Active = false;
        }

        public void Tick()
        {
            SceneManager.Tick();
        }

        public void Stop()
        {
            GameLoop.Stop();
        }

        public bool SendToAllPlayers(JsonPacket packet)
        {
            // Sende an alle aktiven Spieler

            var data = PacketHandler.EncodePacket(packet);
            if (data == null)
                return false;
            var connectionIds = (from p in Players where p.Active select p.ConnectionId).ToList();

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
            hubContext.Clients.Clients(connectionIds).Post(packet.GetPacketType(), data);
            return true;
        }
    }
}