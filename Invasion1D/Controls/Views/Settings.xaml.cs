namespace Invasion1D.Controls.Views;

public partial class Settings : ContentView
{
    public Settings()
    {
        InitializeComponent();
    }

    public Settings(bool seedReadOnly)
    {
        InitializeComponent();

        if (seedReadOnly)
        {
            SeedViewControl.Disable();
        }
    }
}