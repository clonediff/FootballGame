using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballLogicLib
{
    public class Game
    {
        // убрать
        public string TeamA { get; set; }
        public string TeamB { get; set; }
        public string TeamAImageSource { get; set; }
        public string TeamBImageSource { get; set; }

        public Dictionary<string, Player> Players { get; }
        public Dictionary<string, int> Score { get; }

        //public Game(string player1Id, string player1Team,
        //            string player2Id, string player2Team)
        //{
        //    var player1 = new Player
        //    {
        //        Id = player1Id,
        //        TeamName = player1Team,
        //        Location = new Point(100, 100)
        //    };

        //    var player2 = new Player
        //    {
        //        Id = player2Id,
        //        TeamName = player2Team,
        //        Location = new Point(500, 100)
        //    };

        //    Players = new()
        //    {
        //        [player1.Id] = player1,
        //        [player2.Id] = player2
        //    };

        //    Score = new()
        //    {
        //        [player1.Id] = 0,
        //        [player2.Id] = 0
        //    };
        //}

        public void MovePlayer(string id, Point direction)
        {
            Players[id].Move(direction);
        }
    }
}
