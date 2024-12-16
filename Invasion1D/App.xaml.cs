using Invasion1D.Views;

namespace Invasion1D;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new StartMenu();
    }
}