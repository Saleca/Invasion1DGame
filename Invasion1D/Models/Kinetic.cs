using Invasion1D.Helpers;
using System.Diagnostics;
using System.Timers;

namespace Invasion1D.Models
{
	public abstract class Kinetic(Dimension dimension, double position, Color color, double speed) : Interactive(dimension, position, color)
	{
		public const bool clockwise = true;

		public double speed = speed;
		public bool
			direction,
			isMoving = false;

		//TODO:modify stepDistance and movementInterval when implementing framerate and delta time
		protected double stepDistance = speed / 10;
		protected TimeSpan movementInterval = TimeSpan.FromMilliseconds(100);

		public double DistanceFromTarget(Interactive target)
		{
			//double characterOffset = Radius + target.Radius;
			double distance = currentDimension.GetDistanceBetweenPointsOnShape(PositionPercentage, target.PositionPercentage, direction);
			distance -= Radius * 2;
			return distance;
		}

		public Interactive? FindInteractive(out double closestTargetDistance, Interactive? ignoreInstance = null, params Type[] ignoreTypes)
		{
			Interactive? closestTarget = null;
			closestTargetDistance = double.MaxValue;

			foreach (var target in currentDimension.interactiveObjects)
			{
				if (ignoreTypes.Any(t => target.GetType() == t)
					|| ReferenceEquals(target, ignoreInstance))
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
			Interactive? target = FindInteractive(out double interactiveDistance, ignoreTypes: typeof(Player));
			return GameColors.CalculateView(interactiveDistance, target?.DisplayColor());
		}

		public void NegativeMove()
		{
			isMoving = true;
			direction = !clockwise;
		}

		public void PositiveMove()
		{
			isMoving = true;
			direction = clockwise;
		}

		public void StopMovement()
		{
			isMoving = false;
		}

		public abstract void Move();

		public void UpdateUI()
		{
			body.TranslationX = Position.X;
			body.TranslationY = Position.Y;
		}

		public abstract void TakeDamage(double damage);
	}
}