using Invasion1D.Views;

namespace Invasion1D;

public partial class App : Application
{
    static PrivacyPolicy privacyPolicy;
    public static PrivacyPolicy PrivacyPolicyInstance => privacyPolicy??=new();

    public App()
    {
        InitializeComponent();
        MainPage = new StartMenu();
    }
}