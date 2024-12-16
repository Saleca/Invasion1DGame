using Invasion1D.Helpers;
using System;
using System.Collections.Generic;
namespace Invasion1D.Models;

internal class WeaveModel(Dimension dimension, float position)
    : Item(dimension, position, GameColors.Weave)
{
    public override bool Power(Character character)
    {
        character.AddWeave();
        toDispose = true;
        return true;
    }
}