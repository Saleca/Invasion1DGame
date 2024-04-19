using Invasion1D.Helpers;
using System.Diagnostics;

namespace Invasion1D.Models
{
	public class Player : Character
	{
		public List<Dimension> visitedDimensions = [];

		CancellationTokenSource cancelMovement = null!;

		public Player(Dimension dimension, double position, double speed) : base(dimension, position, Colors.Green, speed)
		{
			direction = clockwise;
		}

		public async void WarpAsync()
		{
			if (warpium > 0)
			{
				visitedDimensions.Add(CurrentDimention);

				if (visitedDimensions.Count == MainPage.Instance.universe.dimensions.Count)
					visitedDimensions.Clear();

				var unvisitedDimentions = MainPage.Instance.universe.dimensions.Except(visitedDimensions).ToArray();

				Dimension cd = CurrentDimention;
				CurrentDimention = unvisitedDimentions[MainPage.Instance.universe.random.Next(unvisitedDimentions.Length)];
				cd.interactiveObjects.Remove(this);

				do
				{
					PercentageInShape = MainPage.Instance.universe.random.NextDouble();
				} while (CurrentDimention.CheckOverlap(this));

				warpium--;

				await MainPage.Instance.WarpAnimation(this,
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

				MainPage.Instance.AddBullets(
					new Bullet(
						shape: CurrentDimention,
						position: direction ?
								GameMath.AddPercentage(PercentageInShape, sizePercentage) :
								GameMath.SubtractPercentage(PercentageInShape, sizePercentage),
						direction: direction));
			}
		}

		public override void NegativeMove()
		{
			if (cancelMovement is not null && !cancelMovement.IsCancellationRequested)
			{
				StopMovement();
			}
			cancelMovement = new();
			_ = MoveAsync(!clockwise);
		}

		public override void PositiveMove()
		{
			if (cancelMovement is not null && !cancelMovement.IsCancellationRequested)
			{
				StopMovement();
			}
			cancelMovement = new();
			_ = MoveAsync(clockwise);
		}

		public override void StopMovement()
		{
			if (cancelMovement is not null && !cancelMovement.IsCancellationRequested)
			{
				cancelMovement.Cancel();
				cancelMovement.Dispose();
			}
		}

		async Task MoveAsync(bool direction)
		{
			this.direction = direction;
			List<Type> ignore = [typeof(Player)];

			while (!cancelMovement.IsCancellationRequested)
			{
				try
				{
					Interactive? target = FindInteractive(out double distanceFromTarget, [.. ignore]);
					double tryStep = stepDistance;
					if (distanceFromTarget < tryStep)
					{
						if (target is Item item)
						{
							if (!item.Power(this))
							{
								switch (item)
								{
									case Vitalux:
										ignore.Add(typeof(Vitalux));
										break;
									case Warpium:
										ignore.Add(typeof(Warpium));
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
			if (health <= 0)
			{
				Dispose();
				MainPage.Instance.universe.GameOver();
			}
		}
	}
}