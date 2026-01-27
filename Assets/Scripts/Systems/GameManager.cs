using System.Collections.Generic;

public class GameManager
{
    public MatchState State { get; private set; }

    public GameManager(string matchName = "New Game")
    {
        State = new MatchState(matchName);
    }

    public void InitializeDeck()
    {
        State.Deck.Clear();

        CardSuit[] suits = {
            CardSuit.Cartesian,
            CardSuit.Polar,
            CardSuit.Graphic,
            CardSuit.Matrix,
            CardSuit.ExpMatrix
        };

        foreach (var suit in suits)
        {
            for (int value = 1; value <= 12; value++)
            {
                int angle = value * 30;
                ComplexNumber complex = new ComplexNumber(angle);
                Card card = new Card(value, suit, complex);
                State.Deck.Add(card);
            }
        }
    }

    public void AddPlayer(string name, int seatNumber)
    {
        Player player = new Player(name, seatNumber);
        State.AddPlayer(player);
    }

    public void StartGame()
    {
        InitializeDeck();
        State.Deck.Shuffle();

        // Repartir 5 cartas al jugador
        foreach (var player in State.Players)
        {
            for (int i = 0; i < 5; i++)
            {
                if (State.Deck.TryDrawTop(out Card card))
                {
                    player.AddCardToHand(card);
                }
            }
        }

        // 4 cartas en la mesa
        for (int i = 0; i < 4; i++)
        {
            if (State.Deck.TryDrawTop(out Card card))
            {
                State.Table.Add(card);
            }
        }
    }
}