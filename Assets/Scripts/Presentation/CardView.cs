using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardView : MonoBehaviour
{
    [Header("Referencias del prefab")]
    public Image           cardBackground;
    public TextMeshProUGUI angleText;
    public TextMeshProUGUI representationText;
    public TextMeshProUGUI suitSymbolText;
    public Image           selectionGlow;

    [Header("Colores por palo")]
    public Color cartesianColor = new Color(0.2f, 0.6f, 1f);
    public Color polarColor     = new Color(1f,   0.5f, 0.2f);
    public Color graphicColor   = new Color(0.3f, 0.85f,0.3f);
    public Color matrixColor    = new Color(1f,   0.3f, 0.3f);
    public Color complexColor   = new Color(0.8f, 0.3f, 1f);

    // Carta virtual (construccion) siempre amarilla
    static readonly Color VIRTUAL_COLOR = new Color(1f, 0.85f, 0f, 1f);

    public float hoverScale = 1.1f;

    public Card  Model      { get; private set; }
    public bool  IsSelected { get; private set; }

    public System.Action<CardView> OnCardClicked;

    public void OnClick() => OnCardClicked?.Invoke(this);

    public void SetCard(Card card)
    {
        Model = card;

        if (angleText != null)
            angleText.text = card.AngleDegrees + "deg";

        if (representationText != null)
            representationText.text = card.GetRepresentation();

        if (suitSymbolText != null)
            suitSymbolText.text = card.IsVirtual ? "[+]" : SuitLabel(card.Suit);

        if (cardBackground != null)
            cardBackground.color = card.IsVirtual ? VIRTUAL_COLOR : SuitColor(card.Suit);

        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        IsSelected = selected;
        if (selectionGlow != null)
            selectionGlow.gameObject.SetActive(selected);
        else
            transform.localScale = selected ? Vector3.one * hoverScale : Vector3.one;
    }

    static string SuitLabel(CardSuit s)
    {
        switch (s)
        {
            case CardSuit.Cartesian: return "Cart";
            case CardSuit.Polar:     return "Pol";
            case CardSuit.Graphic:   return "Graf";
            case CardSuit.Matrix:    return "Mat";
            default:                 return "Comp";
        }
    }

    Color SuitColor(CardSuit s)
    {
        switch (s)
        {
            case CardSuit.Cartesian: return cartesianColor;
            case CardSuit.Polar:     return polarColor;
            case CardSuit.Graphic:   return graphicColor;
            case CardSuit.Matrix:    return matrixColor;
            default:                 return complexColor;
        }
    }
}
