using Invasion1D.Data;
using Microsoft.Maui.Controls.Shapes;

namespace Invasion1D.Models;

public abstract class Interactive : GFX, ICircular
{
    protected bool disposed = false;

    public float Diameter { get; init; }
    public float Radius { get; init; }

    public PointF Position { get; set; }

    public float diameterPercentage;
    public float radiusPercentage;

    public Dimension currentDimension = null!;

    float positionPercentage;
    public float PositionPercentage
    {
        get => positionPercentage;
        set => positionPercentage = (value + 1) % 1;
    }

    public Interactive(Dimension dimension, float positionPercentage, Color color)
        : base(0, color, color)
    {
        Radius = Stats.interactiveObjectRadius;
        Diameter = Radius * 2;

        GoToDimension(dimension, positionPercentage);

        body = new Ellipse()
        {
            StrokeThickness = strokeThickness,
            WidthRequest = Diameter,
            HeightRequest = Diameter,
            TranslationX = Position.X,
            TranslationY = Position.Y,
            ZIndex = 1
        };

        body.SetAppThemeColor(Shape.StrokeProperty, lightTheme, darkTheme);
        body.SetAppThemeColor(Shape.FillProperty, lightTheme, darkTheme);
    }

    public void GoToDimension(Dimension dimension, float positionPercentage)
    {
        currentDimension = dimension;
        dimension.AddInteractiveObject(this);
        diameterPercentage = dimension.GetPercentageFromDistance(Diameter);
        radiusPercentage = diameterPercentage / 2;

        PositionPercentage = positionPercentage;
        Position = dimension.GetPositionInShape(this.positionPercentage, Radius);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="offsetPercentage">positive values move clockwise</param>
    public void MovePositionByPercentage(float offsetPercentage)
    {
        PositionPercentage += offsetPercentage;
        Position = currentDimension.GetPositionInShape(positionPercentage, Radius);
    }

    public override void Dispose()
    {
        disposed = true;

        base.Dispose();
        currentDimension.RemoveInteractiveObject(this);
    }
}