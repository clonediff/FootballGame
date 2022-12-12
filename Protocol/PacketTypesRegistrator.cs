using Protocol.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    public class PacketTypesRegistrator
    {
        public static void RegisterTypes()
        {
            PacketTypeManager.RegisterType(PacketType.Handshake, 0);
            PacketTypeManager.RegisterType(PacketType.TestPacket, 1);
        }
    }
}
