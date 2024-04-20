namespace Invasion1D.Models
{
	public abstract class Dimension() : GFX(6, Colors.Black, Colors.White)
	{
		public readonly List<Interactive> interactiveObjects = [];

		//TODO
		//public event EventHandler? ColapsedDimention;
		//public async void TeleportAsync(object? sender, EventArgs args)
		//dimension.ColapsedDimention += (player)TeleportAsync;
		//ColapsedDimention?.Invoke(this, EventArgs.Empty);

		public void AddInteractiveObject(Interactive interactiveObj) =>
			interactiveObjects.Add(interactiveObj);

		public void RemoveInteractiveObject(Interactive interactiveObj) =>
			interactiveObjects.Remove(interactiveObj);

		public override void Reset()
		{
			interactiveObjects.Clear();
			toDispose = false;
		}

		public override void Dispose()
		{
			base.Dispose();
			((App)Application.Current!).universe.dimensions.Remove(this);
		}

		public abstract Point GetPositionInShape(Interactive interactive);
		public abstract double GetDistanceBetweenPointsOnShape(double positionA, double positionB, bool clockwise);
		public abstract double GetPercentageFromDistance(double distance);
		public abstract double GetDistanceFromPercentage(double percentage);

		public bool CheckOverlap(Interactive interactive)
		{
			double halfSize = interactive.sizePercentage / 2;
			double start1 = interactive.PercentageInShape - halfSize;
			double end1 = interactive.PercentageInShape + halfSize;

			foreach (var obj in interactiveObjects)
			{
				if (obj == interactive)
					continue;

				double halfSize2 = obj.sizePercentage / 2;
				double start2 = obj.PercentageInShape - halfSize2;
				double end2 = obj.PercentageInShape + halfSize2;

				if (start1 < end2 && start2 < end1)
				{
					return true;
				}
			}

			return false;
		}
	}
}