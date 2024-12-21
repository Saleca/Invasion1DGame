using Invasion1D.Data;
using Invasion1D.Helpers;
using Invasion1D.Logic;

namespace Invasion1D.Models;

internal class VitaluxModel(Dimension dimension, float position)
    : Item(dimension, position, GameColors.Vitalux)
{
    float amount = Game.Instance.Fate.Next(3) switch
    {
        0 => Stats.minScoreItem,
        1 => Stats.mediumScoreItem,
        _ => Stats.maxScoreItem
    };

    public override bool Power(Character character)
    {
        float startAmount = amount;
        character.AddVitalux(amount, out amount);

        if (amount <= 0)
        {
            toDispose = true;
        }

        if (amount < startAmount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}