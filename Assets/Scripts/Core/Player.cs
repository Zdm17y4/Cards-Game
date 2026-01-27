public class Player
{
    public string Name { get; set; }
    public int SeatNumber { get; set; }
    public CardStack Hand { get; private set; }
    public CardStack ScorePile { get; private set; }
    public int Score { get; set; }
    public bool IsActive { get; set; }

    public Player(string name, int seatNumber = 0)
    {
        Name = name;
        SeatNumber = seatNumber;
        Hand = new CardStack("Hand");
        ScorePile = new CardStack("Score");
        Score = 0;
        IsActive = true;
    }

    public void AddCardToHand(Card card)
    {
        Hand.Add(card);
    }

    public void AddCardToScore(Card card)
    {
        ScorePile.Add(card);
        Score++; // o calcular según tu lógica
    }

    public void ClearHand()
    {
        Hand.Clear();
    }
}