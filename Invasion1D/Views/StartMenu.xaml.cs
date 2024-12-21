using Invasion1D.Logic;

namespace Invasion1D.Views;

public partial class StartMenu : ContentPage
{
    public StartMenu()
    {
        InitializeComponent();
    }

    private void Start_Clicked(object sender, EventArgs e)
    {
        Game.Instance.Start(0, false);
    }

    private void Tutorial_Clicked(object sender, EventArgs e)
    {
        Game.Instance.Start(0, true);
    }

    private void PrivacyPolicy_Clicked(object sender, EventArgs e)
    {
        Launcher.OpenAsync("https://saleca.github.io/Home/invasion1d/privacy-policy");
    }
}