using Microsoft.Extensions.Logging;

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

        /*
#if WINDOWS
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
        {
            var borderThickness = new Microsoft.UI.Xaml.Thickness(0, 0, 0, 1);
            var borderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 175, 175, 175));
            var pointerOverBorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 143, 143, 143));

            Microsoft.UI.Xaml.Controls.TextBox windowsNativeView = handler.PlatformView;
            windowsNativeView.BorderThickness = borderThickness;
            windowsNativeView.BorderBrush = borderBrush;
         
            var visualStateGroups = Microsoft.UI.Xaml.VisualStateManager.GetVisualStateGroups(windowsNativeView);

            //focused
            var focusedAnimationObjects = visualStateGroups
                .FirstOrDefault(g => g.Name == "Focused")?
                .States.FirstOrDefault(s => s.Name == "Focused")?
                .Storyboard?.Children
                .OfType<ObjectAnimationUsingKeyFrames>();

            var borderThicknessAnimation = focusedAnimationObjects?
                 .FirstOrDefault(a => a.GetType().GetProperty("TargetProperty")?.GetValue(a)?.ToString() == "(UIElement.BorderThickness)");

            if (borderThicknessAnimation != null)
            {
                borderThicknessAnimation.KeyFrames.Clear();
                borderThicknessAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame()
                {
                    KeyTime = TimeSpan.Zero,
                    Value = borderThickness // Adjust border thickness as needed
                });
            }

            var borderBrushAnimation = focusedAnimationObjects?
                .FirstOrDefault(a => a.GetType().GetProperty("TargetProperty")?.GetValue(a)?.ToString() == "(UIElement.BorderBrush)");

            if (borderBrushAnimation != null)
            {
                borderBrushAnimation.KeyFrames.Clear();
                borderBrushAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame()
                {
                    KeyTime = TimeSpan.Zero,
                    Value = borderBrush
                });
            }

            //pointerover
            var pointerOverBorderBrushAnimation = visualStateGroups
                .FirstOrDefault(g => g.Name == "PointerOver")?
                .States.FirstOrDefault(s => s.Name == "PointerOver")?
                .Storyboard?.Children
                .OfType<ObjectAnimationUsingKeyFrames>()?
                .FirstOrDefault(a => a.GetType().GetProperty("TargetProperty")?.GetValue(a)?.ToString() == "(UIElement.BorderBrush)");

            if (pointerOverBorderBrushAnimation != null)
            {
                pointerOverBorderBrushAnimation.KeyFrames.Clear();
                pointerOverBorderBrushAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame()
                {
                    KeyTime = TimeSpan.Zero,
                    Value = pointerOverBorderBrush
                });
            }

        });
#endif
        //*/

        return builder.Build();
    }
}