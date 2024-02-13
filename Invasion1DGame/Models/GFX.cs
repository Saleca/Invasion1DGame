using Microsoft.Maui.Controls.Shapes;

namespace Invasion1DGame.Models
{
	public abstract class GFX(double strokeThickness, Color lightTheme, Color darkTheme)
	{
		public readonly double strokeThickness = strokeThickness;
		public readonly Color lightTheme = lightTheme;
		public readonly Color darkTheme = darkTheme;

		public bool toDispose = false;
		public Shape body = null!;

		public Color DisplayColor() => Application.Current?.RequestedTheme switch
		{
			AppTheme.Dark => darkTheme,
			AppTheme.Light => lightTheme,
			_ => darkTheme,
		};

		public abstract void Reset();

		public virtual void Dispose()
		{
			MainPage.Instance.RemoveFromView(body);
		}
	}
}