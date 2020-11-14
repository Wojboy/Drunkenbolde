using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrunkenboldeServer.Models;
using DrunkenboldeServer.Packet;

namespace DrunkenboldeServer
{
    public class GameRoomManager
    {
        protected List<GameRoom> Rooms;

        private static GameRoomManager instance = null;
        private static readonly object padlock = new object();

        public static GameRoomManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new GameRoomManager();
                    }
                    return instance;
                }
            }
        }

        public GameRoomManager()
        {
            Rooms = new List<GameRoom>();
        }

        public GameRoom GetGameRoom(string key)
        {
            var ele = Rooms.FirstOrDefault(room => room.RoomName == key);
            if (ele == null)
            {
                ele = new GameRoom(key);
                Rooms.Add(ele);
            }

            return ele;
        }

        public void OnDisconnected(string connectionId)
        {
            foreach (GameRoom room in Rooms)
            {
                var player = room.GetPlayer(connectionId);
                if (player != null)
                {
                    room.PlayerDisconnected(player);

                    // Kein Spieler übrig, schließe raum
                    var activePlayer = room.GetPlayers().FirstOrDefault(a => a.Active);
                    if (activePlayer == null)
                    {
                        room.Stop();
                        Rooms.Remove(room);
                    }
                }

            }
        }

        protected Player GetPlayerFromConnectionId(string conString)
        {
            foreach (GameRoom room in Rooms)
            {
                var player = room.GetPlayer(conString);
                if (player != null)
                    return player;
            }

            return null;
        }

        public void PacketReceived(int packetNumber, string payload, string connectionId)
        {
            var packet = PacketHandler.DecodePacket(packetNumber, payload);
            if (packet == null)
                return;

            if (packet.GetType() == typeof(LoginPacket))
            {
                // Packet is Login Packet
                LoginPacket loginPacket = new LoginPacket();

                var room = Rooms.FirstOrDefault(r => r.RoomName == loginPacket.Room);

                if (room == null)
                {
                    room = new GameRoom(loginPacket.Room);
                    Rooms.Add(room);
                }
                
                room.ReceivePacket(connectionId, loginPacket);
            }
            else
            {   
                // Andere Packete, forwarde zum Raum
                foreach (GameRoom room in Rooms)
                {
                    var player = room.GetPlayer(connectionId);
                    if (player != null)
                        room.ReceivePacket(player, packet);
                }
            }

        }
    }
}