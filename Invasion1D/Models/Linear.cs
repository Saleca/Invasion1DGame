using Invasion1D.Helpers;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Shapes;

namespace Invasion1D.Models
{
	public class Linear : Dimension
	{
		public PointF StartPosition { get; init; }
		public PointF EndPosition { get; init; }
		public float Length { get; init; }

		public Linear(PointF startPosition, PointF endPosition) : base()
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

			((App)Application.Current!).universe.dimensions.Add(this);
		}

		public override PointF GetPositionInShape(float positionPercentage, float halfSize)
		{
			PointF position = GameMath.GetPositionInLine(this, positionPercentage);
			position.X -= halfSize;
			position.Y -= halfSize;
			return position;
		}

		public override float GetDistanceBetweenPointsOnShape(float positionA, float positionB, bool direction)
		{
			float percentageDistance;
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

		public override float GetPercentageFromDistance(float distance) => distance / Length;

		public override float GetDistanceFromPercentage(float percentage) => percentage * Length;
	}
}