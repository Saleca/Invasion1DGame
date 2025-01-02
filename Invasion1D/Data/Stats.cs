namespace Invasion1D.Data;

public static class Stats
{
    public const int frameMS = 100;

    //Dimentions
    public const int
        minCircleRadius = 33,
        maxCircleRadius = 66,
        minLineLength = 180,
        maxLineLength = 360,
        maxMultiverseSize = 500;

    //Items
    public const int
        startWarpiumCount = 0;

    public const float
        AttackCost = 0.25f,
        AttackDamage = 0.5f,

        weaveAttackCost = 0.1f,
        weaveAttackDamage = 0.5f,

        minScoreItem = 0.5f,
        mediumScoreItem = 0.75f,
        maxScoreItem = 1.0f;

    //Kinetic
    public const float
        playerSpeed = 1,
        enemySpeed = 1,
        bulletSpeed = 2;

    public const int
        bulletDurationF = 60,
        minEnemyReactionF = 10,
        maxEnemyReactionF = 30;

    //Interactive
    public const float
        interactiveObjectRadius = 5;

    //Player
    public const uint
        warpAnimationDurationMS = 4000,
        halfAnimationDurationMS = warpAnimationDurationMS / 2;

    public const float
        warpCooldownIncrementCount = warpAnimationDurationMS / (frameMS * smoothIncrementIntervalF);

    //UI
    public const int
        smoothIncrementIntervalF = 1;
    public const float
        weaveCooldownIncrement = .02f,
        weaveShootCooldownIncrement = .25f,
        shootCooldownIncrement = .05f,
        warpCooldownIncrement = 1.0f / warpCooldownIncrementCount;

    //automation : enemies/items to space ratio, items respawn rate
}