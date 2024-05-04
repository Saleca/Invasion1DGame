using Invasion1D.Helpers;
using Invasion1D.Models;
using System.Diagnostics;

namespace Invasion1D
{
	public partial class App : Application
	{
		//diagnostics
		readonly bool debugExceptions = false;

		//
		public Universe universe = null!;
		public MainPage UI = null!;
		public Random throwDice = new();

		//state
		bool isStarted = false;
		CancellationTokenSource cancelUpdate = null!;

		public App()
		{
			InitializeComponent();

			UI = new();
			MainPage = UI;

			if (debugExceptions)
			{
				AppDomain.CurrentDomain.FirstChanceException += FirstChanceException;
			}
		}

		public void Start()
		{
			if (isStarted)
			{
				Stop();
				Reset();
			}
			else
			{
				UI.UpdateStartKeyText("Restart");
			}
			universe = new();
			universe.Initiate();
			cancelUpdate = new();

			universe.Start();
			UI.ShowStats(true);
			UI.ShowControls(true);
			Task.Run(Update);

			isStarted = true;
		}

		public void CancelUpdate()
		{
			if (cancelUpdate is null || cancelUpdate.IsCancellationRequested)
				return;

			cancelUpdate.Cancel();
			cancelUpdate.Dispose();
		}

		async Task Update()
		{
			while (!cancelUpdate.IsCancellationRequested)
			{
				//TODO
				//automate enemies

				try
				{
					int enemyCount = universe.dimensions.SelectMany(d => d.interactiveObjects.OfType<Enemy>()).Count();

					Task uiTask = MainThread.InvokeOnMainThreadAsync(() =>
					{
						if (!UI.IsAnimating)
						{
							UI.UpdateView(universe.playerData.GetView());
						}
						UI.UpdateTime(universe.stopwatch.Elapsed.CustomToString());
						UI.UpdateEnemies($"{enemyCount}/9");
					});
					await Task.Delay(100, cancelUpdate.Token);
					await uiTask;
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"An error occurred: {ex.Message}");
					break;
				}
			}
		}
		public void Stop()
		{
			CancelUpdate();
			UI.ClearCoolDownButtons();
		}
		public void End()
		{
			Stop();

			UI.UpdateView(GameColors.VoidColor);
			UI.ShowText(text: "Game Over");
			UI.ShowControls(false);
		}

		public void Reset()
		{
			UI.ShowText(false);
			UI.ResetAnimation();
			universe.ResetDimentions();
			UI.ClearMap();
			UI.ClearWarpium();
			UI.ClearWeave();
		}

		private void FirstChanceException(object? sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
		{
			Debug.WriteLine(e.Exception.ToString());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>true for clockwise or false for !clockwise</returns>
		public bool RandomDirection() => throwDice.Next(2) == 1;
	}
}