namespace Invasion1D.Controls;

public partial class PopupMenu : Frame
{
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(PopupMenu), default(string));
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set
        {
            SetValue(TitleProperty, value);
            TitleLabel.Text = value;
        }
    }

    public static readonly BindableProperty MenuContentProperty = BindableProperty.Create(nameof(MenuContent), typeof(VerticalStackLayout), typeof(PopupMenu), default(VerticalStackLayout));
    public VerticalStackLayout MenuContent
    {
        get => (VerticalStackLayout)GetValue(MenuContentProperty);
        set
        {
            SetValue(MenuContentProperty, value);
            MenuContentView.Content = value;
        }
    }

    public static readonly BindableProperty ModalProperty = BindableProperty.Create(nameof(Modal), typeof(ContentView), typeof(PopupMenu), default(ContentView?));
    public ContentView? Modal
    {
        get => (ContentView?)GetValue(ModalProperty);
        set
        {
            SetValue(ModalProperty, value);
            ModalView.Content = value;
            if (value != null)
            {
                ModalPage.IsVisible = true;
            }
            else
            {
                ModalPage.IsVisible = false;
            }
        }
    }

    public PopupMenu()
    {
        InitializeComponent();

        Modal = null;
        MenuContent = [];
        MenuContentView.Content = MenuContent;
    }

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        Modal = null;
    }
}