namespace Invasion1D.Controls;

public partial class VariableBorder : ContentView
{
    public static readonly BindableProperty InnerContentProperty =
       BindableProperty.Create(nameof(InnerContent), typeof(View), typeof(VariableBorder), null);
    public View? InnerContent
    {
        get => (View)GetValue(InnerContentProperty);
        set
        {
            SetValue(InnerContentProperty, value);
            InnerContentView.Content = value;
        }
    }

    public static readonly BindableProperty InnerBackgroundColorProperty =
        BindableProperty.Create(nameof(InnerBackgroundColor), typeof(Color), typeof(VariableBorder), default(Color));
    public Color InnerBackgroundColor
    {
        get => (Color)GetValue(InnerBackgroundColorProperty);
        set
        {
            SetValue(InnerBackgroundColorProperty, value);
            InnerContentView.BackgroundColor = value;
        }
    }

    public static readonly BindableProperty BorderThicknessProperty =
        BindableProperty.Create(nameof(BorderThickness), typeof(Thickness), typeof(VariableBorder), default(Thickness));
    public Thickness BorderThickness
    {
        get => (Thickness)GetValue(BorderThicknessProperty);
        set
        {
            SetValue(BorderThicknessProperty, value);
            Padding = value;
        }
    }

    public static readonly BindableProperty BorderColorProperty =
       BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(VariableBorder), default(Color));
    public Color BorderColor
    {
        get => (Color)GetValue(BorderColorProperty);
        set
        {
            SetValue(BorderColorProperty, value);
            BackgroundColor = value;
        }
    }

    public VariableBorder()
    {
        InitializeComponent();
    }
}