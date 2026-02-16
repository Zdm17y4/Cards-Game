using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Contenedores")]
    public Transform playerHandContainer;
    public Transform tableCardsContainer;

    [Header("Prefab")]
    public GameObject cardPrefab;

    [Header("Textos")]
    public TextMeshProUGUI turnInfoText;
    public TextMeshProUGUI deckCountText;
    public TextMeshProUGUI score1Text;
    public TextMeshProUGUI score2Text;

    [Header("Paneles")]
    public GameObject      messagePanel;
    public TextMeshProUGUI messageText;
    public GameObject      endPanel;
    public TextMeshProUGUI endText;

    [Header("Botones")]
    public Button playButton;
    public Button passButton;
    public Button restartButton;

    const float CARD_W = 160f;

    private System.Action       cbPlay, cbPass, cbRestart;
    private System.Action<Card> cbHandCard, cbTableCard;
    private List<CardView>      handViews  = new List<CardView>();
    private List<CardView>      tableViews = new List<CardView>();
    private CardView            selHand;

    public void Setup(
        System.Action       onPlay,
        System.Action       onPass,
        System.Action       onRestart,
        System.Action<Card> onHandCard,
        System.Action<Card> onTableCard)
    {
        cbPlay=onPlay; cbPass=onPass; cbRestart=onRestart;
        cbHandCard=onHandCard; cbTableCard=onTableCard;
        if (playButton    != null) playButton   .onClick.AddListener(() => cbPlay?   .Invoke());
        if (passButton    != null) passButton   .onClick.AddListener(() => cbPass?   .Invoke());
        if (restartButton != null) restartButton.onClick.AddListener(() => cbRestart?.Invoke());
        if (messagePanel  != null) messagePanel.SetActive(false);
        if (endPanel      != null) endPanel    .SetActive(false);
    }

    // ── Mano ──────────────────────────────────────────────────────────────
    public void UpdateHand(Player player)
    {
        ClearViews(playerHandContainer, handViews);
        var cards = player.Hand.GetAll().ToList();
        int n = cards.Count;
        for (int i = 0; i < n; i++)
        {
            var card = cards[i];
            var view = SpawnCard(card, playerHandContainer, i, n);
            Card c   = card;
            view.OnCardClicked = cv => { SelectHand(cv); cbHandCard?.Invoke(c); };
            handViews.Add(view);
        }
    }

    // ── Mesa ───────────────────────────────────────────────────────────────
    // Ya no necesitamos StackCards — CardView.SetCard detecta IsVirtual y se pinta solo
    public void UpdateTable(CardStack table, HashSet<Card> _ = null)
    {
        ClearViews(tableCardsContainer, tableViews);
        var cards = table.GetAll().ToList();
        int n = cards.Count;
        for (int i = 0; i < n; i++)
        {
            var card = cards[i];
            var view = SpawnCard(card, tableCardsContainer, i, n);
            Card c   = card;
            view.OnCardClicked = cv => cbTableCard?.Invoke(c);
            tableViews.Add(view);
        }
    }

    // ── Info ───────────────────────────────────────────────────────────────
    public void UpdateInfo(int deckCount, string currentName, List<Player> players)
    {
        if (turnInfoText  != null) turnInfoText.text  = $"Turno: {currentName}";
        if (deckCountText != null) deckCountText.text = $"Mazo: {deckCount} cartas";
        if (players != null && players.Count >= 2)
        {
            if (score1Text != null) score1Text.text = $"{players[0].Name}: {players[0].Score} pts";
            if (score2Text != null) score2Text.text = $"{players[1].Name}: {players[1].Score} pts";
        }
    }

    // ── Resaltar ───────────────────────────────────────────────────────────
    public void HighlightPossibleTargets(List<PossiblePlay> plays)
    {
        foreach (var tv in tableViews) tv.SetSelected(false);
        var targets = plays
            .Where(p => p.Type != PlayType.SimplePlay)
            .SelectMany(p => p.TableCards).Distinct().ToList();
        foreach (var tv in tableViews)
            if (targets.Contains(tv.Model)) tv.SetSelected(true);
    }

    // ── Mensajes ───────────────────────────────────────────────────────────
    public void ShowMessage(string msg, bool permanent = false)
    {
        if (messageText  != null) messageText.text = msg;
        if (messagePanel != null) messagePanel.SetActive(true);
        else if (turnInfoText != null) turnInfoText.text = msg;
        if (!permanent) StartCoroutine(HideMsg(3f));
        Debug.Log($"[UI] {msg}");
    }

    IEnumerator HideMsg(float t)
    {
        yield return new WaitForSeconds(t);
        if (messagePanel != null) messagePanel.SetActive(false);
    }

    // ── Fin ────────────────────────────────────────────────────────────────
    public void ShowEndScreen(string msg, List<Player> players)
    {
        if (endPanel == null) { ShowMessage(msg, true); return; }
        endPanel.SetActive(true);
        if (endText != null)
        {
            string full = msg + "\n\n";
            if (players != null)
                foreach (var p in players.OrderByDescending(x => x.Score))
                    full += $"{p.Name}: {p.Score} cartas\n";
            endText.text = full;
        }
    }

    public void HideEndScreen()
    {
        if (endPanel != null) endPanel.SetActive(false);
    }

    // ── SpawnCard centrado ─────────────────────────────────────────────────
    // Fuerza anchor y pivot al centro para ignorar el layout del padre
    CardView SpawnCard(Card card, Transform parent, int i, int n)
    {
        var go   = Instantiate(cardPrefab, parent);
        var view = go.GetComponent<CardView>();
        if (view != null) view.SetCard(card);   // SetCard ya colorea virtual vs normal

        var rt = go.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin        = new Vector2(0.5f, 0.5f);
            rt.anchorMax        = new Vector2(0.5f, 0.5f);
            rt.pivot            = new Vector2(0.5f, 0.5f);
            float offset        = (i - (n - 1) / 2f) * CARD_W;
            rt.anchoredPosition = new Vector2(offset, 0f);
        }
        return view;
    }

    void ClearViews(Transform parent, List<CardView> list)
    {
        foreach (var v in list) if (v != null) Destroy(v.gameObject);
        list.Clear();
    }

    void SelectHand(CardView cv)
    {
        if (selHand != null) selHand.SetSelected(false);
        selHand = cv;
        selHand?.SetSelected(true);
    }
}
