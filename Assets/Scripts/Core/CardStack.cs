using System.Collections.Generic;

public class CardStack
{
    private List<Card> cards = new List<Card>();
    public string Name { get; set; }

    public CardStack(string name = "Stack") { Name = name; }

    //  Agregar 
    public void Add(Card card)
    {
        if (card != null) cards.Add(card);
    }

    public void AddRange(IEnumerable<Card> toAdd)
    {
        foreach (var c in toAdd) Add(c);
    }

    //  Sacar cartas 
    public bool TryDrawTop(out Card card)
    {
        card = null;
        if (cards.Count == 0) return false;
        card = cards[0];
        cards.RemoveAt(0);
        return true;
    }

    // Sacar carta específica 
    public bool Remove(Card card)
    {
        return cards.Remove(card);
    }

    // Leer
    public IReadOnlyList<Card> GetAll() => cards.AsReadOnly();

    public int Count => cards.Count;

    // (Fisher-Yates)
    public void Shuffle()
    {
        var rng = new System.Random();
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j    = rng.Next(i + 1);
            var tmp  = cards[i];
            cards[i] = cards[j];
            cards[j] = tmp;
        }
    }

    public void Clear() => cards.Clear();
}
