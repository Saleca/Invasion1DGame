using Invasion1D.Data;
using Invasion1D.Helpers;
using System.Diagnostics;

namespace Invasion1D.Models
{
	public class Enemy : Character
	{
		static App Game =>
			(App)App.Current!;

		readonly System.Timers.Timer
			reactionTimer;

		public bool
			toReact = false;

		Interactive?
			targetInSight;

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
			reactionTimer = SetUpTimer(Enemy.Game.throwDice.Next(Stats.minEnemyReaction, Stats.maxEnemyReaction), () => OnElapsedReactionTimer(null, EventArgs.Empty));
		}

		public void Start()
		{
			reactionTimer.Start();
			toReact = false;
		}
		public void Stop()
		{
			reactionTimer.Stop();
			toReact = false;
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

		public void React()
		{
			if (targetInSight is null)
			{
				switch (Enemy.Game.throwDice.Next(3))
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
					switch (Enemy.Game.throwDice.Next(2))
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
			toReact = false;
		}

		private void OnElapsedReactionTimer(object? sender, EventArgs e)
		{
			toReact = true;
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