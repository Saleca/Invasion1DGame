using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invasion1D.Models
{
	internal class Vitalux(
		Dimension dimension, 
		double position) : 
			Item(
				dimension, 
				position, 
				GetResourcesColor(nameof(Vitalux))!)
	{
		static App Game => (App)App.Current!;
		double amount = Game.throwDice.Next(3) switch { 0 => .5, 1 => .75, _ => 1 };

		public override bool Power(Character character)
		{
			double startAmount = amount;
			character.AddVitalux(amount, out amount);

			if (amount <= 0)
			{
				Dispose();
			}

			if (amount < startAmount)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
