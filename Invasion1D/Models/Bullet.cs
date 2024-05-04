using System.Diagnostics;

namespace Invasion1D.Models
{
	public class Bullet : Kinetic
	{
		static App Game => (App)Application.Current!;

		public double condition = 1;
		public double damage;
		bool weave;
		System.Timers.Timer? cooldownTimer;
		public Bullet(Dimension dimension, double position, bool direction, bool weave, Color color) : base(dimension, position, color, 20)
		{
			this.direction = direction;
			this.weave = weave;
			if(weave)
			{
				damage = .5;
				condition = .5;
			}
			else
			{
				damage = 1;
				condition = 1;
				cooldownTimer = SetUpTimer(6000, () => TakeDamage(damage));
				cooldownTimer.Start();
			}
		}

		public static void Create(Bullet bullet)
		{
			Game.UI.AddToMap(bullet.body);
		}

		protected override async Task MoveAsync(bool direction)
		{
			this.direction = direction;
			List<Type> ignore = [typeof(Vitalux), typeof(Warpium), typeof(Health), typeof(Weave)];

			while (!cancelMovement.IsCancellationRequested)
			{
				try
				{
					Kinetic? target = FindInteractive(out double distanceFromTarget, this, [.. ignore]) as Kinetic;
					
					if (distanceFromTarget < stepDistance)
					{
						target?.TakeDamage(damage);
						double damageReceived = damage;
						if (target is Bullet bullet)
						{
							damageReceived = bullet.damage;
						}
						TakeDamage(damageReceived);
						if (condition <= 0)
						{
							return;
						}
					}

					if (direction)
					{
						PercentageInShape += CurrentDimension.GetPercentageFromDistance(stepDistance);
					}
					else
					{
						PercentageInShape -= CurrentDimension.GetPercentageFromDistance(stepDistance);
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
			condition -= damage;
			if (condition > 0)
			{
				return;
			}

			StopMovement();
			Dispose();
		}
	}
}