using Invasion1D.Helpers;

namespace Invasion1D.Controls.Views;

public partial class ThemeSelector : Frame
{
    static Style themeButtonStyle, selectedThemeButtonStyle;
    Button selected;

    //move to settings
    static bool device = true;
    static ThemeSelector()
    {
        if (!ResourcesInterop.TryGetResource("ThemeButton", out Style? themeButton)
        || !ResourcesInterop.TryGetResource("SelectedThemeButton", out Style? selectedThemeButton))
        {
            throw new Exception();
        }
        themeButtonStyle = themeButton!;
        selectedThemeButtonStyle = selectedThemeButton!;
    }

    public ThemeSelector()
    {
        InitializeComponent();
        if (device)
        {
            selected = DeviceButton;
        }
        else
        {
            AppTheme currentTheme = App.Current!.RequestedTheme;
            selected = currentTheme switch
            {
                AppTheme.Light => LightButton,
                AppTheme.Dark => DarkButton,
                AppTheme.Unspecified => DeviceButton,
                _ => DeviceButton,
            };
        }

        selected.Style = selectedThemeButtonStyle;
    }

    private void LightButton_Clicked(object sender, EventArgs e)
    {
        selected.Style = themeButtonStyle;
        selected = LightButton;
        selected.Style = selectedThemeButtonStyle;
        Application.Current!.UserAppTheme = AppTheme.Light;
        device = false;
    }

    private void DeviceButton_Clicked(object sender, EventArgs e)
    {
        selected.Style = themeButtonStyle;
        selected = DeviceButton;
        selected.Style = selectedThemeButtonStyle;
        Application.Current!.UserAppTheme = AppTheme.Unspecified;
        device = true;
    }

    private void DarkButton_Clicked(object sender, EventArgs e)
    {
        selected.Style = themeButtonStyle;
        selected = DarkButton;
        selected.Style = selectedThemeButtonStyle;
        Application.Current!.UserAppTheme = AppTheme.Dark;
        device = false;
    }
}