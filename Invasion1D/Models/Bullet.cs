using Invasion1D.Data;
using Invasion1D.Logic;

namespace Invasion1D.Models
{
    public class Bullet : Kinetic
    {
        public float
            condition,
            damage;

        int cooldownTimer = -1;

        public Bullet(Dimension dimension, float position, bool direction, bool weave, Color color)
            : base(dimension, position, color, Stats.bulletSpeed)
        {
            this.direction = direction;
            if (weave)
            {
                damage = condition = Stats.weaveAttackDamage;
            }
            else
            {
                damage = condition = Stats.regularAttackDamage;

                cooldownTimer = Stats.bulletDurationF;
            }
            Game.Instance.universe.bullets.Add(this);
        }

        public void Tick()
        {
            if (cooldownTimer == -1)
            {
                return;
            }

            cooldownTimer--;
            if (cooldownTimer != 0)
            {
                return;
            }

            TakeDamage(damage);
        }

        public static void AddToUI(Bullet bullet)
        {
            Game.Instance.UI.AddToMap(bullet.body);
        }

        public override void Move()
        {
            List<Type> ignore = [typeof(VitaluxModel), typeof(WarpiumModel), typeof(HealthModel), typeof(WeaveModel)];

            Kinetic? target = FindInteractive(out float distanceFromTarget, direction, this, [.. ignore]) as Kinetic;

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