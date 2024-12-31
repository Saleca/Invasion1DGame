using Invasion1D.Data;
using Invasion1D.Helpers;
using Invasion1D.Logic;
using System.Diagnostics;

namespace Invasion1D.Models;

public class EnemyModel : Character
{
    Cooldown reactionCooldown = null!;
    bool react = false;
    public bool CanReact => react;
    Interactive? targetInSight;

    public EnemyModel(Dimension shape, float position, float speed)
        : base(shape, position, GameColors.Enemy, speed)
    {
        direction = Game.Instance.RandomDirection();

        SetUpCooldownTimers();
    }

    private void SetUpCooldownTimers()
    {
        shootCooldown = new(
              interval: Stats.smoothIncrementIntervalF,
              increment: Stats.shootCooldownIncrement);

        weaveCooldown = new(
            interval: Stats.smoothIncrementIntervalF,
            increment: Stats.weaveCooldownIncrement,
            start: () => { weave = true; },
            complete: () => { weave = false; });

        reactionCooldown = new(
            start: () => { react = false; },
            complete: () => { react = true; });
    }

    public void Start()
    {
        reactionCooldown.Activate(Game.Instance.Fate.Next(Stats.minEnemyReactionF, Stats.maxEnemyReactionF));
    }

    public void Stop()
    {
        reactionCooldown.Reset();
        react = false;
    }

    public override void Attack()
    {
        float currentAttackCost = weave ? Stats.weaveAttackCost : Stats.regularAttackCost;
        if (vitalux >= currentAttackCost)
        {
            vitalux -= currentAttackCost;

            Debug.WriteLine($"direction: {direction}");
            Bullet bullet = new(dimension: currentDimension,
                    position: direction ?
                        GameMath.AddPercentage(PositionPercentage, diameterPercentage) :
                        GameMath.SubtractPercentage(PositionPercentage, diameterPercentage),
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
            shootCooldown.Activate();
        }
    }

    public override void Move()
    {
        List<Type> ignore = [typeof(WarpiumModel)];
        if (vitalux == 1) ignore.Add(typeof(VitaluxModel));
        if (health == 1) ignore.Add(typeof(HealthModel));
        if (weave) ignore.Add(typeof(WeaveModel));

        Interactive? target = FindInteractive(out float distanceFromTarget, direction, this, ignoreTypes: [.. ignore]);

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
            switch (Game.Instance.Fate.Next(3))
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
                switch (Game.Instance.Fate.Next(2))
                {
                    case 0:
                        MoveToTarget();
                        break;
                    case 1:
                        if (!shootCooldown.IsActive) // run away if no attack available
                        {
                            Attack();
                        }
                        break;
                }
            }
            else
            {
                MoveToTarget();
            }
        }
        reactionCooldown.Activate(Game.Instance.Fate.Next(Stats.minEnemyReactionF, Stats.maxEnemyReactionF));
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

    internal new void Tick()
    {
        base.Tick();
        if (reactionCooldown.IsActive)
        {
            reactionCooldown.Update();
        }
    }
}