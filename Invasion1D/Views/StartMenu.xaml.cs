using Invasion1D.Controls;
using Invasion1D.Logic;

namespace Invasion1D.Views;

public partial class StartMenu : ContentPage
{
    //change to settings
    public static int seed = 0;
    public StartMenu()
    {
        InitializeComponent();

        //check why title property need to be set in code
        Menu.Title = "Start Menu";

        //BorderTest.InnerBackgroundColor = Colors.Red;
        //BorderTest.BorderThickness = new(1,0,1,1);
        //BorderTest.BorderColor = Colors.Transparent;

        Random random = new();
        seed = random.Next(10000);
        SeedEntry.Text = seed.ToString();

    }

    private void Start_Clicked(object sender, EventArgs e)
    {
        Game.Instance.Start(seed);
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

        if (int.TryParse(e.NewTextValue, out int seedValue))
        {
            seed = seedValue;
            SeedEntry.Text = seed.ToString();

        }
        else
        {
            SeedEntry.Text = e.OldTextValue;
        }
    }

    private void Tutorial_Clicked(object sender, EventArgs e)
    {
        Menu.Modal = new TutorialIntro();
    }

    private void SettingsButton_Clicked(object sender, EventArgs e)
    {
        Menu.Modal = new Settings();
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