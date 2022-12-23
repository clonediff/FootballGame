using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballLogicLib.Models
{
    public class Game
    {
        public Player FirstPlayer { get; }
        public Player SecondPlayer { get; }
        public string Time { get; set; }

        public Game(Player firstPlayer, Player secondPlayer)
        {
            FirstPlayer = firstPlayer;
            SecondPlayer = secondPlayer;
            Time = "3:00";
        }
    }
}
