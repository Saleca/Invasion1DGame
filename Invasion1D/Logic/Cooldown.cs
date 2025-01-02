using System.Numerics;

namespace Invasion1D.Logic;

public class Cooldown(Action? start = null, Action<float>? update = null, Action? complete = null, float increment = 0, int interval = 1, bool inverted = false)
{
    readonly bool inverted = inverted;
    readonly Action?
        start = start,
        complete = complete;
    readonly Action<float>? update = update;

    bool active = false;
    public bool IsActive
    {
        get => active;
        set
        {
            if (value)
            {
                Activate();
            }
            else
            {
                Reset();
            }
        }
    }

    int interval = interval;
    float increment = increment;

    int currentInterval;

    float progress;

    public void Activate(int? interval = null, float? increment = null)
    {
        active = true;
        progress = inverted ? 1 : 0;


        if (interval != null)
        {
            this.interval = interval.Value;
        }
        if(increment != null)
        {
            this.increment = increment.Value;
        }

        currentInterval = this.interval;

        start?.Invoke();
    }

    public void Update()
    {
        if (!active)
        {
            return;
        }

        currentInterval--;
        if (currentInterval > 0)
        {
            return;
        }

        progress += inverted ? -increment : increment;
        if (progress <= 0 || progress >= 1)
        {
            progress = inverted ? 0 : 1;
            Complete();
            return;
        }
        update?.Invoke(progress);

        currentInterval = interval;
    }

    void Complete()
    {
        active = false;
        complete?.Invoke();
        return;
    }

    public void Reset()
    {
        active = false;
    }
}