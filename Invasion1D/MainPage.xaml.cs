using Invasion1D.Data;
using Invasion1D.Helpers;
using Invasion1D.Models;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Shapes;
using System.Diagnostics;

namespace Invasion1D
{
	public partial class MainPage : ContentPage
	{
		readonly bool debug = false;

		public static MainPage Instance = null!;
		//public static MainPage Instance => instance ??= new();

		//TODO *1
		//Frame? frame;

		bool isStarted = false;

		public Universe universe = null!;
		bool isMapVisible = false;
		public List<Bullet> bullets = [];

		readonly object locker = new();
		bool isAnimating;
		bool IsAnimating
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
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				return;
			}

			InitializeComponent();

			if (debug)
			{
				Grid.SetRowSpan(StartKey, 2);
				MapModeKey.IsVisible = true;
			}

			//TOTO *1
			//get frame to focus allowing keyboard control
			/*if (Instance.Handler?.PlatformView is not null)
			{
				frame = (Frame)Instance.Handler.PlatformView;
			}*/

		}

		public void Initiate()
		{
			IsAnimating = false;
			bullets = [];
			universe = new();
			universe.Initiate();
			isStarted = true;
		}

		public void Start()
		{
			if (isStarted)
			{
				universe.CancelUpdate();
				Reset();
			}
			Initiate();

			universe.Start();

			ControlsGrid.IsVisible = true;
			StartKey.Text = "Restart";
		}

		public void Reset()
		{
			universe.ResetDimentions();
			MapView.Children.Clear();
		}

		public void Draw()
		{
			foreach (var dimension in universe.dimensions)
			{
				MapView.Add(dimension.body);

				foreach (var interactiveObj in dimension.interactiveObjects)
				{
					MapView.Add(interactiveObj.body);
				}
			}
		}

		public void UpdateUI(Player playerData, string time)
		{
			if (bullets.Count > 0)
			{
				UpdateBullets();
			}

			//TODO
			//automate and update enemies

			if (!IsAnimating)
			{
				UpdateView(playerData.GetView());
			}

			//TODO
			//make bar for health and vitalux
			HealthLabel.Text = playerData.health.ToString();
			VitaluxLabel.Text = playerData.vitalux.ToString();

			WarpiumLabel.Text = playerData.warpium.ToString();

			TimeLabel.Text = time;
		}

		//TODO 
		//move this to bullet class
		void UpdateBullets()
		{
			List<Bullet> bulletsToRemove = [];
			foreach (Bullet b in bullets)
			{
				if (b.direction)
				{
					b.PositiveMove();
				}
				else
				{
					b.NegativeMove();
				}
				if (b.toDispose)
				{
					bulletsToRemove.Add(b);
				}
			}
			foreach (Bullet b in bulletsToRemove)
			{
				bullets.Remove(b);
				b.Dispose();
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

		internal void UpdateView(Color? color = null) => PlayerView.BackgroundColor = color ?? VoidColor;

		internal void RemoveFromView(Shape shape)
		{
			MapView.Children.Remove(shape);
		}

		internal void AddBullets(Bullet bullet)
		{
			bullets.Add(bullet);

			MapView.Children.Add(bullet.body);
		}

		public void PositiveMove(bool pressed)
		{
			if (pressed)
			{
				universe.PlayerMove(true);
			}
		}

		public void NegativeMove(bool pressed)
		{
			if (pressed)
			{
				universe.PlayerMove(false);
			}
		}

		public static Color VoidColor => Application.Current?.RequestedTheme switch
		{
			AppTheme.Dark => Colors.Black,
			AppTheme.Light => Colors.White,
			_ => Colors.Black,
		};

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

		protected override void OnAppearing()
		{
			base.OnAppearing();
			Window.Destroying += OnWindowDestroying;
		}

		private void OnWindowDestroying(object? sender, EventArgs e)
		{
			universe.CancelUpdate();
		}

		private void NegPressed(object sender, EventArgs e) => NegativeMove(true);
		private void NegReleased(object sender, EventArgs e) => NegativeMove(false);

		private void PosPressed(object sender, EventArgs e) => PositiveMove(true);
		private void PosReleased(object sender, EventArgs e) => PositiveMove(false);

		private void ShootClicked(object sender, EventArgs e) => universe.PlayerAttack();
		private void WarpClicked(object sender, EventArgs e) => WarpKey.Text = "test";//universe.WarpPlayer();
		private void StartClicked(object sender, EventArgs e) => Start();
		private void MapModeClicked(object sender, EventArgs e) => ChangeMapMode();
	}
}