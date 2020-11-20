using System;
using Newtonsoft.Json;

namespace DrunkenboldeServer.Packet
{
    public class PacketHandler
    {
        public static JsonPacket DecodePacket(int packetNumber, string payload)
        {
            if (Enum.IsDefined(typeof(PacketType), packetNumber))
            {
                PacketType pType = (PacketType)packetNumber;
                Type t = GetTypeForPacketNumber(pType);
                if (t == null)
                    return null;
                var packet = (JsonPacket) JsonConvert.DeserializeObject(payload, t);
                return !packet.IsValid() ? null : packet;
            }

            return null;
        }

        public static string EncodePacket(JsonPacket packet)
        {
            return !packet.IsValid() ? null : JsonConvert.SerializeObject(packet);
        }

        protected static Type GetTypeForPacketNumber(PacketType packetNumber)
        {
            switch (packetNumber)
            {
                case PacketType.LoginPacket:
                    return typeof(LoginPacket);
                case PacketType.ChangeScene:
                    return typeof(ChangeScenePacket);
                case PacketType.GambleResult:
                    return typeof(GambleResultPacket);
                case PacketType.GambleSet:
                    return typeof(GambleSetPacket);
                case PacketType.LoginPacketAnswer:
                    return typeof(LoginAnswerPacket);
                case PacketType.Message:
                    return typeof(MessagePacket);
                case PacketType.Scoreboard:
                    return typeof(ScoreboardPacket);
                case PacketType.ShareSet:
                    return typeof(ShareSetPacket);
                case PacketType.ShareResult:
                    return typeof(ShareResultPacket);
                case PacketType.WaitingGamesList:
                    return typeof(WaitingGamesListPacket);
                case PacketType.WaitingVote:
                    return typeof(WaitingVotePacket);
                case PacketType.WaitingVoteSelection:
                    return typeof(WaitingVoteSelectionPacket);
                case PacketType.SongGuessingSongPacket:
                    return typeof(SongGuessingPacket);
                case PacketType.SongGuessingAnswerPacket:
                    return typeof(SongGuessingAnswerPacket);
                case PacketType.SongGuessingIsHostPacket:
                    return typeof(SongGuessingIsHostPacket);
                case PacketType.UpdatePlayerList:
                    return typeof(UpdatePlayerListPacket);
                case PacketType.UpdatePlayer:
                    return typeof(UpdatePlayerPacket);
                default:
                    return null;
            }
        }
    }


    public enum PacketType
    {
        LoginPacket = 0,
        LoginPacketAnswer = 1,
        Message = 2,
        Scoreboard = 3,
        ChangeScene = 4,
        GambleSet = 5,
        GambleResult = 6,
        ShareSet = 7,
        ShareResult = 8,
        WaitingGamesList = 9,
        WaitingVote = 10,
        WaitingVoteSelection = 11,
        UpdatePlayer = 12,
        UpdatePlayerList = 13,
        SongGuessingSongPacket = 14,
        SongGuessingAnswerPacket = 15,
        SongGuessingIsHostPacket = 16,


    }
}