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

		List<Kinetic> objectsToUpdateUI = [];

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

		async Task Update()
		{
			while (!cancelUpdate.IsCancellationRequested)
			{
				try
				{
					UpdateGameObjects();
					ManageDisposedGameObjects();

					Task uiTask = MainThread.InvokeOnMainThreadAsync(() =>
					{
						UpdateGameObjectsUI();
						if (!UI.IsAnimating)
						{
							UI.UpdateView(universe.playerData.GetView());
						}
						UI.UpdateTime(universe.stopwatch.Elapsed.CustomToString());
						UI.UpdateEnemies($"{universe.enemyCount}/{universe.initialEnemyCount}");
					});
					//TODO:
					//measure current frame compute time and delay only the difference between prefered framerate time and compute time
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

		private void ManageDisposedGameObjects()
		{
			List<Interactive> interactiveObjectsToDispose = [];
			foreach (var dimension in universe.dimensions)
			{
				foreach (var interactiveObject in dimension.interactiveObjects)
				{
					if (interactiveObject.toDispose)
					{
						interactiveObjectsToDispose.Add(interactiveObject);
					}
				}
			}
			int toDisposeCount = interactiveObjectsToDispose.Count;
			for (int i = 0; i < toDisposeCount; i++)
			{
				if (objectsToUpdateUI.Contains(interactiveObjectsToDispose[0]))
				{
					objectsToUpdateUI.Remove((Kinetic)interactiveObjectsToDispose[0]);
				}
				interactiveObjectsToDispose[0].Dispose();
			}
		}

		private void UpdateGameObjects()
		{
			foreach (var dimension in universe.dimensions)
			{
				foreach (var interactiveObject in dimension.interactiveObjects)
				{
					if (interactiveObject is not Kinetic kineticObject)
					{
						continue;
					}

					if (kineticObject.isMoving)
					{
						kineticObject.Move();
						objectsToUpdateUI.Add(kineticObject);
					}
				}
			}
		}

		void UpdateGameObjectsUI()
		{
			foreach (var kineticObject in objectsToUpdateUI)
			{
				kineticObject.UpdateUI();
			}
			objectsToUpdateUI = [];
		}

		public void CancelUpdate()
		{
			if (cancelUpdate is null || cancelUpdate.IsCancellationRequested)
				return;

			cancelUpdate.Cancel();
			cancelUpdate.Dispose();
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
			universe.ResetDimensions();
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