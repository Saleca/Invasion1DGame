using Invasion1D.Data;
using Invasion1D.Helpers;
using Invasion1D.Models;
using Microsoft.Maui.Controls.Shapes;
using System.Diagnostics;

namespace Invasion1D
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
			if (dir)
			{
				playerData.PositiveMove();
			}
			else
			{
				playerData.NegativeMove();
			}
		}
		public void PlayerAttack() => playerData.Attack();
		public void WarpPlayer() => playerData.WarpAsync();
	}
}