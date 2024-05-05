using Invasion1D.Helpers;
using System.Diagnostics;
using System.Timers;

namespace Invasion1D.Models
{
	public abstract class Kinetic(Dimension dimension, float position, Color color, float speed) : Interactive(dimension, position, color)
	{
		public const bool clockwise = true;

		public float speed = speed;
		public bool
			direction,
			isMoving = false;

		//TODO:modify stepDistance and movementInterval when implementing framerate and delta time
		protected float stepDistance = speed / 10;
		protected TimeSpan movementInterval = TimeSpan.FromMilliseconds(100);

		public float DistanceFromTarget(Interactive target)
		{
			float distance = currentDimension.GetDistanceBetweenPointsOnShape(PositionPercentage, target.PositionPercentage, direction);
			distance -= Size;
			return distance;
		}

		public Interactive? FindInteractive(out float closestTargetDistance, Interactive? ignoreInstance = null, params Type[] ignoreTypes)
		{
			Interactive? closestTarget = null;
			closestTargetDistance = float.MaxValue;

			lock (currentDimension.interactiveObjects)
			{
				foreach (var target in currentDimension.interactiveObjects)
				{
					if (ignoreTypes.Any(t => target.GetType() == t)
						|| ReferenceEquals(target, ignoreInstance))
						continue;

					float distance = DistanceFromTarget(target);

					if (distance < closestTargetDistance)
					{
						closestTargetDistance = distance;
						closestTarget = target;
					}
				}
			}
			return closestTarget;
		}

		public Color? GetView()
		{
			Interactive? target = FindInteractive(out float interactiveDistance, ignoreTypes: typeof(Player));
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

		public abstract void TakeDamage(float damage);
	}
}