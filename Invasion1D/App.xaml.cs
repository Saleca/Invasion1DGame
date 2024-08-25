﻿using Invasion1D.Helpers;
using Invasion1D.Models;
using System.Diagnostics;

namespace Invasion1D
{
    public partial class App : Application
    {
        //diagnostics
        readonly bool
            debugExceptions = false;

        //Global variables
        public Universe
            universe = null!;
        public MainPage
            UI = null!;
        public Random
            throwDice = new();

        //state
        bool
            isStarted = false,
            isPaused = false;
        CancellationTokenSource
            cancelUpdate = null!;
        List<Kinetic>
            objectsToUpdateUI = [];

        public bool IsPaused => isPaused;
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
                UI.Initiate();
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

            UI.ShowPauseButton(true);
            isStarted = true;
        }

        async Task Update()
        {
            //add !isPaused 
            while (!cancelUpdate.IsCancellationRequested)
            {
                try
                {
                    UpdateGameObjects();
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

        public void Pause(bool pause)
        {
            isPaused = pause;
            /*foreach (var enemy in universe.enemies)
            {
                enemy.Pause(IsPaused);
            }
            foreach (var bullet in universe.bullets)
            {
                bullet.Pause(IsPaused);
            }*/
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
                UI.ShowPopUpMenu(text: "Game Over");
                UI.ShowControls(false);
                UI.ShowPauseButton(false);
            });
        }

        public void Reset()
        {
            Stop();
            isPaused = false;
            UI.ShowPopUpMenu(false);
            UI.ResetAnimation();
            universe.ResetDimensions();
            UI.ClearMap();
            UI.ClearWarpium();
            UI.ClearWeave();
        }

        private void UpdateGameObjects()
        {
            foreach (var enemy in universe.enemies)
            {
                if (enemy.toReact)
                {
                    enemy.React();
                }
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
                lock (dimension.interactiveObjects)
                {
                    foreach (var interactiveObject in dimension.interactiveObjects)
                    {
                        if (interactiveObject.toDispose)
                        {
                            interactiveObjectsToDispose.Add(interactiveObject);
                        }
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
                    case EnemyModel enemy:
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