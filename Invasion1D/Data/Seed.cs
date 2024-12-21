using Android.Util;
using Invasion1D.Logic;
using Invasion1D.Models;

using Linear = Invasion1D.Models.Linear;

namespace Invasion1D.Data;

public class Seed
{
    //TODO randomise seed
    public Seed()
    {
        bool multiverseHasSpace = true;
        while (multiverseHasSpace)
        {
            Dimension dimension = null!;
            float size = 0;

            int value = Game.Instance.throwDice.Next(0, 2);
            switch (value)
            {
                case 0:
                    //TODO:
                    //generate position and radius
                    PointF position = new();
                    float radius = 0;

                    //TODO:
                    //check overlap

                    dimension = new Circular(position, radius);
                    //TODO:
                    //calculate perimeter size
                    size = 0;
                    break;
                case 1:
                    //TODO:
                    //generate line coordinates
                    PointF
                        startPosition = new(),
                        endPosition = new();

                    //TODO:
                    //check overlap

                    dimension = new Linear(startPosition, endPosition);

                    //TODO:
                    //calculate size
                    size = 0;
                    break;
            }

            //calculate contents density
            int maxContentCount = (int)Math.Round(size / Stats.maxDensity);
            int contents = Game.Instance.throwDice.Next((int)(maxContentCount / 2), maxContentCount); //includes a warpium by default

            //TODO:
            //Calculate positions
            _ = new WarpiumModel(dimension, .3f);

            //add contents
            if (Game.Instance.throwDice.Next(0, 2) > 0)
            {
                _ = new WeaveModel(dimension, .85f);
                contents--;
            }

            //TODO:
            //calculate how many of each item
            int enemyCount = 0,
                healthCount = 0,
                vitaluxCount = 0;
            for (int i = 0; i < enemyCount; i++)
            {
                //TODO:
                //Calculate speed
                float speed = Stats.minEnemySpeed + Stats.maxEnemySpeed;
                _ = new EnemyModel(dimension, .20f, speed);
            }
            for (int i = 0; i < healthCount; i++)
            {
                _ = new HealthModel(dimension, .05f);
            }
            for (int i = 0; i < vitaluxCount; i++)
            {
                _ = new VitaluxModel(dimension, .55f);
            }
        }

        Circular
            c1 = new(new(33, 33), 33),
            c2 = new(new(150, 150), 50),
            c3 = new(new(400, 100), 66);

        _ = new EnemyModel(c1, .20f, 10);
        _ = new EnemyModel(c1, .70f, 10);
        _ = new VitaluxModel(c1, .55f);
        _ = new WarpiumModel(c1, .3f);

        _ = new EnemyModel(c2, .35f, 10);
        _ = new EnemyModel(c2, .98f, 10);
        _ = new WarpiumModel(c2, .2f);
        _ = new VitaluxModel(c2, .55f);
        _ = new HealthModel(c2, .05f);

        _ = new EnemyModel(c3, .05f, 10);
        _ = new EnemyModel(c3, .7f, 10);
        _ = new WarpiumModel(c3, .3f);
        _ = new HealthModel(c3, .4f);
        _ = new VitaluxModel(c3, .55f);
        _ = new WeaveModel(c3, .85f);

        Linear
            l1 = new(new(90, 75), new(300, 0)),
            l2 = new(new(200, 300), new(500, 350)),
            l3 = new(new(0, 200), new(100, 200));

        _ = new EnemyModel(l1, .25f, 10);
        _ = new WarpiumModel(l1, .7f);
        _ = new HealthModel(l1, .05f);

        _ = new EnemyModel(l2, .15f, 10);
        _ = new WarpiumModel(l2, .5f);

        _ = new EnemyModel(l3, .75f, 10);
        _ = new WarpiumModel(l3, .3f);
        _ = new HealthModel(l3, .5f);
    }

    //calculate center of circles to leave a certain offset(set on stats) from margin
    //calculate line direction
    //check overlaping shapes
}