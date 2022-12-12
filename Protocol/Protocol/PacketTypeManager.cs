using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Protocol
{
    public static class PacketTypeManager
    {
        static readonly Dictionary<PacketType, byte> TypeDictionary = new();

        public static void RegisterType(PacketType type, byte btype)
        {
            if (TypeDictionary.ContainsKey(type))
                throw new Exception($"Packet type {type:G} is already registered.");

            TypeDictionary[type] = btype;
        }

        public static byte GetType(PacketType type)
        {
            if (!TypeDictionary.ContainsKey(type))
                throw new Exception($"Packet type {type:G} is not registered.");

            return TypeDictionary[type];
        }

        public static PacketType GetTypeFromPacket(Packet packet)
        {
            var packetType = packet.PacketType;

            foreach (var (myPacketType, type) in TypeDictionary)
                if (type == packetType)
                    return myPacketType;

            return PacketType.Unknown;
        }
    }
}
