using Invasion1D.Logic;

namespace Invasion1D.Views;

public partial class StartMenu : ContentPage
{
    int seed;
    public StartMenu()
    {
        InitializeComponent();
    }

    private void Start_Clicked(object sender, EventArgs e)
    {

        Game.Instance.Start(seed);
    }

    private void Tutorial_Clicked(object sender, EventArgs e)
    {
        Game.Instance.Start(seed, true);
    }

    private void PrivacyPolicy_Clicked(object sender, EventArgs e)
    {
        Launcher.OpenAsync("https://saleca.github.io/Home/invasion1d/privacy-policy");
    }

    private void SeedEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (int.TryParse(e.NewTextValue, out int seed))
        {
            this.seed = seed;
        }
        else
        {
            SeedEntry.Text = e.OldTextValue;
        }
    }
}