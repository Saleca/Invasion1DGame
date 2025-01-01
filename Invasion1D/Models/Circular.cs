using Invasion1D.Logic;
using Microsoft.Maui.Controls.Shapes;
using System.Diagnostics;

namespace Invasion1D.Models;

public class Circular : Dimension, ICircular
{
    const float TwoPI = 2 * MathF.PI;

    public PointF Position { get; set; }
    public float Diameter { get; init; }
    public float Radius { get; init; }

    public Circular(PointF position, float radius) : base()
    {
        Position = position;
        Radius = radius;
        Diameter = Radius * 2;

        float offset = Radius - strokeThickness;
        body = new Ellipse()
        {
            StrokeThickness = strokeThickness,
            WidthRequest = Diameter,
            HeightRequest = Diameter,

            TranslationX = Position.X - offset,
            TranslationY = Position.Y - offset
        };
        body.SetAppThemeColor(Shape.StrokeProperty, lightTheme, darkTheme);

        Game.Instance.universe.dimensions.Add(this);
    }

    public override PointF GetPositionInShape(float positionPercentage, float radius)
    {
        float angle = positionPercentage * TwoPI;
        float offset = Radius - (strokeThickness / 2);
        return new(
            (offset * MathF.Cos(angle)) + Position.X + (strokeThickness - radius),
            (offset * MathF.Sin(angle)) + Position.Y + (strokeThickness - radius));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="positionPercentageA"></param>
    /// <param name="positionPercentageB"></param>
    /// <param name="direction">direction that character is looking</param>
    /// <returns>arc length between two points</returns>
    public override float GetDistanceBetweenPointsOnShape(float positionPercentageA, float positionPercentageB, bool direction)
    {
        float percentageDistance;
        if (direction) // Clockwise
        {
            percentageDistance = positionPercentageB - positionPercentageA;
        }
        else // Counterclockwise
        {
            percentageDistance = positionPercentageA - positionPercentageB;
        }

        percentageDistance += 1;
        percentageDistance %= 1;

        return GetDistanceFromPercentage(percentageDistance);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="distance">arc length</param>
    /// <returns>angle of arc length</returns>

    public override float GetPercentageFromDistance(float distance) => distance / Radius / TwoPI;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="percentage">angle from 0 to 1</param>
    /// <returns>arc length of the angle</returns>
    /// 
    public override float GetDistanceFromPercentage(float percentage) => percentage * Radius * TwoPI;
}