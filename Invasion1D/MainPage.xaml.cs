﻿using Invasion1D.Controls;
using Invasion1D.Data;
using Invasion1D.Helpers;
using Invasion1D.Models;
using Microsoft.Maui.Controls.Shapes;
using System;

namespace Invasion1D
{
	public partial class MainPage : ContentPage
	{
		readonly bool debug = true;

		static App Game => ((App)Application.Current!);

		//TODO *1
		//Frame? frame;

		bool isMapVisible = false;

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

		public MainPage()
		{
			//SetShootCooldownTimer();

			InitializeComponent();

			if (debug)
			{
				Grid.SetRowSpan(StartKey, 1);
				MapModeKey.IsVisible = true;
			}

			//TOTO *1
			//get frame to focus allowing keyboard control
			/*if (Instance.Handler?.PlatformView is not null)
			{
				frame = (Frame)Instance.Handler.PlatformView;
			}*/
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

		public void UpdateView(Color? color)
		{
			PlayerView.BackgroundColor = color ?? GameColors.VoidColor;
		}

		public void UpdateTime(string time)
		{
			TimeLabel.Text = time;
		}

		public void UpdateHealth(double progress)
		{
			double width = HealthProgressFrame.Width;
			if (width == -1)
			{
				width = 1000;
			}
			HealthProgress.WidthRequest = width * progress;
		}

		public void UpdateVitaLux(double progress)
		{
			double width = VitaluxProgressFrame.Width;
			if (width == -1)
			{
				width = 1000;
			}
			VitaluxProgress.WidthRequest = width * progress;
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
		public void ShootCooldown(double progress)
		{
			ShootCooldownProgress.WidthRequest = ShootCooldownProgressFrame.Width * progress;
		}

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
		public void WarpCooldown(double progress)
		{
			WarpCooldownProgress.WidthRequest = WarpCooldownProgressFrame.Width * progress;
		}

		public void AddToMap(Shape shape)
		{
			MapView.Children.Add(shape);
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

		internal async Task WarpAnimation(Player playerData, Point start, Point end)
		{
			double offset = playerData.Radius - playerData.strokeThickness,
				mapOffsetX = PlayerView.Width / 2 - offset,
				mapOffsetY = PlayerView.Height / 2 - offset,

				startX = mapOffsetX - start.X,
				startY = mapOffsetY - start.Y,
				endX = mapOffsetX - end.X,
				endY = mapOffsetY - end.Y;

			double scale = default!,
				endScaledX = default!,
				endScaledY = default!,
				midX = default!,
				midY = default!;
			if (!isMapVisible)
			{
				scale = 5;

				MapView.Scale = scale;

				double startScaledX = startX * scale,
				startScaledY = startY * scale;

				endScaledX = endX * scale;
				endScaledY = endY * scale;

				midX = GameMath.LinearInterpolation(startX, endX, .5);
				midY = GameMath.LinearInterpolation(startY, endY, .5);

				MapView.TranslationX = startScaledX;
				MapView.TranslationY = startScaledY;

				UpdateView(Colors.Transparent);
				MapView.IsVisible = true;
			}

			IsAnimating = true;

			Task<bool> translatePlayer = playerData.body.TranslateTo(end.X, end.Y, 4000, Easing.CubicInOut);

			if (!isMapVisible)
			{
				Task<bool> translateOut = MapView.TranslateTo(midX, midY, 2000, Easing.CubicOut);
				Task<bool> scaleOut = MapView.ScaleTo(1, 2000, Easing.CubicOut);
				await Task.WhenAll(translateOut, scaleOut);

				Task<bool> translateIn = MapView.TranslateTo(endScaledX, endScaledY, 2000, Easing.CubicIn);
				Task<bool> scaleIn = MapView.ScaleTo(scale, 2000, Easing.CubicIn);
				await Task.WhenAll(translateIn, scaleIn);
			}

			await translatePlayer;

			IsAnimating = false;

			if (!isMapVisible)
			{
				MapView.IsVisible = false;
			}

			//TODO *1
			//frame.Focus();
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

		private void NegPressed(object sender, EventArgs e) => Game.universe.PlayerMove(false);
		private void NegReleased(object sender, EventArgs e) => Game.universe.StopPlayer();

		private void PosPressed(object sender, EventArgs e) => Game.universe.PlayerMove(true);
		private void PosReleased(object sender, EventArgs e) => Game.universe.StopPlayer();

		private void ShootClicked(object sender, EventArgs e) => Game.universe.PlayerAttack();
		private void WarpClicked(object sender, EventArgs e) => Game.universe.WarpPlayer();
		private void StartClicked(object sender, EventArgs e) => Game.Start();
		private void MapModeClicked(object sender, EventArgs e) => ChangeMapMode();
	}
}