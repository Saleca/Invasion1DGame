using Invasion1D.Helpers;

namespace Invasion1D.Models
{
	public abstract class Kinetic(Dimension dimension, double position, Color color, double speed) : Interactive(dimension, position, color)
	{
		public const bool clockwise = true;

		public double speed = speed;
		public bool direction;

		protected double stepDistance = speed/10;//modify when implementing framerate and delta time
		protected TimeSpan movementInterval = TimeSpan.FromMilliseconds(100);

		public double DistanceFromTarget(Interactive target)
		{
			//double characterOffset = Radius + target.Radius;
			double distance = CurrentDimention.GetDistanceBetweenPointsOnShape(PercentageInShape, target.PercentageInShape, direction);
			distance -= Radius * 2;
			return distance;
		}

		public Interactive? FindInteractive(out double closestTargetDistance, params Type[] ignoreTypes)
		{
			Interactive? closestTarget = null;
			closestTargetDistance = double.MaxValue;

			foreach (var target in CurrentDimention.interactiveObjects)
			{
				if (ignoreTypes.Any(t => target.GetType() == t))
					continue;

				double distance = DistanceFromTarget(target);

				if (distance < closestTargetDistance)
				{
					closestTargetDistance = distance;
					closestTarget = target;
				}
			}
			return closestTarget;
		}

		public Color? GetView()
		{
			Interactive? target = FindInteractive(out double interactiveDistance, typeof(Player));
			return GameColors.CalculateView(interactiveDistance, target?.DisplayColor());
		}

		public abstract void PositiveMove();
		public abstract void NegativeMove();
		public abstract void StopMovement();

		public abstract void TakeDamage(double damage);
	}
}