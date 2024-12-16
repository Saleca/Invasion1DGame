using Invasion1D.Data;
using Invasion1D.Helpers;

namespace Invasion1D.Models
{
    public class PlayerModel : Character
    {
        public List<Dimension>
            visitedDimensions = [];

        Dimension? travelingToDimension;
        float positionPercentageForNewDimention;

        public PlayerModel(
            Dimension dimension,
            float position,
            float speed) :
                base(
                    dimension,
                    position,
                    GameColors.Player,
                    speed)
        {
            direction = Game.RandomDirection();

            Game.GamePageInstance.AddWarpium();
            Game.GamePageInstance.UpdateVitaLux(vitalux);
            Game.GamePageInstance.UpdateHealth(health);
        }

        public void Warp()
        {
            if (warpium > 0)
            {
                if (visitedDimensions.Count == Game.universe.dimensions.Count - 1)
                {
                    visitedDimensions.Clear();
                }

                visitedDimensions.Add(currentDimension);

                Dimension[] unvisitedDimensions = Game.universe.dimensions.Except(visitedDimensions).ToArray();

                travelingToDimension = unvisitedDimensions[Game.throwDice.Next(unvisitedDimensions.Length)];

                bool newPositionFound;
                PointF? newPosition;
                do
                {
                    positionPercentageForNewDimention = Game.throwDice.NextSingle();
                    newPositionFound = travelingToDimension.CheckIfPositionIsAvailable(
                        positionPercentage: positionPercentageForNewDimention,
                        halfSize: Size / 2,
                        position: out newPosition);
                } while (!newPositionFound);

                warpium--;
                Game.GamePageInstance.RemoveWarpium();

                currentDimension.RemoveInteractiveObject(this);

                _ = WarpAnimation(start: new((float)body.TranslationX, (float)body.TranslationY),
                            end: newPosition!.Value);
            }
        }

        internal async Task WarpAnimation(PointF start, PointF end)
        {
            float offset = Size - strokeThickness,
                mapOffsetX = (float)Game.GamePageInstance.PlayerViewAccess.Width / 2 - offset,
                mapOffsetY = (float)Game.GamePageInstance.PlayerViewAccess.Height / 2 - offset,
                startX = mapOffsetX - start.X,
                startY = mapOffsetY - start.Y,
                endX = mapOffsetX - end.X,
                endY = mapOffsetY - end.Y;

            float scale = default!,
                endScaledX = default!,
                endScaledY = default!,
                midX = default!,
                midY = default!;

            if (!Game.GamePageInstance.isMapVisible)
            {
                scale = 5;
                Game.GamePageInstance.MapViewAccess.Scale = scale;

                float startScaledX = startX * scale,
                startScaledY = startY * scale;
                endScaledX = endX * scale;
                endScaledY = endY * scale;

                midX = GameMath.LinearInterpolation(startX, endX, 0.5f);
                midY = GameMath.LinearInterpolation(startY, endY, 0.5f);

                Game.GamePageInstance.MapViewAccess.TranslationX = startScaledX;
                Game.GamePageInstance.MapViewAccess.TranslationY = startScaledY;

                Game.GamePageInstance.UpdateView(Colors.Transparent, Colors.Transparent);

                Game.GamePageInstance.MapViewAccess.IsVisible = true;
            }

            Game.GamePageInstance.IsAnimating = true;
            Task<bool> translatePlayer = body.TranslateTo(end.X, end.Y, Stats.warpAnimationDurationMS, Easing.CubicInOut);
            Game.GamePageInstance.ActivateWarpCooldown();

            if (!Game.GamePageInstance.isMapVisible)
            {
                Task<bool> translateOut = Game.GamePageInstance.MapViewAccess.TranslateTo(midX, midY, Stats.halfAnimationDurationMS, Easing.CubicOut);
                Task<bool> scaleOut = Game.GamePageInstance.MapViewAccess.ScaleTo(1, Stats.halfAnimationDurationMS, Easing.CubicOut);
                await Task.WhenAll(translateOut, scaleOut);

                Task<bool> translateIn = Game.GamePageInstance.MapViewAccess.TranslateTo(endScaledX, endScaledY, Stats.halfAnimationDurationMS, Easing.CubicIn);
                Task<bool> scaleIn = Game.GamePageInstance.MapViewAccess.ScaleTo(scale, Stats.halfAnimationDurationMS, Easing.CubicIn);
                await Task.WhenAll(translateIn, scaleIn);

            }

            await translatePlayer;
            Game.GamePageInstance.IsAnimating = false;

            if (!Game.GamePageInstance.isMapVisible)
            {
                Game.GamePageInstance.MapViewAccess.IsVisible = false;
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
                Game.GamePageInstance.UpdateVitaLux(vitalux);

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

                Game.GamePageInstance.ActivateShootCooldown();
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
                                Game.GamePageInstance.RunOnUIThread(() => Game.GamePageInstance.UpdateVitaLux(vitalux));
                                break;
                            case WarpiumModel:
                                Game.GamePageInstance.RunOnUIThread(Game.GamePageInstance.AddWarpium);
                                break;
                            case HealthModel:
                                Game.GamePageInstance.RunOnUIThread(() => Game.GamePageInstance.UpdateHealth(health));
                                break;
                            case WeaveModel:
                                Game.GamePageInstance.RunOnUIThread(() => Game.GamePageInstance.UpdateVitaLux(vitalux));
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

            Game.GamePageInstance.RunOnUIThread(() => Game.GamePageInstance.UpdateHealth(health));
            if (health > 0)
            {
                return;
            }

            toDispose = true;
            Game.End();
        }
    }
}