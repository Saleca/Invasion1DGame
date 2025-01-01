using Invasion1D.Helpers;
using System.Diagnostics;

namespace Invasion1D.Models;

public abstract class Kinetic(Dimension dimension, float position, Color color, float stepDistance)
    : Interactive(dimension, position, color)
{
    public bool
        direction,
        isMoving = false;

    protected float stepDistance = stepDistance;

    public float DistanceFromTarget(Interactive target, bool direction)
    {
        float marginPosition = PositionPercentage + (direction ? radiusPercentage : -radiusPercentage);
        float targetMarginPosition = target.PositionPercentage + (direction ? -target.radiusPercentage : target.radiusPercentage);
        float distance = currentDimension.GetDistanceBetweenPointsOnShape(marginPosition, targetMarginPosition, direction);
        return distance;
    }

    public Interactive? FindInteractive(out float targetDistance, bool direction, Interactive? ignoreInstance = null, params Type[] ignoreTypes)
    {
        Interactive? target = null;
        targetDistance = float.MaxValue;

        lock (currentDimension.interactiveObjects)
        {
            foreach (var interactiveObject in currentDimension.interactiveObjects)
            {
                if (ignoreTypes.Any(t => interactiveObject.GetType() == t)
                    || ReferenceEquals(interactiveObject, ignoreInstance))
                {
                    continue;
                }

                //*
                if (this is Bullet bullet && currentDimension.CheckOverlap(bullet, interactiveObject))
                {
                    targetDistance = 0;
                    return interactiveObject;
                }
                //*/

                float distance = DistanceFromTarget(interactiveObject, direction);

                if (distance < targetDistance)
                {
                    targetDistance = distance;
                    target = interactiveObject;
                }
            }
        }
        return target;
    }

    public void GetView(out Color? view, out Color? rearView)
    {
        view = null;
        rearView = null;
        Interactive? target = FindInteractive(out float interactiveDistance, direction, ignoreTypes: typeof(PlayerModel));
        if (target != null)
        {
            view = GameColors.CalculateView(interactiveDistance, target?.DisplayColor());
        }
        Interactive? backTarget = FindInteractive(out float interactiveDistanceAtBack, !direction, ignoreTypes: typeof(PlayerModel));
        if (backTarget != null)
        {
            rearView = GameColors.CalculateView(interactiveDistanceAtBack, backTarget?.DisplayColor());
        }
    }

    public void NegativeMove()
    {
        isMoving = true;
        direction = false;
    }

    public void PositiveMove()
    {
        isMoving = true;
        direction = true;
    }

    public void StopMovement()
    {
        isMoving = false;
    }

    public void UpdateUI()
    {
        body.TranslationX = Position.X;
        body.TranslationY = Position.Y;
    }

    public abstract void Move();
    public abstract void TakeDamage(float damage);
}