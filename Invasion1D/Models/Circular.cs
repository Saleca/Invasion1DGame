using Invasion1D.Logic;
using Microsoft.Maui.Controls.Shapes;

namespace Invasion1D.Models;

public class Circular : Dimension, ICircular
{
    const float TwoPI = 2 * MathF.PI;

    public PointF Position { get; set; }
    public float Radius { get; init; }

    public Circular(PointF position, float radius) : base()
    {
        Position = position;
        //TODO
        //Radius is being treated as diameter sometimes
        Radius = radius;

        float diameter = Radius * 2;
        float offset = Radius - strokeThickness;
        body = new Ellipse()
        {
            StrokeThickness = strokeThickness,
            Margin = 0,

            WidthRequest = diameter,
            HeightRequest = diameter,

            TranslationX = Position.X - offset,
            TranslationY = Position.Y - offset
        };
        body.SetAppThemeColor(Shape.StrokeProperty, lightTheme, darkTheme);

        Game.Instance.universe.dimensions.Add(this);
    }

    public override PointF GetPositionInShape(float positionPercentage, float radius)
    {
        float angle = positionPercentage * TwoPI;
        float offset = Radius - strokeThickness / 2;
        return new(
            (offset * MathF.Cos(angle)) + Position.X + (strokeThickness - radius),
            (offset * MathF.Sin(angle)) + Position.Y + (strokeThickness - radius));
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

    //TODO:
    //Radius might have diameter values
    public override float GetPercentageFromDistance(float distance) => distance / Radius / TwoPI;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="percentage">angle from 0 to 1</param>
    /// <returns>arc length of the angle</returns>
    /// 
    //TODO:
    //Radius might have diameter values
    public override float GetDistanceFromPercentage(float percentage) => percentage * Radius * TwoPI;
}