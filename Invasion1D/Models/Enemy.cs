using Invasion1D.Data;
using Invasion1D.Helpers;
using System.Diagnostics;

namespace Invasion1D.Models
{
	public class Enemy : Character
	{
		App Game => (App)App.Current!;

		System.Timers.Timer
			reactionTimer;

		Interactive? targetInSight;

		public Enemy(
			Dimension shape,
			float position,
			float speed) :
				base(
					shape,
					position,
					GetResourcesColor(nameof(Enemy))!,
					speed)
		{
			direction = ((App)Application.Current!).RandomDirection();
			reactionTimer = SetUpTimer(Game.throwDice.Next(Stats.minEnemyReaction, Stats.maxEnemyReaction), () => OnElapsedReactionTimer(null, EventArgs.Empty));
		}

		public void Start()
		{
			reactionTimer.Start();
		}
		public void Stop()
		{
			reactionTimer.Stop();
		}

		public override void Attack()
		{
			float currentAttackCost = weave ? weaveAttackCost : vitaAttackCost;
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
			}
		}

		public override void Move()
		{
			List<Type> ignore = [typeof(Warpium)];
			if (vitalux == 1) ignore.Add(typeof(Vitalux));
			if (health == 1) ignore.Add(typeof(Health));
			if (weave) ignore.Add(typeof(Weave));

			Interactive? target = FindInteractive(out float distanceFromTarget, this, ignoreTypes: [.. ignore]);

			if (target is Enemy)
			{
				direction = !direction;
				return;
			}

			if (target is Player)
			{
				targetInSight = target;
			}

			float step = stepDistance;
			if (distanceFromTarget < step)
			{
				if (target is Item item)
				{
					_ = item.Power(this);
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

		private void OnElapsedReactionTimer(object? sender, EventArgs e)
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
							Attack();
							break;
					}
				}
				else
				{
					MoveToTarget();
				}
			}
			RestartReactionTimer();
		}

		void RestartReactionTimer()
		{
			reactionTimer.Start();
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

		public override void TakeDamage(float damage)
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