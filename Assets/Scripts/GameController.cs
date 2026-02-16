using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Referencias")]
    public UIManager uiManager;

    [Header("Configuracion")]
    public string       matchName    = "ANGULOS";
    public AIDifficulty aiDifficulty = AIDifficulty.Medium;
    [Range(0.5f, 4f)]
    public float        aiThinkTime  = 1.5f;

    GameManager gm;
    AIPlayer    ai;
    Card        selectedHandCard;

    void Start()
    {
        if (uiManager == null) { Debug.LogError("Asigna UIManager en Inspector!"); return; }
        InitGame();
    }

    void InitGame()
    {
        selectedHandCard = null;
        gm = new GameManager(matchName);
        gm.AddPlayer("Jugador", 0, isAI: false);
        gm.AddPlayer("CPU",     1, isAI: true);

        gm.OnTurnChanged  += OnTurnChanged;
        gm.OnPlayExecuted += OnPlayExecuted;
        gm.OnTableChanged += () => RefreshUI();
        gm.OnGameOver     += OnGameOver;

        ai = GetComponent<AIPlayer>() ?? gameObject.AddComponent<AIPlayer>();
        ai.Difficulty   = aiDifficulty;
        ai.ThinkingTime = aiThinkTime;
        ai.Setup(gm, () => RefreshUI());

        uiManager.Setup(
            onPlay:      OnPlayButton,
            onPass:      OnPassButton,
            onRestart:   () => RestartGame(),
            onHandCard:  OnHandCardSelected,
            onTableCard: OnTableCardSelected
        );

        gm.StartGame();
        RefreshUI();
    }

    void OnTurnChanged(Player player)
    {
        RefreshUI();
        if (player.IsAI && player.Hand.Count > 0)
            ai.TakeTurn(player);
        else if (player.IsAI && player.Hand.Count == 0)
            uiManager.ShowMessage("CPU sin cartas - tu turno");
    }

    void OnPlayExecuted(Player player, PlayResult result)
    {
        uiManager.ShowMessage(result.Message);
    }

    void OnGameOver(Player winner)
    {
        string msg = winner != null
            ? $"Gana {winner.Name} con {winner.Score} cartas!"
            : "Empate!";
        uiManager.ShowEndScreen(msg, gm.State.Players);
    }

    void OnHandCardSelected(Card card)
    {
        if (IsAITurn()) return;
        selectedHandCard = card;
        var plays = gm.GetPlaysFor(card, gm.State.CurrentPlayer);
        uiManager.HighlightPossibleTargets(plays);

        var capts  = plays.Where(p => p.Type == PlayType.DirectCapture).ToList();
        var builds = plays.Where(p => p.Type == PlayType.BuildCapture).ToList();

        string hint = $"Carta: {card.ShortName} ({card.Suit})";
        if (capts.Count  > 0) hint += "  |  CAPTURA disponible!";
        if (builds.Count > 0) hint += $"  |  CONSTRUCCION posible con {builds.Count} carta(s)";
        uiManager.ShowMessage(hint);
    }

    void OnTableCardSelected(Card tableCard)
    {
        if (IsAITurn() || selectedHandCard == null)
        {
            if (selectedHandCard == null)
                uiManager.ShowMessage("Primero selecciona una carta de tu mano");
            return;
        }

        var plays = gm.GetPlaysFor(selectedHandCard, gm.State.CurrentPlayer);

        // Preferir captura directa, luego construccion
        var direct = plays.FirstOrDefault(p =>
            p.Type == PlayType.DirectCapture && p.TableCards.Contains(tableCard));
        var build = plays.FirstOrDefault(p =>
            p.Type == PlayType.BuildCapture && p.TableCards.Contains(tableCard));

        var chosen = direct ?? build;
        if (chosen != null)
        {
            ExecuteAndRefresh(chosen);
        }
        else
        {
            // Mensaje explicativo
            if (tableCard.IsVirtual)
            {
                uiManager.ShowMessage(
                    $"Para capturar esa construccion necesitas una carta de {tableCard.AngleDegrees}deg");
            }
            else if (tableCard.Suit != selectedHandCard.Suit)
            {
                int sum = (selectedHandCard.AngleDegrees + tableCard.AngleDegrees) % 360;
                uiManager.ShowMessage(
                    $"Para construir: necesitas mismo palo " +
                    $"({selectedHandCard.Suit} y {tableCard.Suit} son distintos). " +
                    $"Si quieres capturar, necesitas {tableCard.AngleDegrees}deg en mano.");
            }
            else
            {
                int sum = (selectedHandCard.AngleDegrees + tableCard.AngleDegrees) % 360;
                uiManager.ShowMessage(
                    $"Para construir {selectedHandCard.ShortName}+{tableCard.ShortName}={sum}deg " +
                    $"necesitas tener {sum}deg en mano para poder capturarla despues");
            }
        }
    }

    void OnPlayButton()
    {
        if (IsAITurn() || selectedHandCard == null)
        {
            if (selectedHandCard == null) uiManager.ShowMessage("Selecciona una carta primero");
            return;
        }
        var plays = gm.GetPlaysFor(selectedHandCard, gm.State.CurrentPlayer);
        ExecuteAndRefresh(plays.OrderByDescending(p => p.Priority).First());
    }

    void OnPassButton()
    {
        if (IsAITurn()) return;
        var human = gm.State.CurrentPlayer;
        bool canCapture = human.Hand.GetAll()
            .Any(c => CaptureLogic.MustCapture(c, gm.State.Table));
        if (canCapture)
        {
            uiManager.ShowMessage("No podes pasar! Tenes capturas directas disponibles.");
            return;
        }
        Card toPlay = selectedHandCard ?? human.Hand.GetAll().FirstOrDefault();
        if (toPlay == null) return;
        ExecuteAndRefresh(new PossiblePlay {
            Type = PlayType.SimplePlay, HandCard = toPlay, TableCards = new List<Card>()
        });
    }

    void ExecuteAndRefresh(PossiblePlay play)
    {
        var result = gm.ExecutePlay(play);
        if (!result.Success) { uiManager.ShowMessage(result.Message); return; }
        selectedHandCard = null;
        RefreshUI();
    }

    public void RefreshUI()
    {
        var s = gm.State;
        uiManager.UpdateHand(s.Players[0]);
        uiManager.UpdateTable(s.Table, s.StackCards);
        uiManager.UpdateInfo(s.Deck.Count, s.CurrentPlayer?.Name ?? "-", s.Players);
    }

    bool IsAITurn() => gm.State.CurrentPlayer?.IsAI == true;

    void RestartGame()
    {
        gm.OnTurnChanged  -= OnTurnChanged;
        gm.OnPlayExecuted -= OnPlayExecuted;
        gm.OnGameOver     -= OnGameOver;
        uiManager.HideEndScreen();
        InitGame();
    }

    void OnGUI()
    {
        if (gm == null) return;
        GUILayout.BeginArea(new Rect(10, 10, 280, 200));
        GUILayout.Box("Debug - ANGULOS");
        GUILayout.Label(gm.GetDebugInfo());
        GUILayout.Label("Construccion: mismo palo + tenes el angulo suma");
        if (GUILayout.Button("Reiniciar")) RestartGame();
        GUILayout.EndArea();
    }
}
