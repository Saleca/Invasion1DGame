using Invasion1D.Helpers;

namespace Invasion1D.Models
{
	public class Warpium(
		Dimension dimension,
		float position) :
			Item(
				dimension,
				position,
				GameColors.GetFromResources(nameof(Warpium))!)
	{
		public override bool Power(Character character)
		{
			character.AddWarpium();
			toDispose = true;
			return true;
		}
	}
}