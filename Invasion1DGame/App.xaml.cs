namespace Invasion1DGame
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = Invasion1DGame.MainPage.Instance;
		}
	}
}
