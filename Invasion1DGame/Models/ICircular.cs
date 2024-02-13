namespace Invasion1DGame.Models
{
	public interface ICircular
	{
		public Point Position { get; set; }
		public double Radius { get; init; }
	}
}