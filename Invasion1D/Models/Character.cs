using Invasion1D.Data;
using Invasion1D.Logic;

namespace Invasion1D.Models;

public abstract class Character(Dimension dimension, float position, Color color, float speed)
    : Kinetic(dimension, position, color, speed)
{
    public Cooldown
        weaveCooldown = null!,
        shootCooldown = null!;

    public float
        health = 1,
        vitalux = 1;

    public bool weave = false;

    //temporary
    public void Tick()
    {
        if (weaveCooldown.IsActive)
        {
            weaveCooldown.Update();
        }
        if (shootCooldown.IsActive)
        {
            shootCooldown.Update();
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

    public bool AddWeave()
    {
        if (weave)
        {
            return false;
        }

        vitalux = 1;
        weave = true;

        weaveCooldown.Activate();

        return true;
    }
}