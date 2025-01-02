namespace Invasion1D.Controls.Views;

public partial class InfoItem : Frame
{
    public static readonly BindableProperty ItemColorProperty = BindableProperty.Create(nameof(ItemColor), typeof(Color), typeof(InfoItem), default(Color));
    public Color ItemColor
    {
        get => (Color)GetValue(ItemColorProperty);
        set
        {
            SetValue(ItemColorProperty, value);
            ItemColorView.BackgroundColor = value;
        }
    }
    public static readonly BindableProperty ItemNameProperty = BindableProperty.Create(nameof(ItemName), typeof(string), typeof(InfoItem), default(string));
    public string ItemName
    {
        get => (string)GetValue(ItemNameProperty);
        set
        {
            SetValue(ItemNameProperty, value);
            ItemNameView.Text = value;
        }
    }

    public InfoItem()
    {
        InitializeComponent();
        ItemColorView.SetBinding(BackgroundColorProperty, new Binding(nameof(ItemColor), source: this));
        ItemNameView.SetBinding(Label.TextProperty, new Binding(nameof(ItemName), source: this));

    }
}