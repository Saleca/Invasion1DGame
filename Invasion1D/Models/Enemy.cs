using System.Diagnostics;

namespace Invasion1D.Models
{
	public class Enemy : Character
	{
		public Enemy(Dimension shape, double position, double speed) : base(shape, position, Colors.Red, speed)
		{
			direction = clockwise; //randomize
		}

		public override void Attack()
		{
			throw new NotImplementedException();
		}

		public override void TakeDamage(double damage)
		{
			health -= damage;
			if (health <= 0)
			{
				StopMovement();
				Dispose();
			}
		}

		protected override async Task MoveAsync(bool direction)
		{
			this.direction = direction;
			List<Type> ignore = [];

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
	}
}