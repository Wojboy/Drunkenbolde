﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DrunkenboldeServer.Packet
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ShareSetPacket : JsonPacket
    {
        [JsonProperty]
        public int PlayerId { get; set; }

        [JsonProperty]
        public int Amount { get; set; }

        public override bool IsValid()
        {
            return PlayerId >= 0 && Amount >= 0;
        }

        public override PacketType GetPacketType()
        {
            return PacketType.ShareSet;
        }
    }
}