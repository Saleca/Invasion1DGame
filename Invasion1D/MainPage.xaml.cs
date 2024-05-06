using Invasion1D.Controls;
using Invasion1D.Data;
using Invasion1D.Helpers;
using Invasion1D.Models;
using Microsoft.Maui.Controls.Shapes;
using System;

namespace Invasion1D
{
	public partial class MainPage : ContentPage
	{
		readonly bool
			debug = true;

		static App Game =>
			((App)Application.Current!);

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
			PlayerViewAccess => PlayerView;
		public AbsoluteLayout
			MapViewAccess => MapView;

		public InvertedProgressBar
			HealthProgressBar = null!,
			VitaluxProgressBar = null!,
			WeaveCooldownProgressBar = null!,
			ShootCooldownProgressBar = null!,
			WarpCooldownProgressBar = null!;

		public MainPage()
		{
			InitializeComponent();

			if (debug)
			{
				Grid.SetRowSpan(StartKey, 1);
				MapModeKey.IsVisible = true;
			}
		}

		public void Initiate()
		{
			HealthProgressBarContainer.Content = HealthProgressBar =
				new InvertedProgressBar(GameColors.GetFromResources(nameof(Health))!, UpdateHealth);

			VitaluxProgressBarContainer.Content = VitaluxProgressBar =
				new InvertedProgressBar(GameColors.GetFromResources(nameof(Vitalux))!, UpdateVitaLux);

			WeaveCooldownProgressBarContainer.Content = WeaveCooldownProgressBar =
				new InvertedProgressBar(GameColors.GetFromResources(nameof(Weave))!, WeaveCooldown, Stats.smoothIncrementIntervalMS, Stats.weaveCoolDownIncrement);
			WeaveCooldownProgressBar.CooldownCompleted += WeaveCooldownCompleted;

			ShootCooldownProgressBarContainer.Content = ShootCooldownProgressBar =
				new InvertedProgressBar(GameColors.GetFromResources(nameof(Vitalux))!, ShootCooldown, Stats.smoothIncrementIntervalMS, Stats.shotCoolDownIncrement);
			ShootCooldownProgressBar.CooldownCompleted += ShootCooldownCompleted;

			WarpCooldownProgressBarContainer.Content = WarpCooldownProgressBar =
				new InvertedProgressBar(GameColors.GetFromResources(nameof(Warpium))!, WarpCooldown, Stats.warpIncrementIntervalMS, Stats.warpCooldownIncrements);
			WarpCooldownProgressBar.CooldownCompleted += WarpCooldownCompleted;
		}

		public void Draw()
		{
			foreach (var dimension in Game.universe.dimensions)
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

		public void UpdateView(Color? color)
		{
			PlayerView.BackgroundColor = color ?? GameColors.VoidColor;
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

		public void ActivateShootCooldown() => ShootCooldownProgressBar.ActivateCooldown();
		public void ShootCooldown(float progress) => ShootCooldownProgressBar.Progress = progress;
		public void ShootCooldownCompleted(object? sender, EventArgs e) => ShowShootKey(true);
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
		public void ClearShootColldown()
		{
			ShowShootKey(true);
			ShootCooldown(0);
		}

		public void ActivateWarpCooldown()
		{
			ShowWarpKey(false);
			WarpCooldownProgressBar.ActivateCooldown();

		}
		public void WarpCooldown(float progress) => WarpCooldownProgressBar.Progress = progress;
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
			WarpCooldown(0);
		}
		public void ClearCoolDownButtons()
		{
			ClearShootColldown();
			ClearWarpColldown();
		}

		//weave needs refactoring, timer needs to be on character, to be used by enemy, progress update should come from player 
		public void ActivateWeaveCooldown() => WeaveCooldownProgressBar.ActivateCooldown();
		public void WeaveCooldown(float progress) => WeaveCooldownProgressBar.Progress = progress;
		public void WeaveCooldownCompleted(object? sender, EventArgs e) => Game.universe.player.SetWeave(false);
		public void ClearWeave()
		{
			WeaveCooldown(0);
		}

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

		public void UpdateStartKeyText(string text)
		{
			StartKey.Text = text;
		}

		public void AddToMap(Shape shape)
		{
			Game.UI.RunOnUIThread(() => MapView.Children.Add(shape));
		}

		public void RemoveFromMap(Shape shape)
		{
			MapView.Children.Remove(shape);
		}

		public void ClearMap()
		{
			MapView.Children.Clear();
		}

		public void ShowText(bool show = true, string text = "")
		{
			if (show)
			{
				MainLabel.Text = text;
				MainLabel.IsVisible = true;
			}
			else
			{
				MainLabel.IsVisible = false;
				MainLabel.Text = "";
			}
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
				Grid.SetColumn(PlayerView, 0);
				Grid.SetColumnSpan(PlayerView, 1);
				Grid.SetColumn(MapView, 1);
				Grid.SetColumnSpan(MapView, 1);
				MapView.Scale = 1;
				MapView.TranslationX = 0;
				MapView.TranslationY = 0;
			}
			else
			{
				MapView.IsVisible = false;
				Grid.SetColumn(PlayerView, 0);
				Grid.SetColumnSpan(PlayerView, 2);
				Grid.SetColumn(MapView, 0);
				Grid.SetColumnSpan(MapView, 2);
			}
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

		private void OnWindowDestroying(object? sender, EventArgs e)
		{
			Game.CancelUpdate();
		}

		private void NegPressed(object sender, EventArgs e) => Game.universe.player.NegativeMove();
		private void NegReleased(object sender, EventArgs e) => Game.universe.player.StopMovement();

		private void PosPressed(object sender, EventArgs e) => Game.universe.player.PositiveMove();
		private void PosReleased(object sender, EventArgs e) => Game.universe.player.StopMovement();

		private void ShootClicked(object sender, EventArgs e) => Game.universe.player.Attack();
		private void WarpClicked(object sender, EventArgs e) => Game.universe.player.Warp();
		private void StartClicked(object sender, EventArgs e) => Game.Start();
		private void MapModeClicked(object sender, EventArgs e) => ChangeMapMode();
	}
}