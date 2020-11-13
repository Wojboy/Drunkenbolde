using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace DrunkenboldeServer
{
    public class GameHub : Hub
    {
        private Random Random = new Random();
        public enum MessageTypes
        {
            MessageSmall = 0,
            MessageBig = 1,
            Leaderboard = 2,
            Login = 3,
            ChangeState = 4,
            LobbyUpdate = 5,
        }

        public enum LobbyStates
        {
            Waiting = 0,
            ShareDrinks = 1,
            ShareDrinksResults = 2,
            DuplicateDrinks = 3,
            DuplicateDrinksResults = 4,
            InGame = 5,
            GameEnded = 6,
        }

        public static GameConfig Config;




        public GameHub()
        {
            if (Config == null)
            {
                Config = GameConfig.Instance;
            }

        }



        public override Task OnDisconnected(bool stopCalled)
        {
            var player = Config.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);
            if (player != null)
            {
                player.Active = false;
                SendMessage("Spieler '" + player.DisplayName + "' ist raus.");
                player.ConnectionId = null;
            }

            return base.OnDisconnected(stopCalled);
        }

        public void Login(string key)
        {
            GameConfig.Player player = Config.Players.FirstOrDefault(p => p.Key == key);
            if (player != null && !player.Active)
            {
                Clients.Client(Context.ConnectionId).broadcastMessage((int)MessageTypes.Login, player.Id);
                player.ConnectionId = Context.ConnectionId;
                player.Active = true;
                SendMessage("Spieler '" + player.DisplayName + "' ist da");
                UpdatePlayerBoard();
                Clients.Client(Context.ConnectionId).broadcastMessage((int)MessageTypes.ChangeState, (int)Config.LobbyState);


                if (Config.LobbyState == LobbyStates.DuplicateDrinks)
                {
                    if (Config.DDResult.States.Count(p => p.PlayerId == player.Id) == 0)
                    {
                        var state = new DuplicateDrinksState() {PlayerId = player.Id, AmountBlack = 0, AmountRed = 0};
                        Config.DDResult.States.Add(state);
                    }
                }
                else if (Config.LobbyState == LobbyStates.DuplicateDrinksResults)
                {
                    string jsonString = JsonConvert.SerializeObject(Config.DDResult);
                    Clients.Client(player.ConnectionId).broadcastMessage((int)MessageTypes.LobbyUpdate, jsonString);
                }
            }
            else
            {
                Clients.Client(Context.ConnectionId).broadcastMessage((int)MessageTypes.Login, -1);
            }
        }

        public void Ping()
        {
            GameConfig.Player player = Config.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);
            if (player != null && player.IsAdmin && player.Active)
            {
                CheckState(false);
            }
        }
        public void Next()
        {
            GameConfig.Player player = Config.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);
            if (player != null && player.IsAdmin && player.Active)
            {
                CheckState(true);
            }
        }

        protected void CheckState(bool force)
        {
            if (force)
            {
                Config.nextStep = DateTime.Now.Subtract(new TimeSpan(5));
            }

            if (Config.LobbyState == LobbyStates.InGame)
            {
                // Forward to game
            }
            else
            {
                if (DateTime.Now > Config.nextStep)
                {
                    if (Config.LobbyState == LobbyStates.DuplicateDrinks)
                    {
                        Config.nextStep = DateTime.Now.AddSeconds(10);
                        UpdateState(LobbyStates.DuplicateDrinksResults);
                    }
                    else if (Config.LobbyState == LobbyStates.GameEnded)
                    {
                        Config.nextStep = DateTime.Now.AddSeconds(30);
                        UpdateState(LobbyStates.DuplicateDrinks);
                    }
                    else if (Config.LobbyState == LobbyStates.DuplicateDrinksResults)
                    {
                        Config.nextStep = DateTime.Now.AddSeconds(30);
                        UpdateState(LobbyStates.ShareDrinks);
                    }
                    else if (Config.LobbyState == LobbyStates.ShareDrinks)
                    {
                        //Config.nextStep = DateTime.Now.AddSeconds(30);
                        //UpdateState(LobbyStates.Waiting);
                    }
                    else if (Config.LobbyState == LobbyStates.ShareDrinksResults)
                    {
                        //Config.nextStep = DateTime.Now.AddSeconds(30);
                        //UpdateState(LobbyStates.Waiting);
                    }
                }
            }
        }

        protected void UpdateState(LobbyStates state)
        {
            Config.LobbyState = state;
            Clients.All.broadcastMessage((int)MessageTypes.ChangeState, (int)state);
            if (Config.LobbyState == LobbyStates.DuplicateDrinksResults)
            {
                foreach (var d in Config.DDResult.States)
                {
                    var player = Config.Players.FirstOrDefault(p => p.Id == d.PlayerId);
                    if (player != null)
                    {
                        int add = 0;
                        if ((Config.DDResult.Black && d.AmountBlack > 0) || (!Config.DDResult.Black && d.AmountRed > 0))
                        {
                            add = 3;
                        }
                        else if ((!Config.DDResult.Black && d.AmountBlack > 0) ||
                                 (Config.DDResult.Black && d.AmountRed > 0))
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
                string jsonString = JsonConvert.SerializeObject(Config.DDResult);
                Clients.All.broadcastMessage((int)MessageTypes.LobbyUpdate, jsonString);
                UpdatePlayerBoard();

            }
            else if (Config.LobbyState == LobbyStates.DuplicateDrinks)
            {
                Config.DDResult = new DuplicateDrinksResult();
                Config.DDResult.Black = Random.Next(0, 2) == 0;
                Config.DDResult.States = new List<DuplicateDrinksState>();
                foreach (GameConfig.Player p in Config.Players)
                {
                    if (!p.Active)
                        continue;

                    Config.DDResult.States.Add(new DuplicateDrinksState() {PlayerId = p.Id, PlayerName = p.DisplayName});
                }
            }
            else if (Config.LobbyState == LobbyStates.ShareDrinks)
            {
                
            }
        }

        public void Lobbyupdate(string key, string data)
        {
            GameConfig.Player player = Config.Players.FirstOrDefault(p => p.Key == key);
            if (player != null && player.Active)
            {
                if (Config.LobbyState == LobbyStates.DuplicateDrinks)
                {
                    if (!data.Contains("-"))
                        return;
                    
                    string[] sp = data.Split('-');
                    if (sp.Length != 2)
                        return;
                    
                    int black, red;
                    if (int.TryParse(sp[0], out black) && int.TryParse(sp[1], out red))
                    {
                        var dState = Config.DDResult.States.FirstOrDefault(p => p.PlayerId == player.Id);
                        if (dState == null)
                            return;
                        // Nur Rot oder Schwarz
                        if (black != 0)
                        {
                            dState.AmountBlack = Math.Min(black, Config.DuplicateDrinksMaxAmount);
                        }
                        else
                        {
                            dState.AmountRed = Math.Min(red, Config.DuplicateDrinksMaxAmount);
                        }
                    }
                }
                else if(Config.LobbyState == LobbyStates.ShareDrinks)
                {
                    
                }
            }
        }

        protected void SendMessage(string message)
        {
            Clients.All.broadcastMessage((int) MessageTypes.MessageSmall, message);
        }

        protected void UpdatePlayerBoard()
        {
            string jsonString = JsonConvert.SerializeObject(Config.Players.Where(p => p.Active).OrderByDescending(p => p.OverallPoints));
            Clients.All.broadcastMessage((int)MessageTypes.Leaderboard, jsonString);
        }

    }
    [JsonObject(MemberSerialization.OptIn)]
    public class DuplicateDrinksResult
    {
        [JsonProperty]
        public bool Black { get; set; }

        [JsonProperty]
        public List<DuplicateDrinksState> States { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class DuplicateDrinksState
    {
        [JsonProperty]
        public int PlayerId { get; set; }

        [JsonProperty]
        public string PlayerName { get; set; }

        [JsonProperty]
        public int AmountRed { get; set; }
        [JsonProperty]
        public int AmountBlack { get; set; }
    }
}