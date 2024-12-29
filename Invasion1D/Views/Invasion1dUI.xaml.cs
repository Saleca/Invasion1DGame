using Invasion1D.Controls;
using Invasion1D.Data;
using Invasion1D.Helpers;
using Invasion1D.Logic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using System.Diagnostics;

namespace Invasion1D.Views;

public partial class Invasion1dUI : ContentPage
{
    readonly object locker = new();
    bool isAnimating = false;
    public bool IsAnimating
    {
        get
        {
            lock (locker)
            {
                return isAnimating;
            }
        }
        set
        {
            lock (locker)
            {
                isAnimating = value;
            }
        }
    }

    public Frame
        MainFrameAccess => MainFrame;
    public AbsoluteLayout
        MapViewAccess => MapView;

    readonly Style selectedButtonStyle;
    public Invasion1dUI()
    {
        InitializeComponent();

        if (!ResourcesInterop.TryGetResource("SelectedButton", out Style? selectedButtonStyle))
        {
            throw new Exception();
        }

        this.selectedButtonStyle = selectedButtonStyle!;

        MainFrame.SizeChanged += ViewSizeChanged;
        MapView.SizeChanged += InitializeMap;

        HealthProgressBar.SizeChanged += InitializeHealthProgressBar;
        VitaluxProgressBar.SizeChanged += InitializeVitaluxProgressBar; ;
    }

    private void InitializeVitaluxProgressBar(object? sender, EventArgs e)
    {
        VitaluxProgressBar.Progress = 1;
        VitaluxProgressBar.SizeChanged -= InitializeVitaluxProgressBar; ;
    }

    private void InitializeHealthProgressBar(object? sender, EventArgs e)
    {
        HealthProgressBar.Progress = 1;
        HealthProgressBar.SizeChanged -= InitializeHealthProgressBar;
    }

    public void Draw()
    {
        foreach (var dimension in Game.Instance.universe.dimensions)
        {
            MapView.Add(dimension.body);

            foreach (var interactiveObj in dimension.interactiveObjects)
            {
                MapView.Add(interactiveObj.body);
            }
        }
    }

    public void ResetAnimation()
    {
        MapView.Scale = 1;
        MapView.TranslationX = 0;
        MapView.TranslationY = 0;
        IsAnimating = false;
    }

    public void UpdateView(Color? forwardView, Color? rearView)
    {
        forwardView ??= GameColors.VoidColor;
        rearView ??= GameColors.VoidColor;

        PlayerView.Background = new RadialGradientBrush(
            gradientStops:
            [
                new GradientStop { Color = forwardView, Offset = .75f },
                new GradientStop { Color = rearView, Offset = .9f }
            ],
            center: new(0.5, 0.5),
            radius: .66);
    }

    public void UpdateTime(string time) => TimeLabel.Text = time;

    public void UpdateEnemyCountLabel(string enemies)
    {
        EnemiesLabel.Text = enemies;
    }

    public void UpdateHealth(float progress) => HealthProgressBar.Progress = progress;
    public void UpdateVitaLux(float progress) => VitaluxProgressBar.Progress = progress;
    public void UpdateWeaveCooldown(float progress) => WeaveCooldownProgressBar.Progress = progress;
    public void UpdateShootCooldown(float progress) => ShootCooldownProgressBar.Progress = progress;
    public void UpdateWarpCooldown(float progress) => WarpCooldownProgressBar.Progress = progress;

    public void ShowShootKey(bool show) => ShootKey.IsVisible = show;
    public void ShowWarpKey(bool show) => WarpKey.IsVisible = show;

    public void AddWarpium()
    {
        WarpiumContainer.Add(new Space() { WidthRequest = 5 });
        WarpiumContainer.Add(new WarpiumControl());
    }
    public void RemoveWarpium()
    {
        WarpiumContainer.RemoveAt(0);
        WarpiumContainer.RemoveAt(0);
    }
    public void ClearWarpium() => WarpiumContainer.Clear();

    public void AddToMap(Shape shape)
    {
        Game.Instance.UI.RunOnUIThread(() => MapView.Children.Add(shape));
    }

    public void RemoveFromMap(Shape shape)
    {
        MapView.Children.Remove(shape);
    }

    public void ClearMap()
    {
        MapView.Children.Clear();
    }

    public void SelectDirection(bool direction)
    {
        if (direction)
        {
            PosKey.Style = selectedButtonStyle;
            NegKey.Style = null;
        }
        else
        {
            PosKey.Style = null;
            NegKey.Style = selectedButtonStyle;
        }
    }

    public void ShowContinueButton(bool show) => ContinueButton.IsVisible = show;
    public void ShowControls(bool show) => ControlsGrid.IsVisible = show;
    public void ShowPauseButton(bool show) => PauseButton.IsVisible = show;
    public void ShowStats(bool show) => StatsGrid.IsVisible = show;

    public void ShowPopupMenu(bool show = true, string text = "")
    {
        if (show)
        {
            Menu.Title = text;
            Menu.IsVisible = true;
        }
        else
        {
            Menu.IsVisible = false;
        }
    }


    public void CenterMapView(object? sender, EventArgs e)
    {
        //remove hardcoded margins
        double scaleX = MainFrame.Width / (MapView.Width + 20);
        double scaleY = MainFrame.Height / (MapView.Height + 20);

        double scale = Math.Min(scaleX, scaleY);
        MapView.Scale = scale;

        MapView.TranslationX = (MainFrame.Width - MapView.Width) / 2;
        MapView.TranslationY = (MainFrame.Height - MapView.Height) / 2;
    }

    public void RunOnUIThread(Action action)
    {
        if (MainThread.IsMainThread)
        {
            action();
        }
        else
        {
            Dispatcher.Dispatch(action);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Window.Destroying += OnWindowDestroying;
    }

    void ViewSizeChanged(object? sender, EventArgs e)
    {
        PlayerView.WidthRequest = MainFrame.Width;
        PlayerView.HeightRequest = MainFrame.Height;
    }

    private void InitializeMap(object? sender, EventArgs e)
    {
        if (!Game.Instance.IsTutorial)
        {
            MapView.IsVisible = false;
            MapView.SizeChanged -= InitializeMap;
            return;
        }

        CenterMapView(null, EventArgs.Empty);
    }

    private void OnWindowDestroying(object? sender, EventArgs e)
    {
        Game.Instance.CancelUpdate();
    }

    private void NegativeDirectionPressed(object sender, EventArgs e)
    {
        Game.Instance.universe.player.NegativeMove();
        SelectDirection(false);
    }
    private void PositiveDirectionPressed(object sender, EventArgs e)
    {
        Game.Instance.universe.player.PositiveMove();
        SelectDirection(true);
    }
    private void DirectionButtonReleased(object sender, EventArgs e) => Game.Instance.universe.player.StopMovement();

    private void ShootClicked(object sender, EventArgs e) => Game.Instance.universe.player.Attack();
    private void WarpClicked(object sender, EventArgs e) => Game.Instance.universe.player.Warp();

    private void LaunchPage_Clicked(object sender, EventArgs e)
    {
        Game.Instance.Stop();
        App.Current!.MainPage = new StartMenu();
    }

    private void PauseButtonClicked(object sender, EventArgs e)
    {
        Game.Instance.Pause(!Game.Instance.IsPaused);
        ShowPopupMenu(Game.Instance.IsPaused, Game.Instance.IsPaused ? "Pause" : "");
        ShowControls(!Game.Instance.IsPaused);
    }
    private void RestartClicked(object sender, EventArgs e)
    {
        ShowPopupMenu(false);
        Game.Instance.Start(Game.Instance.Seed, Game.Instance.IsTutorial);
    }

    private void SettingsButton_Clicked(object sender, EventArgs e)
    {
        Menu.Modal = new Settings(seedReadOnly: true);
    }
}