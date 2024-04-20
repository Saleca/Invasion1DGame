using Invasion1D.Helpers;
using System.Diagnostics;

namespace Invasion1D.Models
{
	public class Player : Character
	{
		static App Game => (App)Application.Current!;

		public List<Dimension> visitedDimensions = [];


		public Player(Dimension dimension, double position, double speed) : base(dimension, position, Colors.Green, speed)
		{
			direction = clockwise;//randomize
			Game.UI.UpdateWarpium(warpium.ToString());
			Game.UI.UpdateVitaLux(vitalux.ToString());
			Game.UI.UpdateHealth(health.ToString());
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
				CurrentDimention = unvisitedDimentions[Game.random.Next(unvisitedDimentions.Length)];
				cd.interactiveObjects.Remove(this);

				do
				{
					PercentageInShape = Game.random.NextDouble();
				} while (CurrentDimention.CheckOverlap(this));

				warpium--;
				Game.UI.UpdateWarpium(warpium.ToString());

				await Game.UI.WarpAnimation(this,
					start: new(body.TranslationX, body.TranslationY),
					end: new(Position.X, Position.Y));
			}
		}

		public override void Attack()
		{
			//TODO
			//Add cooldown timer
			if (vitalux >= attackCost)
			{
				vitalux -= attackCost;
				Game.UI.UpdateVitaLux(vitalux.ToString());

				Bullet.AddBullets(
					new(shape: CurrentDimention,
						position: direction ?
							GameMath.AddPercentage(PercentageInShape, sizePercentage) :
							GameMath.SubtractPercentage(PercentageInShape, sizePercentage),
						direction: direction));
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
					Interactive? target = FindInteractive(out double distanceFromTarget, ignoreTypes: [.. ignore]);
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
										Game.UI.UpdateVitaLux(vitalux.ToString());
										break;
									case Warpium:
										Game.UI.UpdateWarpium(warpium.ToString());
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
			Game.UI.UpdateHealth(health.ToString());
			if (health <= 0)
			{
				Dispose();
				Game.End();
			}
		}
	}
}