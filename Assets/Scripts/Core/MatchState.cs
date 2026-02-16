using System.Collections.Generic;
using System.Linq;


public class Build
{
    public Card       VirtualCard;   
    public List<Card> SourceCards;  
    public int        TargetAngle;   

    public Build(Card virtualCard, List<Card> sourceCards, int targetAngle)
    {
        VirtualCard  = virtualCard;
        SourceCards  = new List<Card>(sourceCards);
        TargetAngle  = targetAngle;
    }
}

public class MatchState
{
    public string       MatchName    { get; set; }
    public CardStack    Deck         { get; private set; }
    public CardStack    Table        { get; private set; }
    public List<Player> Players      { get; private set; }
    public int          CurrentTurn  { get; set; }
    public bool         IsGameActive { get; set; }
    public Player       LastCapturer { get; set; }

    // Construcciones activas
    private List<Build> _builds = new List<Build>();

    // Cartas virtuales 
    public HashSet<Card> StackCards { get; private set; } = new HashSet<Card>();

    public MatchState(string matchName = "ANGULOS")
    {
        MatchName    = matchName;
        Deck         = new CardStack("Mazo");
        Table        = new CardStack("Mesa");
        Players      = new List<Player>();
        CurrentTurn  = 0;
        IsGameActive = false;
    }

    public Player CurrentPlayer =>
        Players.Count == 0 ? null : Players[CurrentTurn % Players.Count];

    public void AddPlayer(Player p) => Players.Add(p);

    public void NextTurn()
    {
        if (Players.Count > 0)
            CurrentTurn = (CurrentTurn + 1) % Players.Count;
    }

    // Registrar construccion
    
    public void RegisterBuild(Card virtualCard, List<Card> sourceCards, int targetAngle)
    {
        _builds.Add(new Build(virtualCard, sourceCards, targetAngle));
        StackCards.Add(virtualCard);
    }

    // Dado una carta que esta en mesa
    public Build GetBuildForVirtualCard(Card virtualCard)
        => _builds.FirstOrDefault(b => ReferenceEquals(b.VirtualCard, virtualCard));

    // Hay alguna construccion con este targetAngle
    public Build GetBuildWithTarget(int targetAngle)
        => _builds.FirstOrDefault(b => b.TargetAngle == targetAngle);

    public void RemoveBuild(Build build)
    {
        StackCards.Remove(build.VirtualCard);
        _builds.Remove(build);
    }

    public void ClearBuilds()
    {
        foreach (var b in _builds) StackCards.Remove(b.VirtualCard);
        _builds.Clear();
    }
}
