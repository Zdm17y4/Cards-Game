using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardView : MonoBehaviour
{
    public TextMeshProUGUI valueText;
    public Card Model { get; private set; }
    public bool IsSelected { get; private set; }
    private Image background;
    public System.Action<CardView> OnCardClicked;

    private void Awake()
    {
        background = GetComponent<Image>();
    }

    public void SetCard(Card card)
    {
        Model = card;
        if (valueText != null)
        {
            valueText.text = $"{card.Value}\n{card.Suit}";
        }
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        IsSelected = selected;
        if (background != null)
            background.color = selected ? Color.yellow : Color.white;
    }

    public void OnClick()
    {
        OnCardClicked?.Invoke(this);
    }
}