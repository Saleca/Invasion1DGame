using Microsoft.Maui.Controls.Shapes;

namespace Invasion1D.Models
{
	public class Circular : Dimension, ICircular
	{
		const float TwoPI = 2 * MathF.PI;

		public PointF Position { get; set; }
		public float Size { get; init; }

		public Circular(PointF position, float radius) : base()
		{
			Position = position;
			Size = radius;

			float circleDiameter = Size * 2;
			float circleOffset = Size - strokeThickness;
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

		public override PointF GetPositionInShape(float positionPercentage, float halfSize)
		{
			float angle = positionPercentage * TwoPI;
			float offset = Size - strokeThickness / 2;
			return new(
				(offset * MathF.Cos(angle)) + Position.X + strokeThickness - halfSize,
				(offset * MathF.Sin(angle)) + Position.Y + strokeThickness - halfSize);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="angleA"></param>
		/// <param name="angleB"></param>
		/// <param name="direction">direction that character is looking</param>
		/// <returns>arc length between two points</returns>
		public override float GetDistanceBetweenPointsOnShape(float angleA, float angleB, bool direction)
		{
			float angleDiff;
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
		public override float GetPercentageFromDistance(float distance) => distance / Size / TwoPI;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="percentage">angle from 0 to 1</param>
		/// <returns>arc length of the angle</returns>
		public override float GetDistanceFromPercentage(float percentage) => percentage * Size * TwoPI;
	}
}