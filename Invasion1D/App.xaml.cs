using Invasion1D.Pages;

namespace Invasion1D;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new StartMenu();
    }
}