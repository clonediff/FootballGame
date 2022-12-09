using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Protocol
{
    internal static class PacketTypeManager
    {
        static readonly Dictionary<PacketType, (byte, byte)> TypeDictionary = new();

        public static void RegisterType(PacketType type, byte btype, byte bsubType)
        {
            if (TypeDictionary.ContainsKey(type))
                throw new Exception($"Packet type {type:G} is already registered.");

            TypeDictionary[type] = (btype, bsubType);
        }

        public static (byte, byte) GetType(PacketType type)
        {
            if (!TypeDictionary.ContainsKey(type))
                throw new Exception($"Packet type {type:G} is not registered.");

            return TypeDictionary[type];
        }

        public static PacketType GetTypeFromPacket(Packet packet)
        {
            var packetType = packet.PacketType;
            var packetSubType = packet.PacketSubtype;

            foreach (var (myPacketType, (type, subType)) in TypeDictionary)
                if (type == packetType && subType == packetSubType)
                    return myPacketType;

            return PacketType.Unknown;
        }
    }
}
