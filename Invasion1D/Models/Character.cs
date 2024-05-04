namespace Invasion1D.Models
{
	public abstract class Character(Dimension dimension, double position, Color color, double speed) : Kinetic(dimension, position, color, speed)
	{
		static App Game => (App)App.Current!;
		public double
			health = 1,
			vitalux = 1,
			vitaAttackCost = .25,
			weaveAttackCost = .50;

		public int
			warpium = 1;

		public bool
			weave = false;

		/// <summary>
		/// uses vitalux to inflict damage
		/// </summary>
		public abstract void Attack();

		public void AddVitalux(double amount, out double remaining)
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

		public void AddHealth(double amount, out double remaining)
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

			vitalux += weaveAttackCost;
			if (vitalux > 1)
			{
				vitalux = 1;
			}
			Game.UI.UpdateVitaLux(vitalux);

			weave = true;
			if (this is Player player)
			{
				player.ActivateWeaveCooldown();
			}

			return true;
		}

	}
}