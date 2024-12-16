using Invasion1D.Models;

namespace Invasion1D.Helpers;

internal static class GameMath
{
    internal static float LinearInterpolation(float start, float end, float position) => start + position * (end - start);

    internal static float EaseInInterpolation(float start, float end, float position) => LinearInterpolation(start, end, MathF.Sqrt(position));

    internal static float LineLength(PointF a, PointF b) => MathF.Sqrt(MathF.Pow(b.X - a.X, 2) + MathF.Pow(b.Y - a.Y, 2));
    internal static PointF GetPositionInLine(Linear line, float percentage)
    {
        float x = LinearInterpolation(line.StartPosition.X, line.EndPosition.X, percentage);
        float y = LinearInterpolation(line.StartPosition.Y, line.EndPosition.Y, percentage);
        return new(x, y);
    }

    internal static float Normalize(float min, float max, float x)
    {
        if (x <= min)
        {
            return 0;
        }
        else if (x >= max)
        {
            return 1;
        }
        else
        {
            return (float)(x / max);
        }
    }

    internal static float AddPercentage(float a, float b) => (a + b) % 1;
    internal static float SubtractPercentage(float a, float b) => (a - b) % 1;
}