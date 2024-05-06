namespace Invasion1D.Models
{
	public abstract class Character(Dimension dimension, float position, Color color, float speed) : Kinetic(dimension, position, color, speed)
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
		//temporary
		public void SetWeave(bool weave) => this.weave = weave;

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
			//TODO:
			//activate cooldown on character to affect enemy

			return true;
		}

	}
}