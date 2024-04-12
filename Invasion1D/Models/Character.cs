namespace Invasion1D.Models
{
	public abstract class Character(Dimension dimension, double position, Color color, double speed) : Kinetic(dimension, position, color, speed)
	{
		public double health = 1, attackCost = .25, vitalux = 1;
		public int warpium = 1;

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

		public void AddWarpium() => warpium++;
	}
}