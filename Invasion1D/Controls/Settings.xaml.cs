namespace Invasion1D.Controls;

public partial class Settings : ContentView
{
	public Settings()
	{
		InitializeComponent();
	}
    public Settings(bool seedReadOnly)
    {
        InitializeComponent();
        SeedViewControl.IsReadOnly = seedReadOnly;
    }
}