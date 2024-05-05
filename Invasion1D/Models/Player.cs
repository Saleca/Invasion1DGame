using Invasion1D.Helpers;
using System.Diagnostics;
using System.Timers;

namespace Invasion1D.Models
{
	public class Player : Character
	{
		const uint
			warpAnimationLength = 4000,
			halfAnimationLength = warpAnimationLength / 2;

		const int
			warpCooldownIntervalsLenght = 25;

		const double
			warpCooldownIntervalsCount = (double)warpAnimationLength / warpCooldownIntervalsLenght,
			warpCooldownProgressIncrements = 1.0 / warpCooldownIntervalsCount;

		static App Game => (App)Application.Current!;

		public List<Dimension>
			visitedDimensions = [];

		Dimension? travelingToDimension;
		double positionPercentageForNewDimention;

		double
			shootCooldownProgress = 1,
			warpCooldownProgress = 1,
			weaveCooldownProgress = 1;

		protected System.Timers.Timer?
			shootCooldownTimer,
			warpCooldownTimer,
			weaveCooldownTimer;

		public Player(
			Dimension dimension,
			double position,
			double speed) :
				base(
					dimension,
					position,
					GetResourcesColor(nameof(Player))!,
					speed)
		{
			shootCooldownTimer = SetUpTimer(
									miliseconds: 10,
									reset: true,
									onElapsed: () => OnShootCooldownElapsed(null, EventArgs.Empty));
			warpCooldownTimer = SetUpTimer(
									miliseconds: warpCooldownIntervalsLenght,
									reset: true,
									onElapsed: () => OnWarpCooldownElapsed(null, EventArgs.Empty));
			weaveCooldownTimer = SetUpTimer(
									miliseconds: 10,
									reset: true,
									onElapsed: () => OnWeaveCooldownElapsed(null, EventArgs.Empty));

			direction = Game.RandomDirection();

			Game.UI.AddWarpium();
			Game.UI.UpdateVitaLux(vitalux);
			Game.UI.UpdateHealth(health);
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

				var unvisitedDimensions = Game.universe.dimensions.Except(visitedDimensions).ToArray();

				travelingToDimension = unvisitedDimensions[Game.throwDice.Next(unvisitedDimensions.Length)];

				bool newPositionFound;
				Point? newPosition;
				do
				{
					positionPercentageForNewDimention = Game.throwDice.NextDouble();
					newPositionFound = travelingToDimension.CheckIfPositionIsAvailable(
						positionPercentage: positionPercentageForNewDimention,
						halfSize: Radius,
						position: out newPosition);
				} while (!newPositionFound);

				warpium--;
				Game.UI.RemoveWarpium();

				currentDimension.interactiveObjects.Remove(this);
				_ = WarpAnimation(start: new(body.TranslationX, body.TranslationY),
							end: newPosition!.Value);
			}
		}

		internal async Task WarpAnimation(Point start, Point end)
		{
			double offset = Radius - strokeThickness,
				mapOffsetX = Game.UI.PlayerViewAccess.Width / 2 - offset,
				mapOffsetY = Game.UI.PlayerViewAccess.Height / 2 - offset,
				startX = mapOffsetX - start.X,
				startY = mapOffsetY - start.Y,
				endX = mapOffsetX - end.X,
				endY = mapOffsetY - end.Y;

			double scale = default!,
				endScaledX = default!,
				endScaledY = default!,
				midX = default!,
				midY = default!;

			if (!Game.UI.isMapVisible)
			{
				scale = 5;
				Game.UI.MapViewAccess.Scale = scale;

				double startScaledX = startX * scale,
				startScaledY = startY * scale;
				endScaledX = endX * scale;
				endScaledY = endY * scale;

				midX = GameMath.LinearInterpolation(startX, endX, .5);
				midY = GameMath.LinearInterpolation(startY, endY, .5);


				Game.UI.MapViewAccess.TranslationX = startScaledX;
				Game.UI.MapViewAccess.TranslationY = startScaledY;

				Game.UI.UpdateView(Colors.Transparent);
				Game.UI.MapViewAccess.IsVisible = true;
			}

			Game.UI.IsAnimating = true;
			Task<bool> translatePlayer = body.TranslateTo(end.X, end.Y, warpAnimationLength, Easing.CubicInOut);
			ActivateWarpCooldown();

			if (!Game.UI.isMapVisible)
			{
				Task<bool> translateOut = Game.UI.MapViewAccess.TranslateTo(midX, midY, halfAnimationLength, Easing.CubicOut);
				Task<bool> scaleOut = Game.UI.MapViewAccess.ScaleTo(1, halfAnimationLength, Easing.CubicOut);
				await Task.WhenAll(translateOut, scaleOut);

				Task<bool> translateIn = Game.UI.MapViewAccess.TranslateTo(endScaledX, endScaledY, halfAnimationLength, Easing.CubicIn);
				Task<bool> scaleIn = Game.UI.MapViewAccess.ScaleTo(scale, halfAnimationLength, Easing.CubicIn);
				await Task.WhenAll(translateIn, scaleIn);

			}

			await translatePlayer;
			Game.UI.IsAnimating = false;

			if (!Game.UI.isMapVisible)
			{
				Game.UI.MapViewAccess.IsVisible = false;
			}

			GoToDimension(travelingToDimension!, positionPercentageForNewDimention);
			travelingToDimension = null;
		}

		public override void Attack()
		{
			double currentAttackCost = weave ? weaveAttackCost : vitaAttackCost;
			if (vitalux >= currentAttackCost)
			{
				vitalux -= currentAttackCost;
				Game.UI.UpdateVitaLux(vitalux);

				Bullet bullet = new(dimension: currentDimension,
						position: direction ?
							GameMath.AddPercentage(PositionPercentage, sizePercentage) :
							GameMath.SubtractPercentage(PositionPercentage, sizePercentage),
						direction: direction,
						weave: weave,
						color: GetResourcesColor(weave ? nameof(Weave) : nameof(Vitalux))!
						);

				Bullet.AddToUI(bullet);

				if (direction)
				{
					bullet.PositiveMove();
				}
				else
				{
					bullet.NegativeMove();
				}

				ActivateShootCooldown();
			}
		}

		public override void Move()
		{
			List<Type> ignore = [typeof(Player)];
			if (vitalux == 1) ignore.Add(typeof(Vitalux));
			if (health == 1) ignore.Add(typeof(Health));
			if (weave) ignore.Add(typeof(Weave));

			Interactive? target = FindInteractive(
			closestTargetDistance: out double distanceFromTarget,
			ignoreTypes: [.. ignore]);

			double tryStep = stepDistance;
			if (distanceFromTarget < tryStep)
			{
				if (target is Item item)
				{
					if (item.Power(this))
					{
						switch (item)
						{
							case Vitalux:
								Game.UI.RunOnUIThread(() => Game.UI.UpdateVitaLux(vitalux));
								break;
							case Warpium:
								Game.UI.RunOnUIThread(Game.UI.AddWarpium);
								break;
							case Health:
								Game.UI.RunOnUIThread(() => Game.UI.UpdateHealth(health));
								break;
							case Weave:
								ActivateWeaveCooldown();
								Game.UI.RunOnUIThread(() => Game.UI.UpdateVitaLux(vitalux));
								break;
						}
					}
				}
				else
				{
					tryStep = distanceFromTarget;
					StopMovement();
				}
			}

			if (direction)
			{
				MovePositionByPercentage(currentDimension.GetPercentageFromDistance(tryStep));
			}
			else
			{
				MovePositionByPercentage(-currentDimension.GetPercentageFromDistance(tryStep));
			}
		}

		void ActivateWarpCooldown()
		{
			warpCooldownProgress = 1;
			Game.UI.RunOnUIThread(() =>
			{
				Game.UI.ShowWarpKey(false);
				Game.UI.WarpCooldown(warpCooldownProgress);
				Game.UI.ShowWarpProgress(true);
			});
			warpCooldownTimer?.Start();
		}

		protected void OnWarpCooldownElapsed(object? sender, EventArgs e)
		{
			warpCooldownProgress -= warpCooldownProgressIncrements;

			Game.UI.RunOnUIThread(() =>
			{
				if (disposed)
				{
					return;
				}
				Game.UI.WarpCooldown(warpCooldownProgress);
			});

			if (warpCooldownProgress <= 0)
			{
				warpCooldownTimer?.Stop();

				Game.UI.RunOnUIThread(() => { Game.UI.ShowWarpKey(true); Game.UI.ShowWarpProgress(false); });
			}
		}

		public void ActivateWeaveCooldown()
		{
			weaveCooldownProgress = 1;
			Game.UI.RunOnUIThread(() => Game.UI.WeaveCooldown(warpCooldownProgress));
			weaveCooldownTimer?.Start();
		}
		protected void OnWeaveCooldownElapsed(object? sender, EventArgs e)
		{
			weaveCooldownProgress -= .0001;
			Game.UI.RunOnUIThread(() =>
			{
				if (disposed)
				{
					return;
				}
				Game.UI.WeaveCooldown(weaveCooldownProgress);
			});

			if (weaveCooldownProgress <= 0)
			{
				weaveCooldownTimer?.Stop();
				weave = false;
			}
		}

		void ActivateShootCooldown()
		{
			shootCooldownProgress = 1;
			Game.UI.RunOnUIThread(() =>
			{
				Game.UI.ShowShootKey(false);
				Game.UI.ShowShootProgress(true);
				Game.UI.ShootCooldown(shootCooldownProgress);
			});
			shootCooldownTimer?.Start();
		}

		protected void OnShootCooldownElapsed(object? sender, EventArgs e)
		{
			shootCooldownProgress -= .01;
			Game.UI.RunOnUIThread(() =>
			{
				if (disposed)
				{
					return;
				}
				Game.UI.ShootCooldown(shootCooldownProgress);
			});

			if (shootCooldownProgress <= 0)
			{
				shootCooldownTimer?.Stop();
				Game.UI.RunOnUIThread(() => { Game.UI.ShowShootKey(true); Game.UI.ShowShootProgress(false); });
			}
		}

		public override void TakeDamage(double damage)
		{
			health -= damage;

			Game.UI.RunOnUIThread(() => Game.UI.UpdateHealth(health));
			if (health > 0)
			{
				return;
			}

			toDispose = true;
			Game.End();
		}
	}
}