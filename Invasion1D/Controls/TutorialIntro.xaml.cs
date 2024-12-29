using Invasion1D.Logic;
using Invasion1D.Views;

namespace Invasion1D.Controls;

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