using Invasion1D.Data;
using Invasion1D.Models;
using System.Diagnostics;

namespace Invasion1D
{
	public class Universe
	{
		static App Game => 
			((App)Application.Current!);

		public readonly List<Dimension> 
			dimensions = [];

		public PlayerModel
			player = null!;
		public List<EnemyModel> 
			enemies = [];
		public List<Bullet> 
			bullets = [];

		public int
			initialEnemyCount;

		public Stopwatch 
			stopwatch = null!;

		public void Initiate()
		{
			_ = new Seed();

			enemies = dimensions.SelectMany(dimension => dimension.interactiveObjects.OfType<EnemyModel>()).ToList();
			initialEnemyCount = enemies.Count;

			float pp = .9f;
			player = new((Circular)dimensions[0], pp, 10);
			_ = new WeaveModel((Circular)dimensions[0], 0);

			Game.UI.UpdateView(player.GetView());
			Game.UI.Draw();
		}

		public void Start()
		{
			enemies.AsParallel().ForAll(enemy => enemy.Start());
			stopwatch = Stopwatch.StartNew();
		}
		public void Stop()
		{
			stopwatch.Stop();
			enemies.AsParallel().ForAll(enemy => enemy.Stop());
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
	}
}