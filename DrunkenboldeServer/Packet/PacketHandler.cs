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
                    break;
                case PacketType.ChangeScene:
                    return typeof(ChangeScenePacket);
                    break;
                case PacketType.GambleResult:
                    return typeof(GambleResultPacket);
                    break;
                case PacketType.GambleSet:
                    return typeof(GambleSetPacket);
                    break;
                case PacketType.LoginPacketAnswer:
                    return typeof(LoginAnswerPacket);
                    break;
                case PacketType.Message:
                    return typeof(MessagePacket);
                    break;
                case PacketType.PlayerList:
                    return typeof(PlayerListPacket);
                    break;
                case PacketType.ShareSet:
                    return typeof(ShareSetPacket);
                case PacketType.ShareResult:
                    return typeof(ShareResultPacket);
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
        ShareResult = 8
    }
}