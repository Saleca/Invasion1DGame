namespace Invasion1D.Data
{
	public static class Stats
	{
		//Items
		public const int
			startWarpium = 0;

		public const float
			regularAttackCost = 0.25f,
			regularAttackDamage = 0.5f,

			weaveAttackCost = 0.5f,
			weaveAttackDamage = 0.5f,

			minScoreItem = 0.5f,
			mediumScoreItem = 0.75f,
			maxScoreItem = 1.0f;

		//Kinetic

		public const float
			playerSpeed = 10,
			enemySpeed = 5,
			bulletSpeed = 20;

		public const int
			bulletDuration = 6000,
			minEnemyReaction = 1000,
			maxEnemyReaction = 3000;

		//Interactive

		public const float
			interactiveObjectSize = 10; //make sure is divisible by 2

		//Player

		public const uint
			warpAnimationDurationMS = 4000,
		halfAnimationDurationMS = warpAnimationDurationMS / 2;

		public const int
			warpIncrementIntervalMS = 25;

		public const float
			warpCooldownIncrementCount = warpAnimationDurationMS / warpIncrementIntervalMS,
			warpCooldownIncrements = 1.0f / warpCooldownIncrementCount;

		//UI

		public const int
			smoothIncrementIntervalMS = 10; //try 20 and try it on warp as well
		public const float
			weaveCoolDownIncrement = .0001f,
			shotCoolDownIncrement = .01f;
		//automation : enemies/items to space ratio

		//items respawn rate

		//timers interval
	}
}
