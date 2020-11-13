using DrunkenboldeServer.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrunkenboldeServer
{
    /// <summary>
    /// Class which contains game room data
    /// </summary>
    public class GameRoom
    {
        private int roomId { get; set; }
        public string RoomName { get; set; }
        private List<Player> players { get; set; }
        private IHubContext hubContext { get; set; }
        private static readonly object padlock = new object();
        private static GameRoom instance = null;
        public bool isNew { get; set; }

        public static GameRoom Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new GameRoom();
                        instance.isNew = true;
                        
                    }

                    return instance;
                }
            }
        }

        public GameRoom()
        {
            //this.roomName = name;
            this.players = new List<Player>();
        }

        /// <summary>
        /// Adds a player to the game room.
        /// </summary>
        /// <param name="player">The player to add.</param>
        /// <returns>returns true, if successful, false if not.</returns>
        public bool AddPlayerToGameRoom(Player player)
        {
            if (this.players.Contains(player))
            {
                return false;
            }
            this.players.Add(player);
            return true;
        }

        /// <summary>
        /// Removes a player from a game room.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>true, if successful, false if not.</returns>
        public bool RemovePlayerFromGameRoom(Player player)
        {
            if (this.players.Contains(player))
            {
                this.players.Remove(player);
                return true;
            }
            return false;
        }
    }
}