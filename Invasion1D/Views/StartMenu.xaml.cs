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
        TutorialIntro.IsVisible = true;
    }

    private void PrivacyPolicy_Clicked(object sender, EventArgs e)
    {
        Launcher.OpenAsync("https://saleca.github.io/Home/invasion1d/privacy-policy");
    }

    private void SeedEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue == "")
        {
            SeedEntry.Text = "0";
            return;
        }

        if (int.TryParse(e.NewTextValue, out int seed))
        {
            this.seed = seed;
            SeedEntry.Text = seed.ToString();

        }
        else
        {
            SeedEntry.Text = e.OldTextValue;
        }
    }

    private void StartTutorialButton_Clicked(object sender, EventArgs e)
    {
        Game.Instance.Start(seed, true);
    }

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        TutorialIntro.IsVisible = false;
    }
}