using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invasion1D.Data
{
	public static class Stats
	{
		//items
		public const int
			startWarpium = 0;

		public const float
			regularAttackCost = 0.25f,
			regularAttackDamage = 0.5f,

			weaveAttackCost = 0.5f,
			weaveAttackDamage = 0.5f,

			minScoreItem = 0.5f,
			mediumScoreItem = 0.75f,
			maxScoreItem = 1.0f,
			
			weaveCoolDownIncrement = .0001f,
			shotCoolDownIncrement = .01f;
			

		//kinetic

		public const float
			playerSpeed = 10,
			enemySpeed = 5,
			bulletSpeed = 20;

		public const int
			bulletDuration = 6000,
			minEnemyReaction = 1000,
			maxEnemyReaction = 3000;

		//interactive

		public const float
			interactiveObjectSize = 10 //make sure is divisible by 2
			;

		//automation : enemies/items to space ratio

		//items respawn rate

		//timers interval
	}
}
