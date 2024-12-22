using HtmlAgilityPack;

namespace Invasion1D.Views;

public partial class PrivacyPolicy : ContentPage
{
    const string url = "https://saleca.github.io/Home/invasion1d/privacy-policy";

    public PrivacyPolicy()
    {
        InitializeComponent();

        HtmlNode mainNode = ExtractMainContent() ?? throw new Exception();

        HtmlNodeCollection nodes = mainNode.SelectNodes(".//text()");
        for (int i = 0; i < nodes.Count; i++)
        {
            //if nodes[i] is tittle set style ...
            DisplayLayout.Insert(
                index: i,
                child: new Label()
                    {
                        Text = nodes[i].InnerText.Trim()
                    }); 
        }
    }

    private void Back_Clicked(object sender, EventArgs e)
    {
        App.Current!.MainPage = new StartMenu();
    }

    private void OpenInBrowser_Clicked(object sender, EventArgs e)
    {
        Launcher.OpenAsync(url);
    }

    public static HtmlNode? ExtractMainContent()
    {
        HtmlWeb web = new();
        HtmlDocument doc = web.Load(url);

        HtmlNode mainNode = doc.DocumentNode.SelectSingleNode("//main");

        if (mainNode != null)
        {
            return mainNode;
        }

        return null;
    }
}