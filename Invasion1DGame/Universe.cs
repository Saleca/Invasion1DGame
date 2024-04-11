using Invasion1DGame.Data;
using Invasion1DGame.Helpers;
using Invasion1DGame.Models;
using Microsoft.Maui.Controls.Shapes;
using System.Diagnostics;

namespace Invasion1DGame
{
	public class Universe
	{
		public Random random = null!;
		public readonly List<Dimension> dimensions = [];
		Player playerData = null!;
		private CancellationTokenSource cancelUpdate = new();

		//TODO
		//check if better use datetime and span to display time
		public Stopwatch stopwatch = null!;

		public Universe()
		{
			random = new(1);
		}

		public void Initiate()
		{
			_ = new Seed();
			double pp = .9f;
			playerData = new((Circular)dimensions[0], pp, 10);

			//TODO
			//select shape on map to start player on that shape

			MainPage.Instance.UpdateView(playerData.GetView());
			MainPage.Instance.Draw();
		}

		public void Start()
		{
			stopwatch = Stopwatch.StartNew();
			cancelUpdate = new();

			Task.Run(Update);
		}

		public void ResetDimentions()
		{
			/*int dimentionsCount = dimensions.Count;
			for (int iD = 0; iD < dimentionsCount; iD++)
			{
				int interactiveObjectsCount = dimensions[0].interactiveObjects.Count;
				for (int i = 0; i < interactiveObjectsCount; i++)
				{
					dimensions[0].interactiveObjects[0].Dispose();
				}
			}*/
			dimensions.Clear();
		}

		public void CancelUpdate() => cancelUpdate.Cancel();

		private async Task Update()
		{
			while (!cancelUpdate.IsCancellationRequested)
			{
				try
				{
					await MainThread.InvokeOnMainThreadAsync(() => MainPage.Instance.UpdateUI(playerData, stopwatch.Elapsed.CustomToString()));
					await Task.Delay(100, cancelUpdate.Token);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.ToString());
				}
			}
		}

		public void PlayerMove(bool dir)
		{
			//TODO
			//darken current direction button
			if (dir)
			{
				playerData.PositiveMove();
				//PosKey.Background = Colors.DarkGray;
				//NegKey.Background = Colors.Gray;
			}
			else
			{
				playerData.NegativeMove();
				//NegKey.Background = Colors.DarkGray;
				//PosKey.Background = Colors.Gray;
			}
		}
		public void PlayerAttack() => playerData.Attack();
		public void WarpPlayer() => playerData.WarpAsync();
	}
}