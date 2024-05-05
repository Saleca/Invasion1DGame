namespace Invasion1D.Models
{
	public abstract class Dimension() : GFX(6, Colors.Black, Colors.White)
	{
		public readonly List<Interactive> interactiveObjects = [];

		public void AddInteractiveObject(Interactive interactiveObj)
		{
			lock (interactiveObjects)
			{
				interactiveObjects.Add(interactiveObj);
			}
		}

		public void RemoveInteractiveObject(Interactive interactiveObj)
		{
			lock (interactiveObjects)
			{
				interactiveObjects.Remove(interactiveObj);
			}
		}

		public void Reset()
		{
			lock (interactiveObjects)
			{
				interactiveObjects.Clear();
			}
			toDispose = false;
		}

		public override void Dispose()
		{
			base.Dispose();
			((App)Application.Current!).universe.dimensions.Remove(this);
		}

		public abstract PointF GetPositionInShape(float positionPercentage, float halfSize);
		public abstract float GetDistanceBetweenPointsOnShape(float positionA, float positionB, bool clockwise);
		public abstract float GetPercentageFromDistance(float distance);
		public abstract float GetDistanceFromPercentage(float percentage);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="halfSize"></param>
		/// <returns>true if available</returns>
		public bool CheckIfPositionIsAvailable(float positionPercentage, float halfSize, out PointF? position)
		{
			position = null;
			float sizePercentage = GetPercentageFromDistance(halfSize);
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
		public bool CheckOverlap(float halfSizePercentage, float position)
		{
			float start1 = position - halfSizePercentage;
			float end1 = position + halfSizePercentage;

			lock (interactiveObjects)
			{
				foreach (var obj in interactiveObjects)
				{
					float halfSize = obj.sizePercentage / 2;
					float start2 = obj.PositionPercentage - halfSize;
					float end2 = obj.PositionPercentage + halfSize;

					if (start1 < end2 && start2 < end1)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}