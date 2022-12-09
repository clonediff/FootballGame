using Protocol.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Packets
{
    public class HandshakePacket
    {
        [Field(1)]
        public int MagicHandshakeNumber;
    }
}
