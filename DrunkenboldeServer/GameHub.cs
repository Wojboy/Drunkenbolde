using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using DrunkenboldeServer.Models;

namespace DrunkenboldeServer
{
    public class GameHub : Hub
    {
        // GameHub wird nur als Verbindungsmanager benutzt, Handling der Packetzuordnung findet in GameRoomManager statt

        public override Task OnDisconnected(bool stopCalled)
        {
            GameRoomManager.Instance.OnDisconnected(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        public void Post(int packetType, string payload)
        {
            GameRoomManager.Instance.PacketReceived(packetType, payload, Context.ConnectionId);
        }

    }
}