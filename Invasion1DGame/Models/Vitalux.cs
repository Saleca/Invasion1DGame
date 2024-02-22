using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invasion1DGame.Models
{
	internal class Vitalux(Dimension dimension, double position) : Item(dimension, position, Colors.Yellow)
	{
		double amount = 1;
		public override bool Power(Character character)
		{
			character.AddVitalux(amount, out amount);
			if (amount <= 0)
			{
				Dispose();
				return true;
			}
			return false;
		}
	}
}
