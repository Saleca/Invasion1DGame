using Invasion1D.Helpers;
using Invasion1D.Models;
using Microsoft.Maui.Primitives;
using System.Collections.Concurrent;
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
				Reset();
			}
			else
			{
				UI.UpdateStartKeyText("Restart");
			}
			universe = new();
			universe.Initiate();

			//TODO
			//select shape on map to start player on that shape

			UI.ShowStats(true);
			UI.ShowControls(true);
			cancelUpdate = new();

			universe.Start();
			Task.Run(Update);
			isStarted = true;
		}

		async Task Update()
		{
			while (!cancelUpdate.IsCancellationRequested)
			{
				try
				{
					UpdateGameObjectMovement();
					ManageDisposedGameObjects();

					Task uiTask = MainThread.InvokeOnMainThreadAsync(() =>
					{
						UpdateGameObjectsInUI();
						if (!UI.IsAnimating)
						{
							UI.UpdateView(universe.player.GetView());
						}
						UI.UpdateTime(universe.stopwatch.Elapsed.CustomToString());
						UI.UpdateEnemies($"{universe.enemies.Count}/{universe.initialEnemyCount}");
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

		public void Stop()
		{
			CancelUpdate();
			UI.RunOnUIThread(() => UI.ClearCoolDownButtons());
			universe.Stop();
		}

		public void End()
		{
			Stop();

			UI.RunOnUIThread(() =>
			{
				UI.UpdateView(GameColors.VoidColor);
				UI.ShowText(text: "Game Over");
				UI.ShowControls(false);
			});
		}

		public void Reset()
		{
			Stop();

			UI.ShowText(false);
			UI.ResetAnimation();
			universe.ResetDimensions();
			UI.ClearMap();
			UI.ClearWarpium();
			UI.ClearWeave();
		}

		private void UpdateGameObjectMovement()
		{
			foreach (var enemy in universe.enemies)
			{
				if (enemy.isMoving)
				{
					enemy.Move();
					objectsToUpdateUI.Add(enemy);
				}
			}

			foreach (var bullet in universe.bullets)
			{
				bullet.Move();
				objectsToUpdateUI.Add(bullet);
			}

			if (universe.player.isMoving)
			{
				universe.player.Move();
				objectsToUpdateUI.Add(universe.player);
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
				Interactive interactiveObjectToDispose = interactiveObjectsToDispose[0];
				if (objectsToUpdateUI.Contains(interactiveObjectToDispose))
				{
					objectsToUpdateUI.Remove((Kinetic)interactiveObjectToDispose);
				}

				switch (interactiveObjectToDispose)
				{
					case Bullet bullet:
						universe.bullets.Remove(bullet);
						break;
					case Enemy enemy:
						universe.enemies.Remove(enemy);
						break;
				}
				interactiveObjectToDispose.Dispose();
			}
		}

		void UpdateGameObjectsInUI()
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