using Timer = System.Timers.Timer;

namespace Invasion1D.Controls
{
	public class InvertedProgressBar
		: ContentView
	{
		static App Game =>
			(App)App.Current!;

		readonly Timer? timer;
		readonly BoxView progressBar;
		readonly Frame progressBarContainer;

		float progress;
		public float Progress
		{
			get => progress;
			set
			{
				progress = value;
				double width = progressBarContainer.Width;

				//check if width was "measured" 
				if (width == -1)
				{
					width = 1000;
				}

				progressBar.WidthRequest = width * value;
			}
		}

		int? interval;
		public int? Interval
		{
			get => interval;
			set
			{
				interval = value;
				if (timer != null)
				{
					timer.Interval = value ?? 0;
				}
			}
		}

		public float? Increment { get; set; }
		public bool? Cancel { get; set; }
		public EventHandler CooldownCompleted = null!;

		public InvertedProgressBar(Color color, int? interval = null, float? increment = null)
		{
			progressBar = new()
			{
				BackgroundColor = color
			};

			progressBarContainer = new()
			{
				Content = progressBar
			};

			Content = progressBarContainer;

			Progress = 0;

			if (interval != null && increment != null)
			{
				Cancel = false;
				Interval = interval;
				Increment = increment;
				timer = new(interval.Value);
				timer.Elapsed += (s, e) => OnCooldownElapsed(null, EventArgs.Empty);
			}
		}

		public void ActivateCooldown()
		{
			if (timer != null)
			{
				Game.UI.RunOnUIThread(() => Progress = 1);
				timer.Start();
			}
		}

		protected void OnCooldownElapsed(object? sender, EventArgs e)
		{
			if (timer != null && Increment != null && Cancel != null)
			{
				Game.UI.RunOnUIThread(() =>
				{
					if (Cancel.Value)
					{
						return;
					}
					Progress -= Increment.Value;
				});

				if (Progress <= 0)
				{
					timer.Stop();
					CooldownCompleted?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		public void Dispose()
		{
			timer?.Dispose();
		}
	}
}