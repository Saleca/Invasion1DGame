using Invasion1D.Models;
using Microsoft.Maui.Controls.Shapes;
using System.Numerics;

namespace Invasion1D.Helpers;

internal static class GameMath
{
    internal static float LinearInterpolation(float start, float end, float position) => start + position * (end - start);

    internal static float EaseInInterpolation(float start, float end, float position) => LinearInterpolation(start, end, MathF.Sqrt(position));

    internal static float LineLength(PointF a, PointF b) => MathF.Sqrt(MathF.Pow(b.X - a.X, 2) + MathF.Pow(b.Y - a.Y, 2));
    internal static PointF GetPositionInLine(Linear line, float percentage, float radius)
    {

        float x = LinearInterpolation(line.StartPosition.X, line.EndPosition.X, percentage);
        float y = LinearInterpolation(line.StartPosition.Y, line.EndPosition.Y, percentage);

        /* keeping this for now maybe to use on the line body offset 
        Vector2 start = new(line.StartPosition.X, line.StartPosition.Y);
        Vector2 end = new(line.EndPosition.X, line.EndPosition.Y);
        Vector2 direction = Vector2.Normalize(end - start);
        Vector2 offset = new Vector2(-direction.Y, direction.X) * radius;
       
        x -= Math.Abs(offset.X);
        y -= Math.Abs(offset.Y);
        //*/

        x -= radius;
        y -= radius;
        return new(x, y);
    }

    static void CalculateLineEnds(Line line, out PointF start, out PointF end)
    {
        start = new();
        end = new();


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