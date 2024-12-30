using HtmlAgilityPack;
using Invasion1D.Helpers;
using Invasion1D.Pages;

namespace Invasion1D.Views;

public partial class PrivacyPolicy : ContentView
{
    const string url = "https://saleca.github.io/Home/invasion1d/privacy-policy";

    readonly static Style
        documentTextStyle,
        documentSubTitleStyle,
        documentTitleStyle,
        menuButtonStyle;

    readonly Dictionary<string, List<Label>> langContent = [];
    string currentContentDisplayed = string.Empty;

    static PrivacyPolicy()
    {
        if (!ResourcesInterop.TryGetResource("DocumentText", out Style? textStyle)
           || !ResourcesInterop.TryGetResource("DocumentSubTitle", out Style? subTitleStyle)
           || !ResourcesInterop.TryGetResource("DocumentTitle", out Style? titleStyle)
           || !ResourcesInterop.TryGetResource("MenuButton", out Style? menuButton))
        {
            throw new Exception();
        }

        documentTextStyle = textStyle!;
        documentSubTitleStyle = subTitleStyle!;
        documentTitleStyle = titleStyle!;
        menuButtonStyle = menuButton!;
    }

    public PrivacyPolicy()
    {
        InitializeComponent();
        FormatPrivacyPolicy();
    }

    private void FormatPrivacyPolicy()
    {
        HtmlNode? mainNode = ExtractMainContent();
        if (mainNode == null)
        {
            Contents.Add(new Label()
            {
                Text = "An error corrued please check you internet connectivity or visit the website.",
                Style = documentTextStyle,
            });
            Contents.Add(new Label()
            {
                Text = url,
                Style = documentTextStyle,
            });
            return;
        }

        HtmlNode[] langNodes = mainNode!.ChildNodes.Where(x => x.NodeType is HtmlNodeType.Element).ToArray();

        ColumnDefinitionCollection langColumns = [];

        bool isVisible = true;
        for (int i = 0; i < langNodes.Length; i++)
        {
            HtmlNode langNode = langNodes[i];
            langColumns.Add(new());
            string langName = langNode.GetClasses().First();
            if (i == 0)
            {
                currentContentDisplayed = langName;
            }
            Button langButton = new()
            {
                Text = langName,
                Style = menuButtonStyle
            };

            langButton.Clicked += (o, s) =>
            {
                List<Label> currentContentLabels = langContent[currentContentDisplayed!];
                foreach (var contentLabel in currentContentLabels)
                {
                    contentLabel.IsVisible = false;
                }

                string langToDisplay = ((Button)o!).Text;
                List<Label> contentLabels = langContent[langToDisplay];
                foreach (var contentLabel in contentLabels)
                {
                    contentLabel.IsVisible = true;
                }
                currentContentDisplayed = langToDisplay;
            };
            Grid.SetColumn(langButton, i);
            LangButtons.Add(langButton);

            langContent[langName] = [];
            for (int t = 0; t < langNode.ChildNodes.Count; t++)
            {
                HtmlNode contentNode = langNode.ChildNodes[t];
                string text = contentNode.InnerText.CleanElementTextContents();
                if (text == string.Empty)
                {
                    continue;
                }

                Style style = null!;
                if (contentNode.Name == "h1")
                {
                    style = documentTitleStyle;
                }
                else if (contentNode.Name == "h2")
                {
                    style = documentSubTitleStyle;
                }
                else if (contentNode.Name == "p")
                {
                    //add margins to style (windows vs android)
                    style = documentTextStyle;
                }

                Label content = new()
                {
                    Text = text,
                    IsVisible = isVisible,
                    Style = style
                };

                langContent[langName].Add(content);

                Contents.Add(content);
            }
            isVisible = false;
        }
        LangButtons.ColumnDefinitions = langColumns;
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
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            return null;
        }
        //check if connection exists
        HtmlDocument doc = web.Load(url);

        HtmlNode mainNode = doc.DocumentNode.SelectSingleNode("//main");

        if (mainNode != null)
        {
            return mainNode;
        }

        return null;
    }
}