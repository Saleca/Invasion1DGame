using Invasion1D.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invasion1D.Models
{
	internal class Health(
		Dimension dimension,
		float position) :
			Item(
				dimension,
				position,
				GetResourcesColor(nameof(Health))!)
	{
		static App Game => (App)App.Current!;

		float amount = Game.throwDice.Next(3) switch 
		{ 
			0 => Stats.minScoreItem,
			1 => Stats.mediumScoreItem,
			_ => Stats.maxScoreItem 
		};

		public override bool Power(Character character)
		{
			float startAmount = amount;
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
