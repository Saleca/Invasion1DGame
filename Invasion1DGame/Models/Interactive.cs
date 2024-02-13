using Microsoft.Maui.Controls.Shapes;

namespace Invasion1DGame.Models
{
	public abstract class Interactive : GFX, ICircular
	{
		public double Radius { get; init; }
		public Point Position { get; set; }

		internal double percentageOffset;

		private Dimension currentDimention = null!;
		public Dimension CurrentDimention
		{
			get => currentDimention;
			set
			{
				currentDimention = value;
				CurrentDimention.AddInteractiveObject(this);
				percentageOffset = CurrentDimention.GetPercentageFromDistance(Radius * 2);
			}
		}

		double percentageInShape;
		public double PositionPercentage
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
			CurrentDimention = dimension;
			Radius = 5;
			PositionPercentage = positionPercentage;

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

		public abstract void TakeDamage(double damage);

		public override void Reset()
		{
			throw new NotImplementedException();
		}

		public override void Dispose()
		{
			base.Dispose();
			CurrentDimention.RemoveInteractiveObject(this);
		}
	}
}