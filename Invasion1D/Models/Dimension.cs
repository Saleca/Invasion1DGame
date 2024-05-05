namespace Invasion1D.Models
{
	public abstract class Dimension() : GFX(6, Colors.Black, Colors.White)
	{
		public readonly List<Interactive> interactiveObjects = [];

		public void AddInteractiveObject(Interactive interactiveObj) =>
			interactiveObjects.Add(interactiveObj);

		public void RemoveInteractiveObject(Interactive interactiveObj) =>
			interactiveObjects.Remove(interactiveObj);

		public void Reset()
		{
			interactiveObjects.Clear();
			toDispose = false;
		}

		public override void Dispose()
		{
			base.Dispose();
			((App)Application.Current!).universe.dimensions.Remove(this);
		}

		public abstract Point GetPositionInShape(double positionPercentage, double halfSize);
		public abstract double GetDistanceBetweenPointsOnShape(double positionA, double positionB, bool clockwise);
		public abstract double GetPercentageFromDistance(double distance);
		public abstract double GetDistanceFromPercentage(double percentage);


		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="halfSize"></param>
		/// <returns>true if available</returns>
		public bool CheckIfPositionIsAvailable(double positionPercentage, double halfSize, out Point? position)
		{
			position = null;
			var sizePercentage = GetPercentageFromDistance(halfSize);
			if (CheckOverlap(sizePercentage, positionPercentage))
			{
				return false;
			}
			position = GetPositionInShape(positionPercentage, halfSize);
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="halfSizePercentage"></param>
		/// <param name="position"></param>
		/// <returns>true if overlap</returns>
		public bool CheckOverlap(double halfSizePercentage, double position)
		{
			double start1 = position - halfSizePercentage;
			double end1 = position + halfSizePercentage;

			foreach (var obj in interactiveObjects)
			{
				double halfSize = obj.sizePercentage / 2;
				double start2 = obj.PositionPercentage - halfSize;
				double end2 = obj.PositionPercentage + halfSize;

				if (start1 < end2 && start2 < end1)
				{
					return true;
				}
			}

			return false;
		}
	}
}