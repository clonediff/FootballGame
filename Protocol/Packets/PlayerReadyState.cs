using Protocol.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Packets
{
    public class PlayerReadyState
    {
        [Field(0)]
        public string Id;

        [Field(1)]
        public bool IsReady;
    }
}
