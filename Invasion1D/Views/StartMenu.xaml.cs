using Invasion1D.Controls;
using Invasion1D.Logic;

namespace Invasion1D.Views;

public partial class StartMenu : ContentPage
{
    public StartMenu()
    {
        InitializeComponent();

        //check why title property need to be set in code
        Menu.Title = "Start Menu";

        //BorderTest.InnerBackgroundColor = Colors.Red;
        //BorderTest.BorderThickness = new(1,0,1,1);
        //BorderTest.BorderColor = Colors.Transparent;
    }

    private void Start_Clicked(object sender, EventArgs e)
    {
        Game.Instance.Start(Seed.Current);
    }

    private void PrivacyPolicy_Clicked(object sender, EventArgs e)
    {
        App.Current!.MainPage = new PrivacyPolicy();
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