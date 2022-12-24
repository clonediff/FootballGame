namespace FootballLogicLib;

public class Lobby
{
    public List<Player> Players { get; set; } = new();
    public Dictionary<string, bool> ReadyPlayers { get; set; } = new();

    public void AddPlayer(Player player)
    {
        Players.Add(player);
        ReadyPlayers[player.Id] = false;
    }

    public void RemovePlayer(Player player)
    {
        Players.Remove(player);
        ReadyPlayers.Remove(player.Id);
    }
}