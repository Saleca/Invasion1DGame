using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invasion1DGame.Models
{
	public class Warpium(Dimension dimension, double position) : Item(dimension, position, Colors.Blue)
	{
		public override bool Power(Character character)
		{
			character.AddWarpium();
			Dispose();
			return true;
		}
	}
}
