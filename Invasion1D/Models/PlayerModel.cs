using Invasion1D.Data;
using Invasion1D.Helpers;
using Invasion1D.Logic;
using System.Diagnostics;

namespace Invasion1D.Models;

public class PlayerModel : Character
{
    public List<Dimension>
        visitedDimensions = [];

    Dimension? travelingToDimension;
    float positionPercentageForNewDimention;

    public PlayerModel(Dimension dimension, float position, float speed)
        : base(dimension, position, GameColors.Player, speed)
    {
        direction = Game.Instance.RandomDirection();

        Game.Instance.UI.AddWarpium();
        Game.Instance.UI.UpdateVitaLux(vitalux);
        Game.Instance.UI.UpdateHealth(health);
    }

    public void Warp()
    {
        if (warpium > 0)
        {
            if (visitedDimensions.Count == Game.Instance.universe.dimensions.Count - 1)
            {
                visitedDimensions.Clear();
            }

            visitedDimensions.Add(currentDimension);

            Dimension[] unvisitedDimensions = Game.Instance.universe.dimensions.Except(visitedDimensions).ToArray();

            travelingToDimension = unvisitedDimensions[Game.Instance.throwDice.Next(unvisitedDimensions.Length)];

            bool newPositionFound;
            PointF? newPosition;
            do
            {
                positionPercentageForNewDimention = Game.Instance.throwDice.NextSingle();
                newPositionFound = travelingToDimension.CheckIfPositionIsAvailable(
                    positionPercentage: positionPercentageForNewDimention,
                    halfSize: Size / 2,
                    position: out newPosition);
            } while (!newPositionFound);

            warpium--;
            Game.Instance.UI.RemoveWarpium();

            currentDimension.RemoveInteractiveObject(this);

            _ = WarpAnimation(start: new((float)body.TranslationX, (float)body.TranslationY),
                        end: newPosition!.Value);
        }
    }

    internal async Task WarpAnimation(PointF start, PointF end)
    {
        Debug.WriteLine($"{Game.Instance.UI.MainFrameAccess.X}, {Game.Instance.UI.MainFrameAccess.Y}");
        Debug.WriteLine($"{Game.Instance.UI.MainFrameAccess.Width}, {Game.Instance.UI.MainFrameAccess.Height}");
        Debug.WriteLine(start);
        Debug.WriteLine(end);

        float offset = Size - strokeThickness,
            mapOffsetX = (float)Game.Instance.UI.MainFrameAccess.Width / 2 - offset,
            mapOffsetY = (float)Game.Instance.UI.MainFrameAccess.Height / 2 - offset,
            startX = mapOffsetX - start.X,
            startY = mapOffsetY - start.Y,
            endX = mapOffsetX - end.X,
            endY = mapOffsetY - end.Y;

        float scale = default!,
            endScaledX = default!,
            endScaledY = default!,
            midX = default!,
            midY = default!;

        if (!Game.Instance.UI.isMapVisible)
        {
            scale = 5;
            Game.Instance.UI.MapViewAccess.Scale = scale;

            float startScaledX = startX * scale,
            startScaledY = startY * scale;
            endScaledX = endX * scale;
            endScaledY = endY * scale;

            midX = GameMath.LinearInterpolation(startX, endX, 0.5f);
            midY = GameMath.LinearInterpolation(startY, endY, 0.5f);

            Game.Instance.UI.MapViewAccess.TranslationX = startScaledX;
            Game.Instance.UI.MapViewAccess.TranslationY = startScaledY;

            Game.Instance.UI.UpdateView(Colors.Transparent, Colors.Transparent);

            Game.Instance.UI.MapViewAccess.IsVisible = true;
        }

        Game.Instance.UI.IsAnimating = true;
        Task<bool> translatePlayer = body.TranslateTo(end.X, end.Y, Stats.warpAnimationDurationMS, Easing.CubicInOut);
        Game.Instance.UI.ActivateWarpCooldown();

        if (!Game.Instance.UI.isMapVisible)
        {
            Task<bool> translateOut = Game.Instance.UI.MapViewAccess.TranslateTo(midX, midY, Stats.halfAnimationDurationMS, Easing.CubicOut);
            Task<bool> scaleOut = Game.Instance.UI.MapViewAccess.ScaleTo(1, Stats.halfAnimationDurationMS, Easing.CubicOut);
            await Task.WhenAll(translateOut, scaleOut);

            Task<bool> translateIn = Game.Instance.UI.MapViewAccess.TranslateTo(endScaledX, endScaledY, Stats.halfAnimationDurationMS, Easing.CubicIn);
            Task<bool> scaleIn = Game.Instance.UI.MapViewAccess.ScaleTo(scale, Stats.halfAnimationDurationMS, Easing.CubicIn);
            await Task.WhenAll(translateIn, scaleIn);

        }

        await translatePlayer;
        Game.Instance.UI.IsAnimating = false;

        if (!Game.Instance.UI.isMapVisible)
        {
            Game.Instance.UI.MapViewAccess.IsVisible = false;
        }

        GoToDimension(travelingToDimension!, positionPercentageForNewDimention);
        travelingToDimension = null;
    }

    public override void Attack()
    {
        float currentAttackCost = weave ? weaveAttackCost : vitaAttackCost;
        if (vitalux >= currentAttackCost)
        {
            vitalux -= currentAttackCost;
            Game.Instance.UI.UpdateVitaLux(vitalux);

            Bullet bullet = new(dimension: currentDimension,
                    position: direction ?
                        GameMath.AddPercentage(PositionPercentage, sizePercentage) :
                        GameMath.SubtractPercentage(PositionPercentage, sizePercentage),
                    direction: direction,
                    weave: weave,
                    color: weave ? GameColors.Weave : GameColors.Vitalux);

            Bullet.AddToUI(bullet);

            if (direction)
            {
                bullet.PositiveMove();
            }
            else
            {
                bullet.NegativeMove();
            }

            Game.Instance.UI.ActivateShootCooldown();
        }
    }

    public override void Move()
    {
        List<Type> ignore = [typeof(PlayerModel)];
        if (vitalux == 1) ignore.Add(typeof(VitaluxModel));
        if (health == 1) ignore.Add(typeof(HealthModel));
        if (weave) ignore.Add(typeof(WeaveModel));

        Interactive? target = FindInteractive(
                                closestTargetDistance: out float distanceFromTarget,
                                direction: direction,
                                ignoreTypes: [.. ignore]);

        float step = stepDistance;
        if (distanceFromTarget < step)
        {
            if (target is Item item)
            {
                if (item.Power(this))
                {
                    switch (item)
                    {
                        case VitaluxModel:
                            Game.Instance.UI.RunOnUIThread(() => Game.Instance.UI.UpdateVitaLux(vitalux));
                            break;
                        case WarpiumModel:
                            Game.Instance.UI.RunOnUIThread(Game.Instance.UI.AddWarpium);
                            break;
                        case HealthModel:
                            Game.Instance.UI.RunOnUIThread(() => Game.Instance.UI.UpdateHealth(health));
                            break;
                        case WeaveModel:
                            Game.Instance.UI.RunOnUIThread(() => Game.Instance.UI.UpdateVitaLux(vitalux));
                            break;
                    }
                }
            }
            else
            {
                step = distanceFromTarget;
                StopMovement();
            }
        }

        if (direction)
        {
            MovePositionByPercentage(currentDimension.GetPercentageFromDistance(step));
        }
        else
        {
            MovePositionByPercentage(-currentDimension.GetPercentageFromDistance(step));
        }
    }

    public override void TakeDamage(float damage)
    {
        health -= damage;

        Game.Instance.UI.RunOnUIThread(() => Game.Instance.UI.UpdateHealth(health));
        if (health > 0)
        {
            return;
        }

        toDispose = true;
        Game.Instance.End();
    }
}