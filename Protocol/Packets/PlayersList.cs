using Protocol.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootballLogicLib;

namespace Protocol.Packets
{
    public class PlayersList
    {
        [Field(0)]
        public PlayerIsReadyStruct[] Players;
    }

    public struct PlayerIsReadyStruct
    {
        public Player Player { get; set; }
        public  bool IsReady { get; set; }
    }
}
