﻿using FootballLogicLib;
using Protocol.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Packets
{
    public class GameStart
    {
        [Field(0)]
        public Player Player1;

        [Field(1)]
        public Player Player2;
    }
}
