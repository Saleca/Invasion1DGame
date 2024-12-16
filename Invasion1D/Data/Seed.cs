using Invasion1D.Models;

using Linear = Invasion1D.Models.Linear;

namespace Invasion1D.Data;

public class Seed
{
	//TODO randomise seed
	public Seed()
	{
		Circular
			c1 = new(new(50, 50), 25),
			c2 = new(new(150, 150), 50),
			c3 = new(new(400, 100), 100);

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
			l1 = new(new(90, 75), new(300, 50)),
			l2 = new(new(200, 300), new(500, 350)),
			l3 = new(new(20, 200), new(100, 200));

		_ = new EnemyModel(l1, .25f, 10);
		_ = new WarpiumModel(l1, .7f);
		_ = new HealthModel(l1, .05f);

		_ = new EnemyModel(l2, .15f, 10);
		_ = new WarpiumModel(l2, .5f);

		_ = new EnemyModel(l3, .75f, 10);
		_ = new WarpiumModel(l3, .3f);
		_ = new HealthModel(l3, .5f);
	}
}