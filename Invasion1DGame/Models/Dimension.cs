namespace Invasion1DGame.Models
{
	public abstract class Dimension(Universe? universe) : GFX(6, Colors.Black, Colors.White)
	{
		public static readonly List<Dimension> dimensions = [];//
		public Universe? universe = universe;
		public readonly List<Interactive> interactiveObjects = [];

		public event EventHandler? EnemiesEliminated;

		public void AddInteractiveObject(Interactive interactiveObj)
		{
			interactiveObjects.Add(interactiveObj);
		}

		public void RemoveInteractiveObject(Interactive interactiveObj)
		{
			interactiveObjects.Remove(interactiveObj);

			if (interactiveObj is Enemy enemy)
			{
				if (!interactiveObjects.OfType<Enemy>().Any())
				{
					EnemiesEliminated?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		public override void Reset()
		{
			interactiveObjects.Clear();
			toDispose = false;
		}

		public override void Dispose()
		{
			base.Dispose();
			Dimension.dimensions.Remove(this);
		}

		public abstract Point GetPositionInShape(Interactive mobileShape);
		public abstract double GetDistanceBetweenPointsOnShape(double positionA, double positionB, bool clockwise);
		public abstract double GetPercentageFromDistance(double distance);
		public abstract double GetDistanceFromPercentage(double percentage);
	}
}