using Invasion1D.Logic;

namespace Invasion1D.Controls.Views;

public partial class SeedView : Frame
{
    public SeedView()
    {
        InitializeComponent();
        SeedEntry.Text = Seed.Current.ToString();
    }

    public void Disable()
    {
        IsEnabled = false;
    }

    public void GetNewSeed(object s, EventArgs e)
    {
        SeedEntry.Text = Seed.New().ToString();
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