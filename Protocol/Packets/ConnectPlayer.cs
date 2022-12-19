using Protocol.Protocol;
using Protocol.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Packets
{
    public class ConnectPlayer
    {
        [Field(0)]
        public StringWrapper Id;

        [Field(1)]
        public StringWrapper Team;
    }
}
