using Invasion1D.Logic;

namespace Invasion1D.Controls.Views;

public partial class TutorialIntro : ContentView
{
	public TutorialIntro()
	{
		InitializeComponent();
	}

    private void StartTutorialButton_Clicked(object sender, EventArgs e)
    {
        Game.Instance.Start(Seed.Current, true);
    }
}