using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invasion1D.Models
{
	internal class Health(
		Dimension dimension,
		double position) :
			Item(
				dimension,
				position,
				GetResourcesColor(nameof(Health))!)
	{
		static App Game => (App)App.Current!;
		double amount = Game.throwDice.Next(3) switch { 0 => .5, 1 => .75, _ => 1 };

		public override bool Power(Character character)
		{
			double startAmount = amount;
			character.AddHealth(amount, out amount);

			if (amount <= 0)
			{
				toDispose = true;
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
