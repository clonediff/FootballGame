using Protocol.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballLogicLib.Models;

namespace Protocol.Packets
{
    public class PlayersList
    {
        [Field(0)]
        public Player[] Players;
    }
}
