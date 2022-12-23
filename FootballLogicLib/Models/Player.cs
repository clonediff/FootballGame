using System.Drawing;

namespace FootballLogicLib.Models
{
    public class Player
    {
        public string Id { get; set; }
        public string TeamName { get; set; }
        public Point Location { get; set; }
        public int Score { get; set; }
    }
}