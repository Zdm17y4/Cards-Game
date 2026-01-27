using System.Collections.Generic;

public class CardStack
{
    private LinkedList<Card> cards = new LinkedList<Card>();
    public string Name { get; set; } 

    public CardStack(string name = "Default")
    {
        Name = name;
    }

    // Agregar carta
    public void Add(Card card)
    {
        if (card != null)
            cards.AddLast(card);
    }

    // Tomar carta del inicio
    public Card DrawTop()
    {
        if (cards.Count == 0)
            throw new System.InvalidOperationException($"No hay cartas en {Name}");
        
        var card = cards.First.Value;
        cards.RemoveFirst();
        return card;
    }

    // Intentar tomar carta (sin excepción)
    public bool TryDrawTop(out Card card)
    {
        card = null;
        if (cards.Count == 0)
            return false;
        
        card = cards.First.Value;
        cards.RemoveFirst();
        return true;
    }

    // Mezclar
    public void Shuffle()
    {
        List<Card> list = new List<Card>(cards);
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
        
        cards.Clear();
        foreach (var card in list)
            cards.AddLast(card);
    }

    // Obtener todas las cartas (sin remover)
    public IEnumerable<Card> GetAll()
    {
        return cards;
    }

    // Limpiar
    public void Clear()
    {
        cards.Clear();
    }

    public int Count => cards.Count;
}
