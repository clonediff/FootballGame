﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Protocol
{
    public class PacketField
    {
        public byte FieldID { get; set; }
        public byte FieldSize { get; set; }
        public byte[] Contents { get; set; }
    }
}
