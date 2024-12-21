﻿namespace Invasion1D.Data;

public static class Stats
{
    //Dimentions

    public const int
        minCircleRadius = 33,
        maxCircleRadius = 66,
        minLineSize = 180,
        maxLineSize = 360,
        maxMultiverseSize = 500,
        maxDensity = 50; //1 unity/maxDentity


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
        minEnemySpeed = 5,
        maxEnemySpeed = 10,
        bulletSpeed = 20;

    public const int
        bulletDurationF = 60,
        minEnemyReactionF = 10,
        maxEnemyReactionF = 30;

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
        smoothIncrementIntervalF = 1;
    public const float
        weaveCoolDownIncrement = .01f,
        shotCoolDownIncrement = .01f;

    //automation : enemies/items to space ratio, items respawn rate
}