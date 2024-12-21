using Invasion1D.Data;
using Invasion1D.Logic;

namespace Invasion1D.Models;

public abstract class Character(Dimension dimension, float position, Color color, float speed)
    : Kinetic(dimension, position, color, speed)
{
    int
        weaveCooldown = -1,
        shootCooldown = -1,
        warpCooldown = -1;

    public float
        health = 1,
        vitalux = 1;

    public int warpium = 1;
    public bool weave = false;

    float
        weaveCooldownProgress = 0,
        shootCooldownProgress = 0,
        warpCooldownProgress = 0;

    //temporary
    public void Tick()
    {
        if (weaveCooldown != -1)
        {
            WeaveCooldown();
        }

        if (shootCooldown != -1)
        {
            ShootCooldown();
        }

        if (warpCooldown != -1)
        {
            WarpCooldown();
        }
    }

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
        {
            return false;
        }

        vitalux = 1;
        weave = true;

        ActivateWeaveCooldown();

        return true;
    }

    public void ActivateWeaveCooldown()
    {
        weaveCooldownProgress = 1;
        weaveCooldown = Stats.smoothIncrementIntervalF;

        if (this is PlayerModel)
        {
            Game.Instance.UI.RunOnUIThread(() =>
            {
                Game.Instance.UI.UpdateWeaveCooldown(weaveCooldownProgress);
            });
        }
    }

    private void WeaveCooldown()
    {
        weaveCooldown--;
        if (weaveCooldown > 0)
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
                Game.Instance.UI.UpdateWeaveCooldown(weaveCooldownProgress);
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

    public void ActivateWarpCooldown()
    {
        warpCooldownProgress = 1;
        warpCooldown = Stats.smoothIncrementIntervalF;

        if (this is PlayerModel)
        {
            Game.Instance.UI.RunOnUIThread(() =>
            {
                Game.Instance.UI.ShowWarpKey(false);
                Game.Instance.UI.UpdateWeaveCooldown(warpCooldownProgress);
            });
        }
    }

    private void WarpCooldown()
    {
        warpCooldown--;
        if (warpCooldown > 0)
        {
            return;
        }

        warpCooldownProgress -= Stats.warpCooldownIncrement;
        if (this is PlayerModel)
        {
            Game.Instance.UI.RunOnUIThread(() =>
            {
                if (disposed)
                {
                    return;
                }
                Game.Instance.UI.UpdateWarpCooldown(warpCooldownProgress);
            });
        }

        if (warpCooldownProgress <= 0)
        {
            warpCooldown = -1;
            Game.Instance.UI.RunOnUIThread(() =>
            {
                Game.Instance.UI.ShowWarpKey(true);
            });
            return;
        }

        warpCooldown = Stats.smoothIncrementIntervalF;

    }

    public void ActivateShootCooldown()
    {
        shootCooldownProgress = 1;
        shootCooldown = Stats.smoothIncrementIntervalF;

        if (this is PlayerModel)
        {
            Game.Instance.UI.RunOnUIThread(() =>
            {
                Game.Instance.UI.ShowShootKey(false);
                Game.Instance.UI.UpdateShootCooldown(shootCooldownProgress);
            });
        }
    }

    private void ShootCooldown()
    {
        shootCooldown--;
        if (shootCooldown > 0)
        {
            return;
        }

        shootCooldownProgress -= Stats.shootCoolDownIncrement;
        if (this is PlayerModel)
        {
            Game.Instance.UI.RunOnUIThread(() =>
            {
                if (disposed)
                {
                    return;
                }
                Game.Instance.UI.UpdateShootCooldown(shootCooldownProgress);
            });
        }

        if (shootCooldownProgress <= 0)
        {
            shootCooldown = -1;
            Game.Instance.UI.RunOnUIThread(() =>
            {
                Game.Instance.UI.ShowShootKey(true);
            });
            return;
        }

        shootCooldown = Stats.smoothIncrementIntervalF;
    }
}