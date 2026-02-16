using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AIDifficulty { Easy, Medium, Hard }

public class AIPlayer : MonoBehaviour
{
    public AIDifficulty Difficulty   = AIDifficulty.Medium;
    [Range(0.5f, 4f)]
    public float        ThinkingTime = 1.2f;

    private GameManager   gm;
    private System.Action onDone;
    private bool          thinking;

    public void Setup(GameManager gameManager, System.Action onFinished)
    {
        gm = gameManager; onDone = onFinished;
    }

    public void TakeTurn(Player aiPlayer)
    {
        if (!thinking) StartCoroutine(Think(aiPlayer));
    }

    IEnumerator Think(Player aiPlayer)
    {
        thinking = true;
        yield return new WaitForSeconds(ThinkingTime + Random.Range(-0.2f, 0.3f));

        if (aiPlayer.Hand.Count == 0)
        {
            Debug.Log("[IA] Sin cartas, pasa turno");
            onDone?.Invoke();
            thinking = false;
            yield break;
        }

        var allPlays = new List<PossiblePlay>();
        foreach (var card in aiPlayer.Hand.GetAll())
            allPlays.AddRange(CaptureLogic.FindPlays(card, gm.State.Table, aiPlayer));

        if (allPlays.Count == 0) { thinking = false; yield break; }

        var chosen = Choose(allPlays);
        var result = gm.ExecutePlay(chosen);
        Debug.Log($"[IA] {result.Message}");
        onDone?.Invoke();
        thinking = false;
    }

    PossiblePlay Choose(List<PossiblePlay> plays)
    {
        var captures = plays.Where(p => p.Type == PlayType.DirectCapture).ToList();
        var builds   = plays.Where(p => p.Type == PlayType.BuildCapture).ToList();
        var simples  = plays.Where(p => p.Type == PlayType.SimplePlay).ToList();

        switch (Difficulty)
        {
            case AIDifficulty.Easy:
                if (Random.value < 0.5f && captures.Count > 0)
                    return captures[Random.Range(0, captures.Count)];
                return plays[Random.Range(0, plays.Count)];

            case AIDifficulty.Hard:
                if (captures.Count > 0) return captures[Random.Range(0, captures.Count)];
                if (builds.Count   > 0) return builds  [Random.Range(0, builds.Count)];
                return simples.Count > 0 ? simples[0] : plays[0];

            default: // Medium
                if (captures.Count > 0) return captures[Random.Range(0, captures.Count)];
                if (builds.Count > 0 && Random.value > 0.3f)
                    return builds[Random.Range(0, builds.Count)];
                return simples.Count > 0 ? simples[0] : plays[0];
        }
    }
}
