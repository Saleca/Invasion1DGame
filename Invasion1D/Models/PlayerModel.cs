﻿using Invasion1D.Data;
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
        const float scale = 5;

        float playerOffset = Size - strokeThickness,
            mainFrameCenterX = (float)(Game.Instance.UI.MainFrameAccess.Width / 2) - playerOffset,
            mainFrameCenterY = (float)(Game.Instance.UI.MainFrameAccess.Height / 2) - playerOffset,

            //mid position is reached with scale 1
            midPositionX = GameMath.LinearInterpolation(mainFrameCenterX - start.X, mainFrameCenterX - end.X, 0.5f),
            midPositionY = GameMath.LinearInterpolation(mainFrameCenterY - start.Y, mainFrameCenterY - end.Y, 0.5f),

            offsetX = mainFrameCenterX - (float)Game.Instance.UI.MapViewAccess.Width / 2,
            offsetY = mainFrameCenterY - (float)Game.Instance.UI.MapViewAccess.Height / 2,

            scaledMapCenterX = (float)Game.Instance.UI.MapViewAccess.Width * scale / 2,
            scaledMapCenterY = (float)Game.Instance.UI.MapViewAccess.Height * scale / 2,

            startPositionX = offsetX - (start.X * scale - scaledMapCenterX),
            startPositionY = offsetY - (start.Y * scale - scaledMapCenterY),
            endPositionX = offsetX - (end.X * scale - scaledMapCenterX),
            endPositionY = offsetY - (end.Y * scale - scaledMapCenterY);

        if (!Game.Instance.UI.isMapVisible)
        {
            Game.Instance.UI.MapViewAccess.Scale = scale;

            Game.Instance.UI.MapViewAccess.TranslationX = startPositionX;
            Game.Instance.UI.MapViewAccess.TranslationY = startPositionY;

            Game.Instance.UI.UpdateView(Colors.Transparent, Colors.Transparent);
            Game.Instance.UI.MapViewAccess.IsVisible = true;
        }

        Game.Instance.UI.IsAnimating = true;
        Task<bool> translatePlayer = body.TranslateTo(end.X, end.Y, Stats.warpAnimationDurationMS, Easing.CubicInOut);
        Game.Instance.UI.ActivateWarpCooldown();

        if (!Game.Instance.UI.isMapVisible)
        {
            Task<bool> translateOut = Game.Instance.UI.MapViewAccess.TranslateTo(midPositionX, midPositionY, Stats.halfAnimationDurationMS, Easing.CubicOut);
            Task<bool> scaleOut = Game.Instance.UI.MapViewAccess.ScaleTo(1, Stats.halfAnimationDurationMS, Easing.CubicOut);
            await Task.WhenAll(translateOut, scaleOut);

            Task<bool> translateIn = Game.Instance.UI.MapViewAccess.TranslateTo(endPositionX, endPositionY, Stats.halfAnimationDurationMS, Easing.CubicIn);
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