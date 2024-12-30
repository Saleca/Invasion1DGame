using Invasion1D.Models;

namespace Invasion1D.Helpers;

internal static class GameColors
{
    internal static Color Player { get; }
    internal static Color Enemy { get; }
    internal static Color Vitalux { get; }
    internal static Color Health { get; }
    internal static Color Weave { get; }
    internal static Color Warpium { get; }
    internal static Color Light { get; }
    internal static Color Dark { get; }


    static GameColors()
    {
        Exception exception = new("Color not found");
        if (!ResourcesInterop.TryGetResource(nameof(PlayerModel).Replace("Model", ""), out Color? playerColor)
            || !ResourcesInterop.TryGetResource(nameof(EnemyModel).Replace("Model", ""), out Color? enemyColor)
            || !ResourcesInterop.TryGetResource(nameof(VitaluxModel).Replace("Model", ""), out Color? vitaluxColor)
            || !ResourcesInterop.TryGetResource(nameof(HealthModel).Replace("Model", ""), out Color? healthColor)
            || !ResourcesInterop.TryGetResource(nameof(WeaveModel).Replace("Model", ""), out Color? weaveColor)
            || !ResourcesInterop.TryGetResource(nameof(WarpiumModel).Replace("Model", ""), out Color? warpiumColor)
            || !ResourcesInterop.TryGetResource("Light", out Color? LightColor)
            || !ResourcesInterop.TryGetResource("Dark", out Color? DarkColor))
        {
            throw exception;
        }

        Player = playerColor!;
        Enemy = enemyColor!;
        Vitalux = vitaluxColor!;
        Health = healthColor!;
        Weave = weaveColor!;
        Warpium = warpiumColor!;
        Light = LightColor!;
        Dark = DarkColor!;
    }

    internal static Color LinearInterpolation(Color a, Color b, float i)
    {
        float
            red = GameMath.LinearInterpolation(a.Red, b.Red, i),
            green = GameMath.LinearInterpolation(a.Green, b.Green, i),
            blue = GameMath.LinearInterpolation(a.Blue, b.Blue, i);

        return new Color(red, green, blue);
    }
    internal static Color EaseInInterpolation(Color a, Color b, float i)
    {
        float
            red = GameMath.EaseInInterpolation(a.Red, b.Red, i),
            green = GameMath.EaseInInterpolation(a.Green, b.Green, i),
            blue = GameMath.EaseInInterpolation(a.Blue, b.Blue, i);

        return new Color(red, green, blue);
    }

    internal static Color? CalculateView(float distance, Color? color)
    {
        if (color is null) return null;

        Color? viewColor = null;

        float
            minDistance = 0,
            maxDistance = 50;

        if (distance < maxDistance)
        {
            float i = GameMath.Normalize(minDistance, maxDistance, distance);
            viewColor = EaseInInterpolation(color, VoidColor, i);
        }
        return viewColor;
    }

    public static Color VoidColor => Application.Current?.RequestedTheme switch
    {
        AppTheme.Dark => Dark,
        AppTheme.Light => Light,
        _ => Dark,
    };
}