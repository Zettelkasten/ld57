public class Helpers
{
    public static float EaseInOutQuad(float x)
    {
        return x < 0.5 ? 2 * x * x : 1 - System.MathF.Pow(-2 * x + 2, 2) / 2;
    }
}
