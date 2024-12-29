using Invasion1D.Data;
using Invasion1D.Models;
using System.Diagnostics;

namespace Invasion1D.Logic
{
    public class Universe
    {
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

        public void Initiate()
        {
            _ = new Populate();

            enemies = dimensions.SelectMany(dimension => dimension.interactiveObjects.OfType<EnemyModel>()).ToList();
            initialEnemyCount = enemies.Count;

            float pp = .9f;
            player = new((Circular)dimensions[0], pp, 10);
            _ = new WeaveModel((Circular)dimensions[0], 0);

            player.GetView(
                view: out Color? view,
                rearView: out Color? rearView);
            Game.Instance.UI.UpdateView(view, rearView);

            Game.Instance.UI.Draw();
        }

        public void Start()
        {
            enemies.AsParallel().ForAll(enemy => enemy.Start());
        }
       
        public void Stop()
        {
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