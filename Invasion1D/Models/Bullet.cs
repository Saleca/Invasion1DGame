﻿using System.Diagnostics;

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
			if (weave)
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
			Game.universe.bullets.Add(this);
		}

		public static void AddToUI(Bullet bullet)
		{
			Game.UI.AddToMap(bullet.body);
		}

		public override void Move()
		{
			List<Type> ignore = [typeof(Vitalux), typeof(Warpium), typeof(Health), typeof(Weave)];

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
				MovePositionByPercentage(currentDimension.GetPercentageFromDistance(stepDistance));
			}
			else
			{
				MovePositionByPercentage(-currentDimension.GetPercentageFromDistance(stepDistance));
			}
		}

		public override void TakeDamage(double damage)
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