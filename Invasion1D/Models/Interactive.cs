using Invasion1D.Data;
using Microsoft.Maui.Controls.Shapes;

namespace Invasion1D.Models;

public abstract class Interactive : GFX, ICircular
{
    protected bool disposed = false;

    public float Size { get; init; }
    public PointF Position { get; set; }

    public float sizePercentage;

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
        Size = Stats.interactiveObjectSize;

        GoToDimension(dimension, positionPercentage);

        body = new Ellipse()
        {
            StrokeThickness = strokeThickness,
            Margin = 0,

            WidthRequest = Size,
            HeightRequest = Size,

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
        sizePercentage = dimension.GetPercentageFromDistance(Size);

        PositionPercentage = positionPercentage;
        Position = dimension.GetPositionInShape(this.positionPercentage, Size / 2);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="offsetPercentage">positive values move clockwise</param>
    public void MovePositionByPercentage(float offsetPercentage)
    {
        PositionPercentage += offsetPercentage;
        Position = currentDimension.GetPositionInShape(positionPercentage, Size / 2);
    }

    public override void Dispose()
    {
        disposed = true;

        base.Dispose();
        currentDimension.RemoveInteractiveObject(this);
    }
}