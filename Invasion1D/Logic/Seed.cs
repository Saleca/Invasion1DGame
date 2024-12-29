namespace Invasion1D.Logic;

internal static class Seed
{
    static int seed = 0;
    public static int Current
    {
        get => seed;
        set => seed = value;
    }

    static Seed()
    {
        Random random = new();
        seed = random.Next(10000);
    }
}