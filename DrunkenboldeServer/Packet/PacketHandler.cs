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
                case PacketType.PlayerList:
                    return typeof(PlayerListPacket);
                case PacketType.ShareSet:
                    return typeof(ShareSetPacket);
                case PacketType.ShareResult:
                    return typeof(ShareResultPacket);
                case PacketType.SongGuessingSongPacket:
                    return typeof(SongGuessingPacket);
                case PacketType.SongGuessingAnswerPacket:
                    return typeof(SongGuessingAnswerPacket);
                case PacketType.SongGuessingAnswerRightPacket:
                    return typeof(SongGuessingAnswerRightPacket);

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
        PlayerList = 3,
        ChangeScene = 4,
        GambleSet = 5,
        GambleResult = 6,
        ShareSet = 7,
        ShareResult = 8,
        SongGuessingSongPacket = 9,
        SongGuessingAnswerPacket = 10,
        SongGuessingAnswerRightPacket = 11,
    }
}