using Microsoft.Extensions.Logging;

namespace Invasion1D
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();

			builder
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				});
			//TODO: check fonts in styles, had trouble using bold fonts in styles
#if DEBUG
			builder.Logging.AddDebug();
#endif

			return builder.Build();
		}
	}
}