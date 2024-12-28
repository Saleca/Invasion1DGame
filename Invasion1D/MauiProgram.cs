using Microsoft.Extensions.Logging;

#if ANDROID
using AndroidX.AppCompat.Widget;
#elif WINDOWS
using Microsoft.UI.Xaml.Controls;
#endif

namespace Invasion1D;

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

        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
        {
#if ANDROID
            AppCompatEditText androidNativeView = handler.PlatformView;
            androidNativeView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif WINDOWS
            /*
            TextBox windowsNativeView = handler.PlatformView;
            windowsNativeView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0, 0, 0, 1);
            windowsNativeView.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(50, 255, 255, 255));
            */
#endif
        });

        return builder.Build();
    }
}