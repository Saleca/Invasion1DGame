using System.Diagnostics;

namespace Invasion1D.Models
{
	public class Bullet : Kinetic
	{
		static App Game => (App)Application.Current!;

		public static List<Bullet> Bullets = null!;

		public static double Damage => 1;

		public Bullet(Dimension shape, double position, bool direction) : base(shape, position, Colors.Gray, 20)
		{
			this.direction = direction;

			if (direction)
			{
				PositiveMove();
			}
			else
			{
				NegativeMove();
			}
		}

		public static void AddBullets(Bullet bullet)
		{
			Bullets.Add(bullet);
			Game.UI.AddToMap(bullet.body);
		}

		protected override async Task MoveAsync(bool direction)
		{
			this.direction = direction;
			List<Type> ignore = [typeof(Vitalux), typeof(Warpium)];

			while (!cancelMovement.IsCancellationRequested)
			{
				try
				{
					Kinetic? target = FindInteractive(out double distanceFromTarget, this, [.. ignore]) as Kinetic;

					if (distanceFromTarget < stepDistance)
					{
						target?.TakeDamage(Damage);
						TakeDamage(Damage);
						return;
					}

					if (direction)
					{
						PercentageInShape += CurrentDimention.GetPercentageFromDistance(stepDistance);
					}
					else
					{
						PercentageInShape -= CurrentDimention.GetPercentageFromDistance(stepDistance);
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
			StopMovement();
			Bullets.Remove(this);
			Dispose();
		}
	}
}