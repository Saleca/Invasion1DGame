using Invasion1D.Models;

namespace Invasion1D.Helpers
{
	internal static class GameMath
	{
		internal static double LinearInterpolation(double start, double end, double position) => start + position * (end - start);
		internal static float LinearInterpolation(float start, float end, float position) => start + position * (end - start);

		internal static double EaseInInterpolation(double start, double end, double position) => LinearInterpolation(start, end, Math.Sqrt(position));
		internal static float EaseInInterpolation(float start, float end, float position) => LinearInterpolation(start, end, MathF.Sqrt(position));

		internal static double LineLength(Point a, Point b) => Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
		internal static Point GetPositionInLine(Linear line, double percentage)
		{
			double x = LinearInterpolation(line.StartPosition.X, line.EndPosition.X, percentage);
			double y = LinearInterpolation(line.StartPosition.Y, line.EndPosition.Y, percentage);
			return new(x, y);
		}

		internal static float Normalize(double min, double max, double x)
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

		internal static double AddPercentage(double a, double b) => (a + b) % 1;
		internal static double SubtractPercentage(double a, double b) => (a - b) % 1;
	}
}