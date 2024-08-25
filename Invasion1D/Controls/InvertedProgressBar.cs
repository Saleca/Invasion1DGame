namespace Invasion1D.Controls
{
	public class InvertedProgressBar
		: ContentView
	{
		protected static App Game =>
			(App)App.Current!;

		readonly BoxView progressBar;
		readonly Frame progressBarContainer;

		float progress;
		/// <summary>
		/// Progress is a value from 0 to 1, should be set in UI thread
		/// </summary>
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="color">Color of the progress bar</param>
		public InvertedProgressBar(Color color)
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
		}
	}
}