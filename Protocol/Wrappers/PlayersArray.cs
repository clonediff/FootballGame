using FootballLogicLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Wrappers
{
    public struct PlayersArray
    {
        public Player[] Value;

        public static implicit operator PlayersArray(Player[] value)
            => new PlayersArray { Value = value };

        public static implicit operator Player[](PlayersArray value)
            => value.Value;
    }
}
