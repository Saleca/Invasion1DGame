using Microsoft.Maui.Controls.Shapes;

using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace Invasion1D.Helpers
{
	internal static class Geometry
	{
		internal static Path DrawCurvedLink(PointF start, IEnumerable<Point> segments, Brush brush, int strokeThickness)
		{
			PathGeometry connectorGeometry = new()
			{
				Figures = 
				[
					new PathFigure()
					{
						StartPoint = start,
						Segments =
						[
							new PolyBezierSegment(points: (PointCollection)segments)
						]
					}
				]
			};

			Path connector = new()
			{
				Stroke = brush,
				StrokeThickness = strokeThickness,
				Data = connectorGeometry
			};

			return connector;
		}
	}
}