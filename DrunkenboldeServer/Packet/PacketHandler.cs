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
                default:
                    return null;
            }
        }
    }

    public enum PacketType
    {
        LoginPacket = 0,
        Message = 1,
        PlayerList = 2,
        ChangeScene = 3,
    }
}