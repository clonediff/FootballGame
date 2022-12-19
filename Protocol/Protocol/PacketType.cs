using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Protocol
{
    public enum PacketType : byte
    {
        Unknown,
        Connect,
        CantConnect,
        PlayersList
    }
}
