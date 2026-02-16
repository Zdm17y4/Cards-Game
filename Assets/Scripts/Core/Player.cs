// Player.cs  ─  dato puro, SIN MonoBehaviour.
// Se instancia con: new Player("Nombre", 0, false)
using System.Collections.Generic;

public class Player
{
    public string    Name       { get; set; }
    public int       SeatNumber { get; set; }
    public bool      IsAI       { get; set; }
    public CardStack Hand       { get; private set; }
    public CardStack ScorePile  { get; private set; }
    public int       Score      => ScorePile.Count;

    public Player(string name, int seatNumber = 0, bool isAI = false)
    {
        Name       = name;
        SeatNumber = seatNumber;
        IsAI       = isAI;
        Hand       = new CardStack($"Mano de {name}");
        ScorePile  = new CardStack($"Pila de {name}");
    }

    public void AddCardToHand(Card card) => Hand.Add(card);

    public void CaptureCards(List<Card> captured)
    {
        foreach (var c in captured) ScorePile.Add(c);
    }

    public void Reset()
    {
        Hand.Clear();
        ScorePile.Clear();
    }

    public override string ToString() => $"{Name} | Mano:{Hand.Count} | Pts:{Score}";
}
