using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invasion1D.Models
{
	public class Warpium(
		Dimension dimension,
		double position) : 
			Item(
				dimension,
				position, 
				GetResourcesColor(nameof(Warpium))!)
	{
		public override bool Power(Character character)
		{
			character.AddWarpium();
			toDispose = true;
			return true;
		}
	}
}
