using Microsoft.Maui.Controls.Shapes;
using System.Timers;

namespace Invasion1D.Models
{
	public abstract class Interactive : GFX, ICircular
	{
		protected bool disposed = false;

		public double Radius { get; init; }
		public Point Position { get; set; }

		public double sizePercentage;

		public Dimension currentDimension = null!;

		double positionPercentage;
		public double PositionPercentage
		{
			get => positionPercentage;
			set => positionPercentage = (value + 1) % 1;
		}

		List<System.Timers.Timer?> timers = [];

		public Interactive(Dimension dimension, double positionPercentage, Color color) : base(0, color, color)
		{
			Radius = 5;

			GoToDimension(dimension, positionPercentage);

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

		public void GoToDimension(Dimension dimension, double positionPercentage)
		{
			currentDimension = dimension;
			dimension.AddInteractiveObject(this);
			sizePercentage = dimension.GetPercentageFromDistance(Radius * 2);

			PositionPercentage = positionPercentage;
			Position = dimension.GetPositionInShape(this.positionPercentage, Radius);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="offsetPercentage">positive values move clockwise</param>
		public void MovePositionByPercentage(double offsetPercentage)
		{
			PositionPercentage += offsetPercentage;
			Position = currentDimension.GetPositionInShape(positionPercentage, Radius);
		}

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
			disposed = true;

			base.Dispose();
			currentDimension.RemoveInteractiveObject(this);
		}
	}
}