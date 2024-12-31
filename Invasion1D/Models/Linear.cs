using Invasion1D.Helpers;
using Invasion1D.Logic;
using Microsoft.Maui.Controls.Shapes;

namespace Invasion1D.Models;

public partial class Linear : Dimension
{
    public PointF StartPosition { get; init; }
    public PointF EndPosition { get; init; }
    public float Length { get; init; }

    public Linear(PointF startPosition, PointF endPosition)
        : base()
    {
        StartPosition = startPosition;
        EndPosition = endPosition;
        Length = GameMath.LineLength(startPosition, endPosition);

        float offset = 0;
        float radDir = MathF.Atan2(EndPosition.Y - StartPosition.Y, EndPosition.X - StartPosition.X);
        if (radDir < 0 || radDir > MathF.PI / 2)
        {
            offset = -strokeThickness / 2;
        }

        body = new Line()
        {
            StrokeThickness = strokeThickness,
            X1 = StartPosition.X,
            Y1 = StartPosition.Y,
            X2 = EndPosition.X,
            Y2 = EndPosition.Y,
            TranslationX = offset,
            TranslationY = offset,
        };
        body.SetAppThemeColor(Shape.StrokeProperty, lightTheme, darkTheme);

        Game.Instance.universe.dimensions.Add(this);
    }

    public override PointF GetPositionInShape(float positionPercentage, float radius) =>
        GameMath.GetPositionInLine(this, positionPercentage, radius);

    public override float GetDistanceBetweenPointsOnShape(float positionA, float positionB, bool direction)
    {
        float percentageDistance;
        if (direction) // positive
        {
            percentageDistance = positionB - positionA;
        }
        else // negative
        {
            percentageDistance = positionA - positionB;
        }

        percentageDistance += 1;
        percentageDistance %= 1;

        return GetDistanceFromPercentage(percentageDistance);
    }

    public override float GetPercentageFromDistance(float distance) => distance / Length;

    public override float GetDistanceFromPercentage(float percentage) => percentage * Length;
}