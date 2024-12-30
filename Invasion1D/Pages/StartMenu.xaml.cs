using Invasion1D.Controls;
using Invasion1D.Logic;
using Invasion1D.Views;

namespace Invasion1D.Pages;

public partial class StartMenu : ContentPage
{
    public StartMenu()
    {
        InitializeComponent();

        //check why title property need to be set in code
        Menu.Title = "Start Menu";
    }

    private void Start_Clicked(object sender, EventArgs e)
    {
        Game.Instance.Start(Seed.Current);
    }

    private void PrivacyPolicy_Clicked(object sender, EventArgs e)
    {
        Menu.Modal = new PrivacyPolicy();
    }

    private void Tutorial_Clicked(object sender, EventArgs e)
    {
        Menu.Modal = new TutorialIntro();
    }

    private void SettingsButton_Clicked(object sender, EventArgs e)
    {
        Menu.Modal = new Settings();
    }
}