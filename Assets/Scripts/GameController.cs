using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Referencias UI")]
    public UIManager uiManager;

    [Header("Configuración")]
    public string matchName = "Nueva Partida";
    public int numberOfPlayers = 1;

    private GameManager gameManager;

    void Start()
    {
        if (uiManager == null)
        {
            Debug.LogError("UIManager no está asignado!");
            return;
        }

        InitializeGame();
    }

    void InitializeGame()
    {
        gameManager = new GameManager(matchName);

        for (int i = 0; i < numberOfPlayers; i++)
        {
            gameManager.AddPlayer($"Jugador {i + 1}", i);
        }

        gameManager.StartGame();

        // Actualizar UI
        UpdateUI();

        Debug.Log($"Juego iniciado: {matchName}");
    }

    void UpdateUI()
    {
        Player currentPlayer = gameManager.State.Players[0];

        uiManager.UpdateHand(currentPlayer);
        uiManager.UpdateTable(gameManager.State.Table);
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 150));
        GUILayout.Box("Debug Panel");

        if (gameManager != null && gameManager.State != null)
        {
            GUILayout.Label($"Partida: {gameManager.State.MatchName}");
            GUILayout.Label($"Jugador: {gameManager.State.Players[0].Name}");
            GUILayout.Label($"Cartas en mazo: {gameManager.State.Deck.Count}");
            GUILayout.Label($"Cartas en mesa: {gameManager.State.Table.Count}");
            GUILayout.Label($"Cartas en mano: {gameManager.State.Players[0].Hand.Count}");

            if (GUILayout.Button("Reiniciar"))
                InitializeGame();
        }

        GUILayout.EndArea();
    }
}