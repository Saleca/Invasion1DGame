using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invasion1D.Models
{
	internal class Weave(
		Dimension dimension,
		float position) :
			Item(
				dimension,
				position,
				GetResourcesColor(nameof(Weave))!)
	{
		public override bool Power(Character character)
		{
			character.AddWeave();
			toDispose = true;
			return true;
		}
	}

}
