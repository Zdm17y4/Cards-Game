using System.Collections.Generic;

public class MatchState
{
    public string MatchName { get; set; }
    public CardStack Deck { get; private set; }
    public CardStack Table { get; private set; }
    public CardStack DiscardPile { get; private set; }
    public List<Player> Players { get; private set; }
    public int CurrentTurn { get; set; }
    public bool IsGameActive { get; set; }
    public int RoundNumber { get; set; }

    public MatchState(string matchName = "Default Match")
    {
        MatchName = matchName;
        Deck = new CardStack("Deck");
        Table = new CardStack("Table");
        DiscardPile = new CardStack("Discard");
        Players = new List<Player>();
        CurrentTurn = 0;
        IsGameActive = false;
        RoundNumber = 0;
    }

    public Player CurrentPlayer
    {
        get
        {
            if (Players.Count == 0)
                return null;
            return Players[CurrentTurn];
        }
    }

    public void AddPlayer(Player player)
    {
        Players.Add(player);
    }

    public void NextTurn()
    {
        if (Players.Count > 0)
            CurrentTurn = (CurrentTurn + 1) % Players.Count;
    }
}