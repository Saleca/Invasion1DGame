using Invasion1D.Helpers;
using System.Diagnostics;

namespace Invasion1D.Models
{
	public class Enemy : Character
	{
		App Game => (App)App.Current!;

		System.Timers.Timer
			actionTimer,
			shootCooldownTimer;

		bool shootCooldown = false;
		Interactive? targetInSight;

		public Enemy(
			Dimension shape,
			double position,
			double speed) :
				base(
					shape,
					position,
					GetResourcesColor(nameof(Enemy))!,
					speed)
		{
			direction = ((App)Application.Current!).RandomDirection();
			shootCooldownTimer = SetUpTimer(1000, () => OnElapsedShootCooldownTimer(null, EventArgs.Empty));
			actionTimer = SetUpTimer(Game.throwDice.Next(1000, 3000), () => OnElapsedActionTimer(null, EventArgs.Empty));
		}

		public void Start()
		{
			actionTimer.Start();
		}
		public void Stop()
		{
			actionTimer.Stop();
		}

		public override void Attack()
		{
			double currentAttackCost = weave ? weaveAttackCost : vitaAttackCost;
			if (vitalux >= currentAttackCost)
			{
				vitalux -= currentAttackCost;

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

				ActivateShootCooldownTimer();
			}
		}

		public override void Move()
		{
			List<Type> ignore = [typeof(Warpium)];
			if (vitalux == 1) ignore.Add(typeof(Vitalux));
			if (health == 1) ignore.Add(typeof(Health));
			if (weave) ignore.Add(typeof(Weave));

			Interactive? target = FindInteractive(out double distanceFromTarget, this, ignoreTypes: [.. ignore]);

			if (target is Enemy)
			{
				direction = !direction;
				return;
			}

			if (target is Player)
			{
				targetInSight = target;
			}

			double tryStep = stepDistance;
			if (distanceFromTarget < tryStep)
			{
				if (target is Item item)
				{
					_ = item.Power(this);
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

		void ActivateShootCooldownTimer()
		{
			shootCooldown = true;
			shootCooldownTimer.Start();
		}
		private void OnElapsedShootCooldownTimer(object? sender, EventArgs e)
		{
			shootCooldown = false;
		}


		private void OnElapsedActionTimer(object? sender, EventArgs e)
		{
			if (targetInSight is null)
			{
				switch (Game.throwDice.Next(3))
				{
					case 0:
						PositiveMove();
						break;
					case 1:
						NegativeMove();
						break;
					default:
						StopMovement();
						break;
				}
			}
			else
			{
				if (targetInSight is Player)
				{
					switch (Game.throwDice.Next(2))
					{
						case 0:
							MoveToTarget();
							break;
						case 1:
							if(!shootCooldown)
							Attack();
							break;
					}
				}
				else
				{
					MoveToTarget();
				}
			}
			RestartActionTimer();
		}

		void RestartActionTimer()
		{
			//actionTimer.Interval = Game.throwDice.Next(1000, 3000);
			actionTimer.Start();
		}

		void MoveToTarget()
		{
			if (direction)
			{
				PositiveMove();
			}
			else
			{
				NegativeMove();
			}
		}

		public override void TakeDamage(double damage)
		{
			health -= damage;
			if (health > 0)
			{
				return;
			}

			toDispose = true;
		}
	}
}