namespace FootballLogicLib
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