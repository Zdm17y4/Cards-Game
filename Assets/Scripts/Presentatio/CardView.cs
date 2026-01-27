using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardView : MonoBehaviour
{
    public TextMeshProUGUI valueText;
    public Card Model { get; private set; }

    public void SetCard(Card card)
    {
        Model = card;

        if (valueText != null)
        {
            valueText.text = $"{card.Value}\n{card.Suit}";
        }
    }
}