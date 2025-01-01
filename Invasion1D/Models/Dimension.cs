using Invasion1D.Helpers;
using Invasion1D.Logic;

namespace Invasion1D.Models;

public abstract class Dimension()
    : GFX(6, GameColors.Dark, GameColors.Light)
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
        Game.Instance.universe.dimensions.Remove(this);
    }

    public abstract PointF GetPositionInShape(float positionPercentage, float radius);
    public abstract float GetDistanceBetweenPointsOnShape(float positionA, float positionB, bool clockwise);
    public abstract float GetPercentageFromDistance(float distance);
    public abstract float GetDistanceFromPercentage(float percentage);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="radius"></param>
    /// <returns>true if available</returns>
    public bool CheckIfPositionIsAvailable(float positionPercentage, float radius, out PointF? position)
    {
        position = null;
        float radiusPercentage = GetPercentageFromDistance(radius);
        if (CheckOverlap(radiusPercentage, positionPercentage))
        {
            return false;
        }
        position = GetPositionInShape(positionPercentage, radius);
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="radiusPercentage"></param>
    /// <param name="positionPercentage"></param>
    /// <returns>true if overlap</returns>
    public bool CheckOverlap(float radiusPercentage, float positionPercentage)
    {
        float start1 = positionPercentage - radiusPercentage;
        float end1 = positionPercentage + radiusPercentage;

        lock (interactiveObjects)
        {
            foreach (Interactive obj in interactiveObjects)
            {
                float start2 = obj.PositionPercentage - obj.radiusPercentage;
                float end2 = obj.PositionPercentage + obj.radiusPercentage;

                if (start1 < end2 && start2 < end1)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool CheckOverlap(Interactive a, Interactive b)
    {
        lock (interactiveObjects)
        {
            float start1 = a.PositionPercentage - a.radiusPercentage;
            float end1 = a.PositionPercentage + a.radiusPercentage;

            float start2 = b.PositionPercentage - b.radiusPercentage;
            float end2 = b.PositionPercentage + b.radiusPercentage;

            if (start1 < end2 && start2 < end1)
            {
                return true;
            }
        }
        return false;
    }
}