using Microsoft.Maui.Controls.Shapes;
using System.Timers;

namespace Invasion1D.Models
{
	public abstract class Interactive : GFX, ICircular
	{
		public double Radius { get; init; }
		public Point Position { get; set; }

		internal double sizePercentage;

		private Dimension currentDimention = null!;
		public Dimension CurrentDimention
		{
			get => currentDimention;
			set
			{
				currentDimention = value;
				CurrentDimention.AddInteractiveObject(this);
				sizePercentage = CurrentDimention.GetPercentageFromDistance(Radius * 2);
			}
		}

		double percentageInShape;
		public double PercentageInShape
		{
			get => percentageInShape;
			set
			{
				percentageInShape = (value + 1) % 1;
				Position = CurrentDimention.GetPositionInShape(this);
			}
		}

		public Interactive(Dimension dimension, double positionPercentage, Color color) : base(0, color, color)
		{
			Radius = 5;
			CurrentDimention = dimension;
			PercentageInShape = positionPercentage;

			body = new Ellipse()
			{
				StrokeThickness = strokeThickness,
				Margin = 0,

				WidthRequest = Radius * 2,
				HeightRequest = Radius * 2,

				TranslationX = Position.X,
				TranslationY = Position.Y,
				ZIndex = 1
			};
			body.SetAppThemeColor(Shape.StrokeProperty, lightTheme, darkTheme);
			body.SetAppThemeColor(Shape.FillProperty, lightTheme, darkTheme);
		}

		List<System.Timers.Timer?> timers = [];

		protected System.Timers.Timer SetUpTimer(int miliseconds, Action onElapsed, bool reset = false)
		{
			System.Timers.Timer? timer = new(miliseconds);
			timer.Elapsed += (s, e) => onElapsed();
			timer.AutoReset = reset;
			timers.Add(timer);
			return timer;
		}

		public override void Dispose()
		{
			if (timers.Count != 0)
			{
				for (int i = 0; i < timers.Count; i++)
				{
					if (timers[i] is not null)
					{
						timers[i]?.Stop();
						timers[i]?.Dispose();
						timers[i] = null;
					}
				}
			}

			base.Dispose();
			CurrentDimention.RemoveInteractiveObject(this);
		}
	}
}