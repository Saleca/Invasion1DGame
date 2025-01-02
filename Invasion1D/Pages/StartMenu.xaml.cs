using Invasion1D.Logic;
using Invasion1D.Controls.Views;

namespace Invasion1D.Pages;

public partial class StartMenu : ContentPage
{
    public StartMenu()
    {
        InitializeComponent();
    }

    private void Start_Clicked(object sender, EventArgs e)
    {
        Game.Instance.Start(Seed.Current);
    }

    private void Tutorial_Clicked(object sender, EventArgs e)
    {
        Menu.Modal = new TutorialIntro();
    }

    private void SettingsButton_Clicked(object sender, EventArgs e)
    {
        Menu.Modal = new Settings();
    }

    private void PrivacyPolicy_Clicked(object sender, EventArgs e)
    {
        Menu.Modal = new PrivacyPolicy();
    }



}