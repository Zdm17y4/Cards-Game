using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager
{
    public MatchState State { get; private set; }

    public System.Action<Player, PlayResult> OnPlayExecuted;
    public System.Action<Player>             OnTurnChanged;
    public System.Action<Player>             OnGameOver;
    public System.Action                     OnTableChanged;

    const int HAND_SIZE   = 4;
    const int TABLE_SIZE  = 4;
    const int NUM_PLAYERS = 2;

    public GameManager(string matchName = "ANGULOS")
        => State = new MatchState(matchName);

    public void AddPlayer(string name, int seat, bool isAI = false)
        => State.AddPlayer(new Player(name, seat, isAI));

    public void StartGame()
    {
        if (State.Players.Count != NUM_PLAYERS)
            { Debug.LogError("Necesitas 2 jugadores."); return; }
        BuildDeck();
        State.Deck.Shuffle();
        DealHands();
        DealTable();
        State.IsGameActive = true;
        State.CurrentTurn  = 0;
        OnTurnChanged?.Invoke(State.CurrentPlayer);
        Debug.Log($"Juego iniciado. Turno: {State.CurrentPlayer?.Name}");
    }

    void BuildDeck()
    {
        State.Deck.Clear();
        CardSuit[] suits = { CardSuit.Cartesian, CardSuit.Polar,
                             CardSuit.Graphic,   CardSuit.Matrix };
        foreach (var s in suits)
            for (int v = 0; v < 12; v++)
                State.Deck.Add(new Card(v, s, new ComplexNumber(v * 30)));
        Debug.Log($"Mazo: {State.Deck.Count} cartas");
    }

    void DealHands()
    {
        foreach (var p in State.Players)
        {
            p.Hand.Clear();
            for (int i = 0; i < HAND_SIZE; i++)
                if (State.Deck.TryDrawTop(out Card c)) p.AddCardToHand(c);
        }
    }

    void DealTable()
    {
        if (State.Table.Count == 0)
            for (int i = 0; i < TABLE_SIZE; i++)
                if (State.Deck.TryDrawTop(out Card c)) State.Table.Add(c);
    }

    public PlayResult ExecutePlay(PossiblePlay play)
    {
        if (!State.IsGameActive)
            return new PlayResult { Success = false, Message = "Juego inactivo" };

        var current = State.CurrentPlayer;

        // Captura obligatoria: si podes capturar, debes hacerlo
        if (play.Type == PlayType.SimplePlay &&
            CaptureLogic.MustCapture(play.HandCard, State.Table))
            return new PlayResult { Success = false,
                Message = "Captura obligatoria! Debes capturar." };

        var result = CaptureLogic.Execute(play, State, current);
        if (!result.Success) return result;

        OnPlayExecuted?.Invoke(current, result);
        OnTableChanged?.Invoke();

        if (IsGameOver()) { EndGame(); return result; }

        // Reposicion cuando AMBOS se quedan sin cartas
        if (State.Players.TrueForAll(p => p.Hand.Count == 0))
            Redeal();

        AdvanceTurn();
        return result;
    }

    void AdvanceTurn()
    {
        State.NextTurn();
        // Si el proximo jugador no tiene cartas y el mazo esta vacio, buscar uno con cartas
        if (State.CurrentPlayer?.Hand.Count == 0 && State.Deck.Count == 0)
        {
            if (State.Players.Any(p => p.Hand.Count > 0))
            {
                for (int i = 0; i < State.Players.Count; i++)
                {
                    State.NextTurn();
                    if (State.CurrentPlayer?.Hand.Count > 0) break;
                }
            }
        }
        OnTurnChanged?.Invoke(State.CurrentPlayer);
    }

    bool IsGameOver()
        => State.Deck.Count == 0 &&
           State.Players.TrueForAll(p => p.Hand.Count == 0);

    void EndGame()
    {
        State.IsGameActive = false;
        if (State.LastCapturer != null && State.Table.Count > 0)
        {
            State.LastCapturer.CaptureCards(new List<Card>(State.Table.GetAll()));
            State.Table.Clear();
        }
        Player winner = null; int max = -1;
        foreach (var p in State.Players)
            if (p.Score > max) { max = p.Score; winner = p; }
        Debug.Log($"Fin. Ganador: {winner?.Name} con {winner?.Score} cartas");
        OnGameOver?.Invoke(winner);
    }

    void Redeal()
    {
        if (State.Deck.Count == 0) return;
        State.ClearBuilds();
        foreach (var p in State.Players)
            for (int i = 0; i < HAND_SIZE; i++)
                if (State.Deck.TryDrawTop(out Card c)) p.AddCardToHand(c);
        Debug.Log("Reposicion completada.");
    }

    public List<PossiblePlay> GetPlaysFor(Card handCard, Player player = null)
        => CaptureLogic.FindPlays(handCard, State.Table, player ?? State.CurrentPlayer);

    public string GetDebugInfo()
    {
        string s = $"Mazo:{State.Deck.Count} Mesa:{State.Table.Count}\n";
        foreach (var p in State.Players)
            s += $"{(p == State.CurrentPlayer ? ">" : " ")} {p.Name} | Mano:{p.Hand.Count} | Pts:{p.Score}\n";
        return s;
    }
}
