namespace Invasion1D.Models;

public interface ICircular
{
    public PointF Position { get; set; }
    public float Diameter { get; init; }
    public float Radius { get; init; }

}