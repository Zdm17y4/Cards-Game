using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform handContainer;
    public Transform tableContainer;

    private List<GameObject> handCards = new List<GameObject>();
    private List<GameObject> tableCards = new List<GameObject>();

    public void UpdateHand(Player player)
    {
        // Limpiar
        foreach (var obj in handCards)
            if (obj != null) Destroy(obj);
        handCards.Clear();

        // Crear
        int index = 0;
        foreach (var card in player.Hand.GetAll())
        {
            GameObject cardObj = Instantiate(cardPrefab, handContainer);

            RectTransform rt = cardObj.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2((index - 2) * 120, 0);

            CardView view = cardObj.GetComponent<CardView>();
            if (view != null) view.SetCard(card);

            handCards.Add(cardObj);
            index++;
        }

        Debug.Log($"? MANO: {handCards.Count} cartas");
    }

    public void UpdateTable(CardStack table)
    {
        // Limpiar
        foreach (var obj in tableCards)
            if (obj != null) Destroy(obj);
        tableCards.Clear();

        // Crear
        int index = 0;
        foreach (var card in table.GetAll())
        {
            GameObject cardObj = Instantiate(cardPrefab, tableContainer);

            RectTransform rt = cardObj.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2((index - 1.5f) * 120, 0);

            CardView view = cardObj.GetComponent<CardView>();
            if (view != null) view.SetCard(card);

            tableCards.Add(cardObj);
            index++;
        }

        Debug.Log($"? MESA: {tableCards.Count} cartas");
    }
}