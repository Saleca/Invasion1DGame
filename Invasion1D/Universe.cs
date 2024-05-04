using Invasion1D.Data;
using Invasion1D.Helpers;
using Invasion1D.Models;
using Microsoft.Maui.Controls.Shapes;
using System.Diagnostics;

namespace Invasion1D
{
	public class Universe
	{
		static App Game => ((App)Application.Current!);

		public readonly List<Dimension> dimensions = [];
		public Player playerData = null!;

		public int
			initialEnemyCount,
			enemyCount;

		public Stopwatch stopwatch = null!;

		public void Initiate()
		{
			_ = new Seed();
			initialEnemyCount = enemyCount = dimensions.SelectMany(d => d.interactiveObjects.OfType<Enemy>()).Count();

			double pp = .9f;
			playerData = new((Circular)dimensions[0], pp, 10);

			//TODO
			//select shape on map to start player on that shape

			Game.UI.UpdateView(playerData.GetView());
			Game.UI.Draw();
		}

		public void Start()
		{
			stopwatch = Stopwatch.StartNew();
		}

		public void ResetDimensions()
		{
			int dimensionsCount = dimensions.Count;
			for (int i = 0; i < dimensionsCount; i++)
			{
				Dimension dimension = dimensions[0];
				int dimensioninteractiveObjectsCount = dimension.interactiveObjects.Count;
				for (int ii = 0; ii < dimensioninteractiveObjectsCount; ii++)
				{
					Interactive interactiveObject = dimension.interactiveObjects[0];
					interactiveObject.Dispose();
				}
				dimension.Dispose();
			}
		}

		public void PlayerMove(bool dir)
		{
			if (dir)
			{
				playerData.PositiveMove();
			}
			else
			{
				playerData.NegativeMove();
			}
		}

		public void StopPlayer() => playerData.StopMovement();
		public void PlayerAttack() => playerData.Attack();
		public void WarpPlayer() => playerData.Warp();
	}
}