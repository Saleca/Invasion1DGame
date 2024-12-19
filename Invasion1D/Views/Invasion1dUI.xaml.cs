﻿using Invasion1D.Controls;
using Invasion1D.Data;
using Invasion1D.Helpers;
using Invasion1D.Logic;
using Microsoft.Maui.Controls.Shapes;
using System.Diagnostics;

namespace Invasion1D.Views;

public partial class Invasion1dUI : ContentPage
{
    readonly bool
        debug = true;

    public bool
        isMapVisible = false;

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

    public InvertedProgressBar
        HealthProgressBar = null!,
        VitaluxProgressBar = null!,
        WeaveProgressBar = null!;
    public InvertedCooldownProgressBar
        ShootCooldownProgressBar = null!,
        WarpCooldownProgressBar = null!;

    public Invasion1dUI()
    {
        InitializeComponent();
        
        MainFrame.SizeChanged += ViewSizeChanged;
        MapView.SizeChanged += InitializeMap;

        if (debug)
        {
            MapModeKey.IsVisible = true;
        }
    }

    private void InitializeMap(object? sender, EventArgs e)
    {
        MapView.IsVisible = false;
        CenterMapView(null, EventArgs.Empty);
    }

    public void Initiate()
    {
        HealthProgressBarContainer.Content = HealthProgressBar =
            new InvertedProgressBar(GameColors.Health);

        VitaluxProgressBarContainer.Content = VitaluxProgressBar =
            new InvertedProgressBar(GameColors.Vitalux);

        WeaveCooldownProgressBarContainer.Content = WeaveProgressBar =
            new InvertedProgressBar(GameColors.Weave);

        ShootCooldownProgressBarContainer.Content = ShootCooldownProgressBar =
            new InvertedCooldownProgressBar(GameColors.Vitalux, Stats.smoothIncrementIntervalF, Stats.shotCoolDownIncrement);
        ShootCooldownProgressBar.CooldownCompleted += ShootCooldownCompleted;

        WarpCooldownProgressBarContainer.Content = WarpCooldownProgressBar =
            new InvertedCooldownProgressBar(GameColors.Warpium, Stats.warpIncrementIntervalMS, Stats.warpCooldownIncrements);
        WarpCooldownProgressBar.CooldownCompleted += WarpCooldownCompleted;
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
        if (!isMapVisible)
        {
            MapView.IsVisible = false;
        }
        IsAnimating = false;
    }

    public void UpdateView(Color? forwardView, Color? rearView)
    {
        forwardView ??= GameColors.VoidColor;
        rearView ??= GameColors.VoidColor;

        PlayerView.Background = new RadialGradientBrush(
            gradientStops:
            [
                new GradientStop { Color = forwardView, Offset = .8f },
                new GradientStop { Color = rearView, Offset = .9f }
            ],
            center: new(0.5, 0.5),
            radius: .75)
        { };
    }

    public void UpdateTime(string time)
    {
        TimeLabel.Text = time;
    }
    public void UpdateEnemies(string enemies)
    {
        EnemiesLabel.Text = enemies;
    }

    public void UpdateHealth(float progress) => HealthProgressBar.Progress = progress;
    public void UpdateVitaLux(float progress) => VitaluxProgressBar.Progress = progress;
    public void UpdateWeave(float progress) => WeaveProgressBar.Progress = progress;

    public void ActivateShootCooldown()
    {
        ShowShootKey(false);
        ShootCooldownProgressBar.ActivateCooldown();
    }
    public void ShootCooldownCompleted(object? sender, EventArgs e) => ShowShootKey(true);
    public void ClearShootColldown()
    {
        ShowShootKey(true);
        ShootCooldownProgressBar.Progress = 0;
    }
    public void ShowShootKey(bool show)
    {
        if (show)
        {
            ShootKey.IsVisible = true;
        }
        else
        {
            ShootKey.IsVisible = false;
        }
    }

    public void ActivateWarpCooldown()
    {
        ShowWarpKey(false);
        WarpCooldownProgressBar.ActivateCooldown();

    }
    public void WarpCooldownCompleted(object? sender, EventArgs e) => RunOnUIThread(() => ShowWarpKey(true));
    public void ShowWarpKey(bool show)
    {
        if (show)
        {
            WarpKey.IsVisible = true;
        }
        else
        {
            WarpKey.IsVisible = false;
        }
    }
    public void ClearWarpColldown()
    {
        ShowWarpKey(true);
        WarpCooldownProgressBar.Progress = 0;
    }
    public void ClearCoolDownButtons()
    {
        ClearShootColldown();
        ClearWarpColldown();
    }

    public void ClearWeave() =>
        WeaveProgressBar.Progress = 0;

    public void AddWarpium()
    {
        WarpiumContainer.Add(new WarpiumControl());
    }
    public void RemoveWarpium()
    {
        WarpiumContainer.RemoveAt(0);
    }
    public void ClearWarpium()
    {
        WarpiumContainer.Clear();
    }

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

    public void ShowPopUpMenu(bool show = true, string text = "")
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

    public void ShowContinueButton(bool show)
    {
        ContinueButton.IsVisible = show;
    }

    public void ShowControls(bool show)
    {
        if (show)
        {
            ControlsGrid.IsVisible = true;
        }
        else
        {
            ControlsGrid.IsVisible = false;
        }
    }
    public void ShowPauseButton(bool show)
    {
        PauseButton.IsVisible = show;
    }

    public void ShowStats(bool show)
    {
        if (show)
        {
            StatsGrid.IsVisible = true;
        }
        else
        {
            StatsGrid.IsVisible = false;
        }
    }

    public void ChangeMapMode()
    {
        isMapVisible = !isMapVisible;
        if (isMapVisible)
        {
            MapView.IsVisible = true;
            CenterMapView(null, EventArgs.Empty);
        }
        else
        {
            MapView.IsVisible = false;
        }
    }

    public void CenterMapView(object? sender, EventArgs e)
    {
        if (!isMapVisible)
        {
            return;
        }

        double scaleX = MainFrame.Width / MapView.Width;
        double scaleY = MainFrame.Height / MapView.Height;

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
        double size = Math.Max(MainFrame.Width, MainFrame.Height);
        PlayerView.WidthRequest = size;
        PlayerView.HeightRequest = size;

        CenterMapView(null, EventArgs.Empty);
    }

    private void OnWindowDestroying(object? sender, EventArgs e)
    {
        Game.Instance.CancelUpdate();
    }

    private void NegPressed(object sender, EventArgs e) => Game.Instance.universe.player.NegativeMove();
    private void NegReleased(object sender, EventArgs e) => Game.Instance.universe.player.StopMovement();

    private void PosPressed(object sender, EventArgs e) => Game.Instance.universe.player.PositiveMove();
    private void PosReleased(object sender, EventArgs e) => Game.Instance.universe.player.StopMovement();

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
        ShowPopUpMenu(Game.Instance.IsPaused, Game.Instance.IsPaused ? "Pause Menu" : "");
        ShowControls(!Game.Instance.IsPaused);
    }
    private void MapModeClicked(object sender, EventArgs e) => ChangeMapMode();

    private void RestartClicked(object sender, EventArgs e)
    {
        ShowPopUpMenu(false);
        Game.Instance.Start();
    }
}