using Invasion1DGame.Helpers;

namespace Invasion1DGame.Models
{
	public abstract class Kinetic(Dimension dimension, double position, Color color, double speed) : Interactive(dimension, position, color)
	{
		public const bool clockwise = true;

		public double speed = speed;
		public bool direction;

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

		protected abstract bool IsPositiveTouching { get; set; }
		public abstract void PositiveMove();

		protected abstract bool IsNegativeTouching { get; set; }
		public abstract void NegativeMove();

		public abstract void TakeDamage(double damage);
	}
}