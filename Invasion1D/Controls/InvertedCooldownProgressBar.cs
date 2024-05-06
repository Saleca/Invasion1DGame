using Timer = System.Timers.Timer;

namespace Invasion1D.Controls
{
	public class InvertedCooldownProgressBar : InvertedProgressBar
	{
		readonly Timer timer;

		int interval;
		public int Interval
		{
			get => interval;
			set
			{
				interval = value;
				if (timer.Interval != interval)
				{
					timer.Interval = value;
				}
			}
		}

		public float Increment { get; set; }
		public bool Cancel { get; set; }
		public EventHandler CooldownCompleted = null!;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="color">Color of the progress bar</param>
		/// <param name="interval">Interval in miliseconds to update the automated cooldown</param>
		/// <param name="increment">Increment of the progress bar at the end of each interval</param>
		public InvertedCooldownProgressBar(Color color, int interval, float increment) : base(color)
		{
			Progress = 0;

			Cancel = false;
			timer = new(interval);
			timer.Elapsed += (s, e) => OnCooldownElapsed(null, EventArgs.Empty);

			Interval = interval;
			Increment = increment;

		}

		public void ActivateCooldown()
		{
			Game.UI.RunOnUIThread(() => Progress = 1);
			timer.Start();
		}

		protected void OnCooldownElapsed(object? sender, EventArgs e)
		{
			Game.UI.RunOnUIThread(() =>
			{
				if (Cancel)
				{
					return;
				}
				Progress -= Increment;
			});

			if (Progress <= 0)
			{
				timer.Stop();
				CooldownCompleted?.Invoke(this, EventArgs.Empty);
			}
		}

		public void Dispose()
		{
			timer.Dispose();
		}
	}
}