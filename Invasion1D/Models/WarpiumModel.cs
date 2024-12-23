﻿using Invasion1D.Helpers;

namespace Invasion1D.Models;

public class WarpiumModel(Dimension dimension, float position)
    : Item(dimension, position, GameColors.Warpium)
{
    public override bool Power(Character character)
    {
        if (character is not PlayerModel player)
        {
            return false;
        }

        player.AddWarpium();
        toDispose = true;
        return true;
    }
}