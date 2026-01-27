public class Card
{
    public int Value;               // 1–12
    public CardSuit Suit;            // representación
    public ComplexNumber Complex;    // lógica matemática

    public Card(int value, CardSuit suit, ComplexNumber complex)
    {
        Value = value;
        Suit = suit;
        Complex = complex;
    }
}
