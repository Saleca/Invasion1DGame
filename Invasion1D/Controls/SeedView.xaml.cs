using Invasion1D.Logic;
using Microsoft.UI.Xaml.Documents;

namespace Invasion1D.Controls;

public partial class SeedView : Frame
{
    public static readonly BindableProperty IsReadOnlyProperty = BindableProperty.Create(nameof(IsReadOnly), typeof(bool), typeof(PopupMenu), default(bool));
    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set
        {
            SetValue(IsReadOnlyProperty, value);
            SeedEntry.IsEnabled = !value;
            if (!value)
            {
                UnderLineControl.UnderlineColor = Colors.Transparent;
                SeedEntry.HorizontalTextAlignment = TextAlignment.Center;
                //remove pointover underline
            }
            else
            {
                //handle underline with default color
                SeedEntry.HorizontalTextAlignment = TextAlignment.Start;
            }
        }
    }

    public SeedView()
    {
        InitializeComponent();
    }

    private void SeedEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue == "")
        {
            SeedEntry.Text = "0";
            return;
        }

        if (int.TryParse(e.NewTextValue, out int seedValue))
        {
            Seed.Current = seedValue;
            SeedEntry.Text = seedValue.ToString();
        }
        else
        {
            SeedEntry.Text = e.OldTextValue;
        }
    }
    private void SeedEntry_Completed(object? sender, EventArgs e)
    {
        SeedEntry.Unfocus();
    }
    private void SeedEntry_Focused(object? sender, FocusEventArgs e)
    {
        SeedEntry.Focused -= SeedEntry_Focused;
        SeedEntry.Unfocus();
    }
}