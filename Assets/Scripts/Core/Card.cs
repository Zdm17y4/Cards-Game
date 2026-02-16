using UnityEngine;

public class Card
{
    public int      Value;
    public CardSuit Suit;
    public ComplexNumber Complex;

   
    public bool IsVirtual { get; private set; }

    // Constructor normal 
    public Card(int value, CardSuit suit, ComplexNumber complex)
    {
        Value     = value;
        Suit      = suit;
        Complex   = complex;
        IsVirtual = false;
    }

    // Constructor de carta 
    public Card(int sumAngle, CardSuit suit)
    {
        Value     = -1;
        Suit      = suit;
        Complex   = new ComplexNumber(sumAngle);
        IsVirtual = true;
    }

    public int AngleDegrees => Complex.Angle;

    public string GetRepresentation()
    {
        if (IsVirtual) return $"construccion\n= {AngleDegrees}deg";

        float rad  = AngleDegrees * Mathf.Deg2Rad;
        float cos  = Mathf.Round(Mathf.Cos(rad) * 100f) / 100f;
        float sin  = Mathf.Round(Mathf.Sin(rad) * 100f) / 100f;
        string sgn = sin >= 0 ? "+" : "";

        switch (Suit)
        {
            case CardSuit.Cartesian: return $"{cos} {sgn} {sin}i";
            case CardSuit.Polar:     return $"e^({AngleDegrees}i)";
            case CardSuit.Graphic:   return $"rot {AngleDegrees}";
            case CardSuit.Matrix:    return $"[{cos},{-sin}]\n[{sin},{cos}]";
            default:                 return $"{cos}{sgn}{sin}i";
        }
    }

    public string ShortName => IsVirtual ? $"[{AngleDegrees}deg]" : $"{AngleDegrees}deg";

    public override string ToString() =>
        IsVirtual ? $"[Virtual {AngleDegrees}deg]" : $"{AngleDegrees}deg ({Suit})";
}
