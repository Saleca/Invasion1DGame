using Invasion1DGame.Models;

using Linear = Invasion1DGame.Models.Linear;

namespace Invasion1DGame.Data
{
	public class Seed
	{
		//TODO automate seed
		public Seed(Universe? universe = null)
		{
			Circular
				c1 = new(universe, new(50, 50), 25),
				c2 = new(universe, new(150, 150), 50),
				c3 = new(universe, new(400, 100), 100);

			_ = new Enemy(c1, .20f, 10);
			_ = new Enemy(c1, .70f, 10);
			_ = new Vitalux(c1, .55f);
			_ = new Warpium(c1, .3f);

			_ = new Enemy(c2, .35f, 10);
			_ = new Enemy(c2, .98f, 10);
			_ = new Warpium(c2, .2f);
			_ = new Vitalux(c2, .55f);

			_ = new Enemy(c3, .05f, 10);
			_ = new Enemy(c3, .70f, 10);
			_ = new Warpium(c3, .3f);
			_ = new Vitalux(c3, .55f);

			Linear
				l1 = new(universe, new(90, 75), new(300, 50)),
				l2 = new(universe, new(200, 300), new(500, 350)),
				l3 = new(universe, new(20, 200), new(100, 200));

			_ = new Enemy(l1, .25f, 10);
			_ = new Warpium(l1, .7f);

			_ = new Enemy(l2, .15f, 10);
			_ = new Warpium(l2, .5f);

			_ = new Enemy(l3, .75f, 10);
			_ = new Warpium(l3, .3f);

		}
	}
}