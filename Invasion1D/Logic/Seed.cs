namespace Invasion1D.Logic;

internal static class Seed
{
    static readonly Random random;

    static int seed = 0;
    public static int Current
    {
        get => seed;
        set => seed = value;
    }

    static Seed()
    {
        random = new((int)Math.Round((DateTime.Now - DateTime.Today).TotalMinutes));
        seed = random.Next(int.MaxValue);
    }

    public static int New()
    {
        seed = random.Next(int.MaxValue);
        return seed;
    }
}