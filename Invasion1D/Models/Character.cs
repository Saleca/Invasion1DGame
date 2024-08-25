using Invasion1D.Data;
using Timer = System.Timers.Timer;

namespace Invasion1D.Models
{
    public abstract class Character : Kinetic
    {
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
        public Timer weaveCooldownTimer = null!;

        public Character(Dimension dimension, float position, Color color, float speed) : base(dimension, position, color, speed)
        {
            weaveCooldownTimer = SetUpTimer(Stats.smoothIncrementIntervalMS, OnWeaveCooldownElapsed, true);
        }

        //temporary
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
            if (this is Player)
            {
                Game.UI.RunOnUIThread(() => Game.UI.UpdateWeave(weaveCooldownProgress));
            }
            weaveCooldownTimer?.Start();
        }
        protected void OnWeaveCooldownElapsed(object? sender, EventArgs e)
        {
            weaveCooldownProgress -= Stats.weaveCoolDownIncrement;
            if (this is Player)
            {
                Game.UI.RunOnUIThread(() =>
                {
                    if (disposed)
                    {
                        return;
                    }
                    Game.UI.UpdateWeave(weaveCooldownProgress);
                });
            }
            if (weaveCooldownProgress <= 0)
            {
                weaveCooldownTimer?.Stop();
                weave = false;
            }
        }
    }
}