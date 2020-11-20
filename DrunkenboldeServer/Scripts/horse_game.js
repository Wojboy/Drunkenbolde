class HorseGame extends GamePrototype {
    /*  this.ParentElement = parentElement;
        this.Game = game;
        this.PlayerId = playerId;
        this.PlayerName = playerName;
        this.PlayerTable = playerTable;


        Zum Senden von Paketen
        SendPacket(messageType, data)
     */

    Init() {
        $(this.ParentElement).html("test");
    }

    PacketReceived(messageType, messageData) {
    
    }

    GameEnded() {

    }
}