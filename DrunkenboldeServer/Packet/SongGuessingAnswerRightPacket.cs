using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrunkenboldeServer.Packet
{
    public class SongGuessingAnswerRightPacket : JsonPacket
    {
        public override PacketType GetPacketType()
        {
            return PacketType.SongGuessingAnswerRightPacket;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}