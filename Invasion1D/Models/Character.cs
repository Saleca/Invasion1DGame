using Invasion1D.Data;
using Invasion1D.Logic;
using Timer = System.Timers.Timer;

namespace Invasion1D.Models;

public abstract class Character(Dimension dimension, float position, Color color, float speed)
    : Kinetic(dimension, position, color, speed)
{
    int weaveCooldown = -1;

    public float
        health = 1,
        vitalux = 1,
        vitaAttackCost = 0.25f,
        weaveAttackCost = 0.50f;

    public int
        warpium = 1;

    public bool
        weave = false;

    float weaveCooldownProgress = 1;

    //temporary
    public void Tick()
    {
        if (weaveCooldown == -1)
            return;

        weaveCooldown--;
        if (weaveCooldown != 0)
        {
            return;
        }

        weaveCooldownProgress -= Stats.weaveCoolDownIncrement;
        if (this is PlayerModel)
        {
            Game.Instance.UI.RunOnUIThread(() =>
            {
                if (disposed)
                {
                    return;
                }
                Game.Instance.UI.UpdateWeave(weaveCooldownProgress);
            });
        }

        if (weaveCooldownProgress <= 0)
        {
            weaveCooldown = -1;
            weave = false;
            return;
        }

        weaveCooldown = Stats.smoothIncrementIntervalF;
    }
    public void EndWeaveCooldown(bool weave) => this.weave = weave;

    /// <summary>
    /// uses vitalux to inflict damage
    /// </summary>
    public abstract void Attack();

    public void AddVitalux(float amount, out float remaining)
    {
        remaining = amount;
        if (vitalux >= 1) return;

        vitalux += amount;
        if (vitalux > 1)
        {
            remaining = vitalux - 1;
            vitalux = 1;
        }
        else
        {
            remaining = 0;
        }
    }

    public void AddHealth(float amount, out float remaining)
    {
        remaining = amount;
        if (health >= 1) return;

        health += amount;
        if (health > 1)
        {
            remaining = health - 1;
            health = 1;
        }
        else
        {
            remaining = 0;
        }
    }

    public void AddWarpium() => warpium++;

    public bool AddWeave()
    {
        if (weave)
            return false;

        vitalux = 1;
        weave = true;

        ActivateWeaveCooldown();

        return true;
    }

    public void ActivateWeaveCooldown()
    {
        weaveCooldownProgress = 1;
        if (this is PlayerModel)
        {
            Game.Instance.UI.RunOnUIThread(() => Game.Instance.UI.UpdateWeave(weaveCooldownProgress));
        }
        weaveCooldown = Stats.smoothIncrementIntervalF;
    }
}