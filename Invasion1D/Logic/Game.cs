using Invasion1D.Helpers;
using Invasion1D.Models;
using Invasion1D.Views;
using System.Diagnostics;

namespace Invasion1D.Logic;

internal class Game
{
    readonly static Game instance = new();
    public static Game Instance => instance;
    Game() { } //private constructor

    public Invasion1dUI UI = null!;
    public Random Fate = null!;
    public Universe universe = null!;

    //state
    CancellationTokenSource cancelUpdate = null!;
    readonly List<Kinetic> objectsToUpdateUI = [];
    public Stopwatch stopwatch = null!;

    int seed = 0;
    bool isTutorial = false;

    bool
        isStarted = false,
        isPaused = false;

    public int Seed => seed;
    public bool IsTutorial => isTutorial;
    public bool IsPaused => isPaused;

    public void Start(int seed = 0, bool isTutorial = false)
    {
        if (isStarted)
        {
            Stop();
        }

        this.seed = seed;
        this.isTutorial = isTutorial;

        UI = new Invasion1dUI();
        Fate = new(seed);
        universe = new();
        universe.Initiate();

        //TODO
        //select shape on map to start player on that shape

        UI.ShowStats(true);
        UI.ShowControls(true);
        cancelUpdate = new();

        universe.Start();
        stopwatch = Stopwatch.StartNew();
        isPaused = false;
        App.Current!.MainPage = UI;
        Task.Run(Update);

        UI.ShowPauseButton(true);
        isStarted = true;
    }

    async Task Update()
    {
        while (cancelUpdate is not null
            && !cancelUpdate.IsCancellationRequested)
        {
            if (isPaused)
            {
                await Task.Delay(100, cancelUpdate.Token);
                continue;
            }
            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                UpdateGameObjects();
                ManageDisposedGameObjects();

                Task uiTask = MainThread.InvokeOnMainThreadAsync(() =>
                {
                    UpdateGameObjectsInUI();
                    if (!UI.IsAnimating)
                    {
                        universe.player.GetView(
                            view: out Color? view,
                            rearView: out Color? rearView);
                        UI.UpdateView(view, rearView);
                    }
                    UI.UpdateTime(stopwatch.Elapsed.CustomToString());
                    UI.UpdateEnemyCountLabel($"{universe.enemies.Count}/{universe.initialEnemyCount}");
                });

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

            int frameTime = 100 - (int)sw.ElapsedMilliseconds;
            if (frameTime > 0)
            {
                await Task.Delay(frameTime, cancelUpdate.Token);
            }
        }
    }

    public void Pause(bool pause)
    {
        isPaused = pause;
        if (pause)
        {
            stopwatch.Stop();
        }
        else
        {
            stopwatch.Start();
        }
    }

    public void Stop()
    {
        CancelUpdate();
        stopwatch.Stop();
        universe.Stop();
    }

    public void End()
    {
        Stop();

        UI.RunOnUIThread(() =>
        {
            UI.UpdateView(GameColors.VoidColor, GameColors.VoidColor);

            UI.ShowContinueButton(false);
            UI.ShowPopUpMenu(text: "Game Over");
            UI.ShowControls(false);
            UI.ShowPauseButton(false);
        });
    }

    private void UpdateGameObjects()
    {
        foreach (var enemy in universe.enemies)
        {
            enemy.Tick();//temporary till i abstract the update logic 
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
            bullet.Tick();
            bullet.Move();
            objectsToUpdateUI.Add(bullet);
        }

        universe.player.Tick();
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
        objectsToUpdateUI.Clear();
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
    public bool RandomDirection() => Fate.Next(2) == 1;
}
