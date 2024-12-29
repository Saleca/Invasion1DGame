namespace Invasion1D.Controls;

public partial class UnderLineEntry : Grid
{
    public static readonly BindableProperty UnderlineColorProperty = BindableProperty.Create(nameof(UnderlineColor), typeof(Color), typeof(UnderLineEntry), default(Color));
    public Color UnderlineColor
    {
        get => (Color)GetValue(UnderlineColorProperty);
        set
        {
            SetValue(UnderlineColorProperty, value);
            EntryUnderline.BackgroundColor = value;
        }
    }

    public static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(View), typeof(UnderLineEntry), default(View));
    public View Content
    {
        get => (View)GetValue(ContentProperty);
        set
        {
            SetValue(ContentProperty, value);
            Container.Content = value;
        }
    }

    public UnderLineEntry()
    {
        InitializeComponent();
    }
}