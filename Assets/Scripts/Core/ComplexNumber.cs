public class ComplexNumber
{
    public int Angle; 

    public ComplexNumber(int angle)
    {
        Angle = angle;
    }

    public ComplexNumber Multiply(ComplexNumber other)
    {
        return new ComplexNumber((Angle + other.Angle) % 360);
    }

    public override string ToString()
    {
        return $"{Angle}°";
    }
}