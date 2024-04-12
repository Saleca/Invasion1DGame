namespace Invasion1D.Models
{
	public abstract class Item(Dimension dimension, double position, Color color) : Interactive(dimension, position, color)
	{
		public abstract bool Power(Character character);
	}
}