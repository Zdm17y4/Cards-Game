public class ComplexNumber
{
    public int Angle;

    public ComplexNumber(int angle)
    {
        // Normalizar siempre a 0–359
        Angle = ((angle % 360) + 360) % 360;
    }

    // Multiplicar números complejos unitarios = sumar ángulos
    public ComplexNumber Multiply(ComplexNumber other)
    {
        return new ComplexNumber((Angle + other.Angle) % 360);
    }

    public override string ToString() => $"{Angle}deg";
}
