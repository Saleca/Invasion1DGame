using Microsoft.Extensions.Logging;

namespace Invasion1DGame
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			/*PageHandler.Mapper.AppendToMapping("MyPageCustomization", (handler, view) =>
			{
#if WINDOWS
				handler.PlatformView.KeyDown += Windows_KeyDown;
#endif
			});*/

			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				});

#if DEBUG
			builder.Logging.AddDebug();
#endif

			return builder.Build();
		}

		/*private static void Windows_KeyDown(object sender, KeyRoutedEventArgs e)
		{
			switch (e.Key)
			{
				case Windows.System.VirtualKey.A:
					MainPage.Instance.PlayerMove(false);
					break;
				case Windows.System.VirtualKey.D:
					MainPage.Instance.PlayerMove(true);
					break;
				case Windows.System.VirtualKey.S:
					MainPage.Instance.PlayerAttack();
					break;
				case Windows.System.VirtualKey.M:
					MainPage.Instance.ChangeMapMode();
					break;
			}
			e.Handled = true;
		}*/
	}
}
