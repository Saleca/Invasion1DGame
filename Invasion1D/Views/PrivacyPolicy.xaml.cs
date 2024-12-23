using HtmlAgilityPack;
using Windows.Media.Audio;

namespace Invasion1D.Views;

public partial class PrivacyPolicy : ContentPage
{
    const string url = "https://saleca.github.io/Home/invasion1d/privacy-policy";

    public PrivacyPolicy()
    {
        InitializeComponent();

        HtmlNode mainNode = ExtractMainContent() ?? throw new Exception();
        HtmlNode enNode = mainNode.ChildNodes[1];//extract only html nodes 
        //create button for each node class name
        //display only one at the time , associated with a button
        //format text for "document" (same for tutorial intro)
        HtmlNodeCollection nodes = enNode.SelectNodes(".//text()");
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