using Invasion1D.Helpers;

namespace Invasion1D.Controls;

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
            selected = Device;
        }
        else
        {
            AppTheme currentTheme = App.Current!.RequestedTheme;
            switch (currentTheme)
            {
                case AppTheme.Light:
                    selected = Light;
                    break;
                case AppTheme.Dark:
                    selected = Dark;
                    break;
                case AppTheme.Unspecified:
                    selected = Device;
                    break;
                default:
                    selected = Device;
                    break;
            }
        }

        selected.Style = selectedThemeButtonStyle;
    }

    private void LightButton_Clicked(object sender, EventArgs e)
    {
        selected.Style = themeButtonStyle;
        selected = Light;
        selected.Style = selectedThemeButtonStyle;
        Application.Current!.UserAppTheme = AppTheme.Light;
        device = false;
    }

    private void DeviceButton_Clicked(object sender, EventArgs e)
    {
        selected.Style = themeButtonStyle;
        selected = Device;
        selected.Style = selectedThemeButtonStyle;
        Application.Current!.UserAppTheme = AppTheme.Unspecified;
        device = true;
    }

    private void DarkButton_Clicked(object sender, EventArgs e)
    {
        selected.Style = themeButtonStyle;
        selected = Dark;
        selected.Style = selectedThemeButtonStyle;
        Application.Current!.UserAppTheme = AppTheme.Dark;
        device = false;
    }
}