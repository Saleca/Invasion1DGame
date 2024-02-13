namespace Invasion1DGame.Models
{
	public abstract class Character(Dimension dimension, double position, Color color, double speed) : Kinetic(dimension, position, color, speed)
	{
		public double health = 1, vitalux = 1, attackCost = .1;
		public int warpium = 1;

		/// <summary>
		/// uses stamina to inflict damage
		/// </summary>
		public abstract void Attack();
	}
}