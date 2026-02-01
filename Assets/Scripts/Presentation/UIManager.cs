using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform handContainer;
    public Transform tableContainer;

    private List<CardView> handCardViews = new List<CardView>();
    private List<CardView> tableCardViews = new List<CardView>();

    private CardView selectedHandCard;
    private List<CardView> selectedTableCards = new List<CardView>();

    public void UpdateHand(Player player)
    {
        foreach (var obj in handCardViews)
            if (obj != null) Destroy(obj.gameObject);
        handCardViews.Clear();

        int index = 0;
        foreach (var card in player.Hand.GetAll())
        {
            GameObject cardObj = Instantiate(cardPrefab, handContainer);
            RectTransform rt = cardObj.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2((index - 2) * 120, 0);
            CardView view = cardObj.GetComponent<CardView>();
            if (view != null)
            {
                view.SetCard(card);
                view.OnCardClicked = OnHandCardClicked;
            }
            handCardViews.Add(view);
            index++;
        }
        Debug.Log($"? MANO: {handCardViews.Count} cartas");
    }

    public void UpdateTable(CardStack table)
    {
        foreach (var obj in tableCardViews)
            if (obj != null) Destroy(obj.gameObject);
        tableCardViews.Clear();

        int index = 0;
        foreach (var card in table.GetAll())
        {
            GameObject cardObj = Instantiate(cardPrefab, tableContainer);
            RectTransform rt = cardObj.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2((index - 1.5f) * 120, 0);
            CardView view = cardObj.GetComponent<CardView>();
            if (view != null)
            {
                view.SetCard(card);
                view.OnCardClicked = OnTableCardClicked;
            }
            tableCardViews.Add(view);
            index++;
        }
        Debug.Log($"? MESA: {tableCardViews.Count} cartas");
    }

    private void OnHandCardClicked(CardView cardView)
    {
        if (selectedHandCard != null)
            selectedHandCard.SetSelected(false);
        selectedHandCard = cardView;
        selectedHandCard.SetSelected(true);
        // Limpiar selección de mesa
        foreach (var t in selectedTableCards)
            t.SetSelected(false);
        selectedTableCards.Clear();
    }

    private void OnTableCardClicked(CardView cardView)
    {
        if (selectedHandCard == null)
            return;
        if (selectedTableCards.Contains(cardView))
        {
            cardView.SetSelected(false);
            selectedTableCards.Remove(cardView);
        }
        else
        {
            cardView.SetSelected(true);
            selectedTableCards.Add(cardView);
        }
    }

    // Lógica para verificar y eliminar cartas
    public System.Action<Card, List<Card>> OnCardsMatched; // Para notificar al GameManager

    public void TryMatchAndRemove()
    {
        if (selectedHandCard == null || selectedTableCards.Count == 0)
            return;

        // Multiplicar los complejos de la mesa
        ComplexNumber result = selectedTableCards[0].Model.Complex;
        for (int i = 1; i < selectedTableCards.Count; i++)
        {
            result = result.Multiply(selectedTableCards[i].Model.Complex);
        }

        // Redondear ángulo al múltiplo de 30 más cercano
        int[] validAngles = {0,30,60,90,120,150,180,210,240,270,300,330};
        int RoundAngle(int angle)
        {
            angle = ((angle % 360) + 360) % 360; // Asegura que esté en [0,360)
            int closest = validAngles[0];
            int minDiff = Mathf.Abs(angle - closest);
            foreach (int a in validAngles)
            {
                int diff = Mathf.Abs(angle - a);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    closest = a;
                }
            }
            return closest;
        }

        int handAngle = RoundAngle(selectedHandCard.Model.Complex.Angle);
        int resultAngle = RoundAngle(result.Angle);

        if (handAngle == resultAngle)
        {
            // Notificar al GameManager para eliminar cartas de los stacks
            OnCardsMatched?.Invoke(selectedHandCard.Model, selectedTableCards.ConvertAll(c => c.Model));

            // Eliminar visualmente
            Destroy(selectedHandCard.gameObject);
            foreach (var t in selectedTableCards)
                Destroy(t.gameObject);

            handCardViews.Remove(selectedHandCard);
            foreach (var t in selectedTableCards)
                tableCardViews.Remove(t);

            selectedHandCard = null;
            selectedTableCards.Clear();

            Debug.Log($"La multiplicación de complejos coincide");
        }
        else
        {
            Debug.Log($"No coincide la multiplicación ");
        }
    }
    // Método público para botón en la UI
    public void OnPlayButtonClicked()
    {
        TryMatchAndRemove();
    }
}