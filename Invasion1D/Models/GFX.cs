﻿using Microsoft.Maui.Controls.Shapes;
using System;

namespace Invasion1D.Models
{
	public abstract class GFX(double strokeThickness, Color lightTheme, Color darkTheme)
	{
		static App Game => (App)Application.Current!;
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


		public virtual void Dispose()
		{
			Game.UI.RunOnUIThread(() => Game.UI.RemoveFromMap(body));
		}
	}
}