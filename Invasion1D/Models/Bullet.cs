using Invasion1D.Data;

namespace Invasion1D.Models
{
	public class Bullet : Kinetic
	{
		public float
			condition;
		public float
			damage;

		readonly bool
			weave;

		readonly System.Timers.Timer?
			cooldownTimer;

		public Bullet(Dimension dimension, float position, bool direction, bool weave, Color color) : base(dimension, position, color, Stats.bulletSpeed)
		{
			this.direction = direction;
			this.weave = weave;
			if (weave)
			{
				damage = condition = Stats.weaveAttackDamage;
			}
			else
			{
				damage = condition = Stats.regularAttackDamage;

				cooldownTimer = SetUpTimer(Stats.bulletDuration, (o, e) => TakeDamage(damage));
				cooldownTimer.Start();
			}
			Game.universe.bullets.Add(this);
		}

		public static void AddToUI(Bullet bullet)
		{
			Game.UI.AddToMap(bullet.body);
		}

		public override void Move()
		{
			List<Type> ignore = [typeof(Vitalux), typeof(Warpium), typeof(Health), typeof(Weave)];

			Kinetic? target = FindInteractive(out float distanceFromTarget, this, [.. ignore]) as Kinetic;

			if (distanceFromTarget < stepDistance)
			{
				target?.TakeDamage(damage);
				float damageReceived = damage;
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
				MovePositionByPercentage(currentDimension.GetPercentageFromDistance(stepDistance));
			}
			else
			{
				MovePositionByPercentage(-currentDimension.GetPercentageFromDistance(stepDistance));
			}
		}

		public override void TakeDamage(float damage)
		{
			condition -= damage;
			if (condition > 0)
			{
				return;
			}

			toDispose = true;
		}
	}
}