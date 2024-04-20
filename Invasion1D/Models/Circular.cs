using Microsoft.Maui.Controls.Shapes;

namespace Invasion1D.Models
{
	public class Circular : Dimension, ICircular
	{
		const double TwoPI = 2 * Math.PI;

		public Point Position { get; set; }
		public double Radius { get; init; }

		public Circular(Point position, double radius) : base()
		{
			Position = position;
			Radius = radius;

			double circleDiameter = Radius * 2;
			double circleOffset = Radius - strokeThickness;
			body = new Ellipse()
			{
				StrokeThickness = strokeThickness,
				Margin = 0,

				WidthRequest = circleDiameter,
				HeightRequest = circleDiameter,

				TranslationX = Position.X - circleOffset,
				TranslationY = Position.Y - circleOffset
			};
			body.SetAppThemeColor(Shape.StrokeProperty, lightTheme, darkTheme);

			((App)Application.Current!).universe.dimensions.Add(this);
		}

		public override Point GetPositionInShape(Interactive physicalShape)
		{
			double angle = physicalShape.PercentageInShape * TwoPI;
			double offset = Radius - strokeThickness / 2;
			return new(
				(offset * Math.Cos(angle)) + Position.X + strokeThickness - physicalShape.Radius,
				(offset * Math.Sin(angle)) + Position.Y + strokeThickness - physicalShape.Radius);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="angleA"></param>
		/// <param name="angleB"></param>
		/// <param name="direction">direction that character is looking</param>
		/// <returns>arc length between two points</returns>
		public override double GetDistanceBetweenPointsOnShape(double angleA, double angleB, bool direction)
		{
			double angleDiff;
			if (direction) // Clockwise
			{
				angleDiff = (angleB - angleA + 1) % 1;
			}
			else // Counterclockwise
			{
				angleDiff = (angleA - angleB + 1) % 1;
			}

			return GetDistanceFromPercentage(angleDiff);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="distance">arc length</param>
		/// <returns>angle of arc length</returns>
		public override double GetPercentageFromDistance(double distance) => distance / Radius / TwoPI;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="percentage">angle from 0 to 1</param>
		/// <returns>arc length of the angle</returns>
		public override double GetDistanceFromPercentage(double percentage) => percentage * Radius * TwoPI;
	}
}