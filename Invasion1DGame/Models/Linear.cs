using Invasion1DGame.Helpers;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Shapes;

namespace Invasion1DGame.Models
{
	public class Linear : Dimension
	{
		public Point StartPosition { get; init; }
		public Point EndPosition { get; init; }
		public double Length { get; init; }

		//calculate length of line

		public Linear(Point startPosition, Point endPosition) : base()
		{
			StartPosition = startPosition;
			EndPosition = endPosition;
			Length = GameMath.LineLength(startPosition, endPosition);

			body = new Line()
			{
				StrokeThickness = strokeThickness,
				Margin = 0,
				X1 = StartPosition.X,
				Y1 = StartPosition.Y,

				X2 = EndPosition.X,
				Y2 = EndPosition.Y
			};
			body.SetAppThemeColor(Shape.StrokeProperty, lightTheme, darkTheme);

			MainPage.Instance.universe.dimensions.Add(this);
		}

		public override Point GetPositionInShape(Interactive mobileShape)
		{
			Point position = GameMath.GetPositionInLine(this, mobileShape.PercentageInShape);
			position.X -= mobileShape.Radius;
			position.Y -= mobileShape.Radius;
			return position;
		}

		public override double GetDistanceBetweenPointsOnShape(double positionA, double positionB, bool direction)
		{
			double percentageDistance;
			if (direction) // positive
			{
				percentageDistance = (positionB - positionA + 1) % 1;
			}
			else // negative
			{
				percentageDistance = (positionA - positionB + 1) % 1;
			}

			return GetDistanceFromPercentage(percentageDistance);
		}

		public override double GetPercentageFromDistance(double distance) => distance / Length;

		public override double GetDistanceFromPercentage(double percentage) => percentage * Length;
	}
}