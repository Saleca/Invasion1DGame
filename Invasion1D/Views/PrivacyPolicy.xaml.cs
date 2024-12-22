namespace Invasion1D.Views;

public partial class PrivacyPolicy : ContentPage
{
    public PrivacyPolicy()
    {
        InitializeComponent();
    }

    private void Back_Clicked(object sender, EventArgs e)
    {
        App.Current!.MainPage = new StartMenu();
    }

    private void OpenInBrowser_Clicked(object sender, EventArgs e)
    {
        Launcher.OpenAsync("https://saleca.github.io/Home/invasion1d/privacy-policy");
    }

    private async void PrivacyPolicyWebView_Navigated(object sender, WebNavigatedEventArgs e)
    {
        string script = "document.body.scrollHeight;";
        var result = await PrivacyPolicyWebView.EvaluateJavaScriptAsync(script);

        if (!string.IsNullOrEmpty(result))
        {
            int contentHeight = int.Parse(result);
            PrivacyPolicyWebView.HeightRequest = contentHeight;
        }
        else
        {
            PrivacyPolicyWebView.HeightRequest = Height;
        }
    }
}