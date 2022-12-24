using FootballLogicLib;
using Protocol.Protocol;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Packets
{
    public class MovePlayer
    {
        [Field(0)]
        public string Id;

        [Field(1)]
        public Direction Direction;

        [Field(2)]
        public int PlayerTranslation;

        [Field(3)]
        public int BallTranslation;
    }
}
