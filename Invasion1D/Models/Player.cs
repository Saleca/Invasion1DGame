using Invasion1D.Helpers;
using System.Diagnostics;
using System.Timers;

namespace Invasion1D.Models
{
	public class Player : Character
	{
		static App Game => (App)Application.Current!;

		public List<Dimension> visitedDimensions = [];
		double shootCooldownProgress = 1,
			warpCooldownProgress = 1;
		protected System.Timers.Timer? shootCooldownTimer,
									warpCooldownTimer;

		public Player(Dimension dimension, double position, double speed) : base(dimension, position, Colors.Green, speed)
		{
			shootCooldownTimer = SetUpTimer(
									miliseconds: 10,
									reset: true,
									onElapsed: () => OnShootCooldownElapsed(null, EventArgs.Empty));
			warpCooldownTimer = SetUpTimer(
									miliseconds: 10,
									reset: true,
									onElapsed: () => OnWarpCooldownElapsed(null, EventArgs.Empty));

			direction = Game.RandomDirection();

			Game.UI.AddWarpium();
			Game.UI.UpdateVitaLux(vitalux);
			Game.UI.UpdateHealth(health);
		}

		public async void WarpAsync()
		{
			if (warpium > 0)
			{
				visitedDimensions.Add(CurrentDimention);

				if (visitedDimensions.Count == Game.universe.dimensions.Count)
					visitedDimensions.Clear();

				var unvisitedDimentions = Game.universe.dimensions.Except(visitedDimensions).ToArray();

				Dimension cd = CurrentDimention;
				CurrentDimention = unvisitedDimentions[Game.throwDice.Next(unvisitedDimentions.Length)];
				cd.interactiveObjects.Remove(this);

				do
				{
					PercentageInShape = Game.throwDice.NextDouble();
				} while (CurrentDimention.CheckOverlap(this));

				warpium--;
				Game.UI.RemoveWarpium();

				Game.UI.ShowWarpKey(false);
				await Game.UI.WarpAnimation(this,
					start: new(body.TranslationX, body.TranslationY),
					end: new(Position.X, Position.Y));

				//try to start and end cooldown with animation
				ActivateWarpCooldown();
			}
		}

		void ActivateWarpCooldown()
		{
			warpCooldownProgress = 1;
			Game.UI.WarpCooldown(shootCooldownProgress);
			warpCooldownTimer?.Start();
		}

		protected void OnWarpCooldownElapsed(object? sender, EventArgs e)
		{
			warpCooldownProgress -= .01;
			Game.UI.RunOnUIThread(() => Game.UI.WarpCooldown(warpCooldownProgress));

			if (warpCooldownProgress <= 0)
			{
				warpCooldownTimer?.Stop();
				Game.UI.RunOnUIThread(() => Game.UI.ShowWarpKey(true));
			}
		}

		public override void Attack()
		{
			if (vitalux >= attackCost)
			{
				vitalux -= attackCost;
				Game.UI.UpdateVitaLux(vitalux);

				Bullet bullet = new(dimention: CurrentDimention,
						position: direction ?
							GameMath.AddPercentage(PercentageInShape, sizePercentage) :
							GameMath.SubtractPercentage(PercentageInShape, sizePercentage),
						direction: direction);

				Bullet.Create(bullet);

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

		void ActivateShootCooldown()
		{
			Game.UI.ShowShootKey(false);
			shootCooldownProgress = 1;
			Game.UI.ShootCooldown(shootCooldownProgress);
			shootCooldownTimer?.Start();
		}

		protected void OnShootCooldownElapsed(object? sender, EventArgs e)
		{
			shootCooldownProgress -= .01;
			Game.UI.RunOnUIThread(() => Game.UI.ShootCooldown(shootCooldownProgress));

			if (shootCooldownProgress <= 0)
			{
				shootCooldownTimer?.Stop();
				Game.UI.RunOnUIThread(() => Game.UI.ShowShootKey(true));
			}
		}

		protected override async Task MoveAsync(bool direction)
		{
			this.direction = direction;
			List<Type> ignore = [typeof(Player)];

			while (!cancelMovement.IsCancellationRequested)
			{
				try
				{
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
										Game.UI.UpdateVitaLux(vitalux);
										break;
									case Warpium:
										Game.UI.AddWarpium();
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
						PercentageInShape += CurrentDimention.GetPercentageFromDistance(tryStep);
					}
					else
					{
						PercentageInShape -= CurrentDimention.GetPercentageFromDistance(tryStep);
					}

					body.TranslationX = Position.X;
					body.TranslationY = Position.Y;

					try
					{
						await Task.Delay(movementInterval, cancelMovement.Token);
					}
					catch (OperationCanceledException)
					{
						break;
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"An error occurred: {ex.Message}");
					break;
				}
			}
		}

		public override void TakeDamage(double damage)
		{
			health -= damage;
			Game.UI.UpdateHealth(health);
			if (health <= 0)
			{
				Dispose();
				Game.End();
			}
		}
	}
}