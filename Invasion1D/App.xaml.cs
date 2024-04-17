using System.Diagnostics;

namespace Invasion1D
{
	public partial class App : Application
	{
		readonly bool debugExceptions = false;
		public App()
		{
			InitializeComponent();

			MainPage = new MainPage();//Invasion1D.MainPage.Instance;

			if (debugExceptions)
			{
				AppDomain.CurrentDomain.FirstChanceException += FirstChanceException;
			}
		}

		private void FirstChanceException(object? sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
		{
			Debug.WriteLine(e.Exception.ToString());
		}
	}
}