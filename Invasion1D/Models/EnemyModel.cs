using Invasion1D.Data;
using Invasion1D.Helpers;

namespace Invasion1D.Models
{
    public class EnemyModel : Character
    {
        System.Timers.Timer
            reactionTimer;

        public bool
            toReact = false;


        Interactive?
            targetInSight;

        public EnemyModel(
            Dimension shape,
            float position,
            float speed) :
                base(
                    shape,
                    position,
                    GameColors.Enemy,
                    speed)
        {
            direction = ((App)Application.Current!).RandomDirection();
            reactionTimer = SetUpTimer(Game.throwDice.Next(Stats.minEnemyReaction, Stats.maxEnemyReaction), OnElapsedReactionTimer);
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

       /* public void Pause(bool pause)
        {
            if (pause)
            {
                reactionTimer.Stop();
            }
            else
            {
                if (toReact == false)
                {
                    //get the remaining time if it was reacting
                    RemoveTimer(reactionTimer);
                    reactionTimer = SetUpTimer(Game.throwDice.Next(Stats.minEnemyReaction, Stats.maxEnemyReaction), OnElapsedReactionTimer);
                    reactionTimer.Start();
                }
            }
        }*/

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
                        color: weave ? GameColors.Weave : GameColors.Vitalux);

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
            List<Type> ignore = [typeof(WarpiumModel)];
            if (vitalux == 1) ignore.Add(typeof(VitaluxModel));
            if (health == 1) ignore.Add(typeof(HealthModel));
            if (weave) ignore.Add(typeof(WeaveModel));

            Interactive? target = FindInteractive(out float distanceFromTarget, this, ignoreTypes: [.. ignore]);

            if (target is EnemyModel)
            {
                direction = !direction;
                return;
            }

            if (target is PlayerModel)
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
                if (targetInSight is PlayerModel)
                {
                    switch (Game.throwDice.Next(2))
                    {
                        case 0:
                            MoveToTarget();
                            break;
                        case 1:
                            Attack();
                            //allow run away when no attck available (or search vitalux)
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

        private void OnElapsedReactionTimer(object? sender, EventArgs e)
        {
            toReact = true;
        }

        void RestartReactionTimer()
        {
            toReact = false;
            //RemoveTimer(reactionTimer);
            reactionTimer.Start(); //= SetUpTimer(Game.throwDice.Next(Stats.minEnemyReaction, Stats.maxEnemyReaction), OnElapsedReactionTimer);
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