namespace Invasion1D.Helpers
{
	internal static class GameColors
	{
		internal static Color LinearInterpolation(Color a, Color b, float i)
		{
			float
				red = GameMath.LinearInterpolation(a.Red, b.Red, i),
				green = GameMath.LinearInterpolation(a.Green, b.Green, i),
				blue = GameMath.LinearInterpolation(a.Blue, b.Blue, i);

			return new Color(red, green, blue);
		}

		internal static Color? CalculateView(double distance, Color? color)
		{
			if (color is null) return null;

			Color? viewColor = null;

			double
				minDistance = 1,
				maxDistance = 50;

			if (distance < maxDistance)
			{
				float i = GameMath.Normalize(minDistance, maxDistance, distance);
				viewColor = LinearInterpolation(color, MainPage.VoidColor, i);
			}
			return viewColor;
		}
	}
}