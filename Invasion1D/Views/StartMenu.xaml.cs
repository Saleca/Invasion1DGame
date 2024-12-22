using Invasion1D.Logic;

namespace Invasion1D.Views;

public partial class StartMenu : ContentPage
{
    int seed;
    public StartMenu()
    {
        InitializeComponent();
        Random random = new();
        seed = random.Next(10000);
        SeedEntry.Text = seed.ToString();
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
        App.Current!.MainPage = new PrivacyPolicy();
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

    private void BackButton_Clicked(object? sender, EventArgs e)
    {
        TutorialIntro.IsVisible = false;
    }

    private void SeedEntry_Completed(object? sender, EventArgs e)
    {
        SeedEntry.Unfocus();
    }
    private void SeedEntry_Focused(object? sender, FocusEventArgs e)
    {
        SeedEntry.Focused -= SeedEntry_Focused;
        SeedEntry.Unfocus();
    }
}