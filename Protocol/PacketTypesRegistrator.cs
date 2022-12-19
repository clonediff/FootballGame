using Protocol.Packets;
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
            PacketTypeManager.RegisterType(PacketType.Connect,      1);
            PacketTypeManager.RegisterType(PacketType.CantConnect,  2);
        }
    }
}
