using Invasion1D.Models;

using Linear = Invasion1D.Models.Linear;

namespace Invasion1D.Data;

public class Populate
{
    public Populate()
    {
        //*
        Circular
            c1 = new(new(33, 33), 33),
            c2 = new(new(150, 150), 50),
            c3 = new(new(400, 100), 66);

        _ = new EnemyModel(c1, .20f, Stats.enemySpeed);
        _ = new EnemyModel(c1, .70f, Stats.enemySpeed);
        _ = new VitaluxModel(c1, .55f);
        _ = new WarpiumModel(c1, .3f);

        _ = new EnemyModel(c2, .35f, Stats.enemySpeed);
        _ = new EnemyModel(c2, .98f, Stats.enemySpeed);
        _ = new WarpiumModel(c2, .2f);
        _ = new VitaluxModel(c2, .55f);
        _ = new HealthModel(c2, .05f);

        _ = new EnemyModel(c3, .05f, Stats.enemySpeed);
        _ = new EnemyModel(c3, .7f, Stats.enemySpeed);
        _ = new WarpiumModel(c3, .3f);
        _ = new HealthModel(c3, .4f);
        _ = new VitaluxModel(c3, .55f);
        _ = new WeaveModel(c3, .85f);
        //*/

        Linear
           l1 = new(new(90, 75), new(300, 0)),
           l2 = new(new(0, 200), new(100, 250)),
           l3 = new(new(200, 300), new(400, 350));

        _ = new EnemyModel(l1, .25f, Stats.enemySpeed);
        _ = new WarpiumModel(l1, .7f);
        _ = new HealthModel(l1, .05f);

        _ = new EnemyModel(l2, .15f, Stats.enemySpeed);
        _ = new WarpiumModel(l2, .5f);

        _ = new EnemyModel(l3, .75f, Stats.enemySpeed);
        _ = new WarpiumModel(l3, .3f);
        _ = new HealthModel(l3, .5f);

        /*
        Linear
            lt = new(new(100, 90), new(100, 0)),
            ltr = new(new(110, 90), new(200, 0)),
            lr = new(new(110, 100), new(200, 100)),
            lbr = new(new(110, 110), new(200, 200)),
            lb = new(new(100, 110), new(100, 200)),
            lbl = new(new(90, 110), new(0, 200)),
            ll = new(new(90, 100), new(0, 100)),
            ltl = new(new(90, 90), new(0, 0));

        _ = new WarpiumModel(lt, .7f);
        _ = new WeaveModel(lt, .5f);

        _ = new WarpiumModel(ltr, .7f);
        _ = new WeaveModel(ltr, .5f);

        _ = new WarpiumModel(lr, .7f);
        _ = new WeaveModel(lr, .5f);

        _ = new WarpiumModel(lbr, .7f);
        _ = new WeaveModel(lbr, .5f);

        _ = new WarpiumModel(lb, .7f);
        _ = new WeaveModel(lb, .5f);

        _ = new WarpiumModel(lbl, .7f);
        _ = new WeaveModel(lbl, .5f);

        _ = new WarpiumModel(ll, .7f);
        _ = new WeaveModel(ll, .5f);

        _ = new WarpiumModel(ltl, .7f);
        _ = new WeaveModel(ltl, .5f);
        //*/
    }
}