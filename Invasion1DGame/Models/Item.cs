namespace Invasion1DGame.Models
{
	public class Item(Dimension dimension, double position, Color color) : Interactive(dimension, position, color)
	{
		public override void TakeDamage(double damage)
		{
			throw new NotImplementedException();
		}
	}
}
