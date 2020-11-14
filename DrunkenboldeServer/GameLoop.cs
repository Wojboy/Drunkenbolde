using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;

namespace DrunkenboldeServer
{
    /// <summary>
    /// Does the game loop.
    /// </summary>
    public class GameLoop
    {
        private GameRoom currentRoom { get; set; }
        private Timer timer { get; set; }


        public GameLoop(GameRoom room)
        {
            this.currentRoom = room;
            if (room == null)
            {
                throw new ArgumentNullException("Room nicht gefüllt");
            }
            this.timer = new Timer();
            this.timer.Interval = 100;
            this.timer.Elapsed += CallBack;
        }

        public void Start()
        {
            this.timer.Start();   
        }

        public void Stop()
        {
            this.timer.Stop();
        }

        /// <summary>
        /// The callback - Timer.Tick event.
        /// </summary>
        /// <param name="state"></param>
        private void CallBack(object sender, ElapsedEventArgs e)
        {
            currentRoom.Tick();
        }
    }
}