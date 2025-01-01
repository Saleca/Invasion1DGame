using Invasion1D.Data;
using Invasion1D.Logic;

namespace Invasion1D.Models
{
    public class Bullet : Kinetic
    {
        public float
            condition,
            damage;
        Cooldown? lifeSpan;

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

                lifeSpan = new(
                    interval: Stats.bulletDurationF,
                    complete: () => { TakeDamage(damage); });
                lifeSpan.Activate();
            }
            Game.Instance.universe.bullets.Add(this);
        }

        public static void AddToUI(Bullet bullet)
        {
            Game.Instance.UI.AddToMap(bullet.body);
        }

        public override void Move()
        {
            lifeSpan?.Update();

            List<Type> ignore = [typeof(VitaluxModel), typeof(WarpiumModel), typeof(HealthModel), typeof(WeaveModel)];

            Kinetic? target = FindInteractive(out float distanceFromTarget, direction, this, [.. ignore]) as Kinetic;

            if (distanceFromTarget < stepDistance)
            {
                target?.TakeDamage(damage);
                TakeDamage(0);
                return;
            }

            var step = currentDimension.GetPercentageFromDistance(stepDistance);
            if (!direction)
            {
                step = -step;
            }
            MovePositionByPercentage(step);
        }

        public override void TakeDamage(float damage)
        {
            condition = 0;
            toDispose = true;
        }
    }
}