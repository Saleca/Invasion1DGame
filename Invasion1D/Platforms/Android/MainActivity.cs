using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.AppCompat.Widget;
using Microsoft.Maui.Handlers;

namespace Invasion1D;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
        {
            AppCompatEditText androidNativeView = handler.PlatformView;
            androidNativeView.Background = null;
            androidNativeView.SetPadding(0, 0, 0, 0);
        });
    }
}