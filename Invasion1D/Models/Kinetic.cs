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

        public float DistanceFromTarget(Interactive target, bool direction)
        {
            float distance = currentDimension.GetDistanceBetweenPointsOnShape(PositionPercentage, target.PositionPercentage, direction);
            distance -= Size;
            return distance;
        }

        public Interactive? FindInteractive(out float closestTargetDistance, bool direction, Interactive? ignoreInstance = null, params Type[] ignoreTypes)
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

                    float distance = DistanceFromTarget(target, direction);

                    if (distance < closestTargetDistance)
                    {
                        closestTargetDistance = distance;
                        closestTarget = target;
                    }
                }
            }
            return closestTarget;
        }

        public void GetView(out Color? view, out Color? backView)
        {
            view = null;
            backView = null;
            Interactive? target = FindInteractive(out float interactiveDistance, direction, ignoreTypes: typeof(PlayerModel));
            if (target != null)
            {
                view = GameColors.CalculateView(interactiveDistance, target?.DisplayColor());
            }
            Interactive? backTarget = FindInteractive(out float interactiveDistanceAtBack, !direction, ignoreTypes: typeof(PlayerModel));
            if (backTarget != null)
            {
                backView = GameColors.CalculateView(interactiveDistanceAtBack, backTarget?.DisplayColor());
            }
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