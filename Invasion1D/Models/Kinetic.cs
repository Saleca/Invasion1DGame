using Invasion1D.Helpers;
using System.Timers;

namespace Invasion1D.Models
{
	public abstract class Kinetic(Dimension dimension, double position, Color color, double speed) : Interactive(dimension, position, color)
	{
		public const bool clockwise = true;
		protected CancellationTokenSource cancelMovement = null!;

		public double speed = speed;
		public bool direction;

		//TODO:modify stepDistance and movementInterval when implementing framerate and delta time
		protected double stepDistance = speed / 10;
		protected TimeSpan movementInterval = TimeSpan.FromMilliseconds(100);

		public double DistanceFromTarget(Interactive target)
		{
			//double characterOffset = Radius + target.Radius;
			double distance = CurrentDimention.GetDistanceBetweenPointsOnShape(PercentageInShape, target.PercentageInShape, direction);
			distance -= Radius * 2;
			return distance;
		}

		public Interactive? FindInteractive(out double closestTargetDistance, Interactive? ignoreInstance = null, params Type[] ignoreTypes)
		{
			Interactive? closestTarget = null;
			closestTargetDistance = double.MaxValue;

			foreach (var target in CurrentDimention.interactiveObjects)
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
			if (cancelMovement is not null && !cancelMovement.IsCancellationRequested)
			{
				StopMovement();
			}
			cancelMovement = new();
			_ = MoveAsync(!clockwise);
		}

		public void PositiveMove()
		{
			if (cancelMovement is not null && !cancelMovement.IsCancellationRequested)
			{
				StopMovement();
			}
			cancelMovement = new();
			_ = MoveAsync(clockwise);
		}

		public void StopMovement()
		{
			if (cancelMovement is not null &&
				!cancelMovement.IsCancellationRequested)
			{
				cancelMovement.Cancel();
				cancelMovement.Dispose();
			}
		}

		protected abstract Task MoveAsync(bool direction);

		public abstract void TakeDamage(double damage);
	}
}