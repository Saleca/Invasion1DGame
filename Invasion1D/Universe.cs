using Invasion1D.Data;
using Invasion1D.Helpers;
using Invasion1D.Models;
using Microsoft.Maui.Controls.Shapes;
using System.Diagnostics;

namespace Invasion1D
{
	public class Universe
	{
		static App Game => ((App)Application.Current!);

		public readonly List<Dimension> dimensions = [];
		public Player playerData = null!;

		//TODO
		//check if better use datetime and span to display time
		public Stopwatch stopwatch = null!;

		public void Initiate()
		{
			_ = new Seed();
			double pp = .9f;
			playerData = new((Circular)dimensions[0], pp, 10);

			//TODO
			//select shape on map to start player on that shape

			Game.UI.UpdateView(playerData.GetView());
			Game.UI.Draw();
		}

		public void Start()
		{
			stopwatch = Stopwatch.StartNew();
		}

		public void ResetDimentions()
		{
			dimensions.Clear();
		}

		public void PlayerMove(bool dir)
		{
			if (dir)
			{
				playerData.PositiveMove();
			}
			else
			{
				playerData.NegativeMove();
			}
		}

		public void StopPlayer() => playerData.StopMovement();
		public void PlayerAttack() => playerData.Attack();
		public void WarpPlayer() => playerData.WarpAsync();
	}
}