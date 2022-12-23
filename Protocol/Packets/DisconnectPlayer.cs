using Protocol.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Packets
{
    public class DisconnectPlayer
    {
        [Field(0)]
        public string Id;
    }
}
