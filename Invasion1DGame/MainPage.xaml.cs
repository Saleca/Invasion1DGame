using Invasion1DGame.Data;
using Invasion1DGame.Helpers;
using Invasion1DGame.Models;
using Microsoft.Maui.Controls.Shapes;
using System.Diagnostics;

namespace Invasion1DGame
{
	public partial class MainPage : ContentPage
	{
		//Frame? frame;
		public static MainPage Instance = null!;
		//public static MainPage Instance => instance ??= new();

		internal List<Bullet> bullets = [];

		readonly Player playerData = null!;

		bool isMapVisible = true;

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
			if (Instance is null)
			{
				Instance = this;
			}
			else return;


			InitializeComponent();

			//Awake
			/*if (Instance.Handler?.PlatformView is not null)
			{
				frame = (Frame)Instance.Handler.PlatformView;
			}*/

			//(re)Start
			IsAnimating = false;
			bullets = [];
			_ = new Seed();

			//
			//select shape on map to start player on that shape
			//

			double pp = .9f;
			playerData = new((Circular)Dimension.dimensions[0], pp, 10);

			playerData.target = playerData.FindTarget(out double distanceFromTarget, typeof(Player));
			ChangeView(GameColors.CalculateView(distanceFromTarget, playerData.target?.DisplayColor()));

			Draw();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			Task.Run(Update);
		}

		private async Task Update()
		{
			bool isGameOver = false;
			while (!isGameOver)
			{
				try
				{
					await MainThread.InvokeOnMainThreadAsync(UpdateUI);
					await Task.Delay(100);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.ToString());
				}
			}
		}
		void UpdateUI()
		{
			if (bullets.Count > 0)
			{
				UpdateBullets();
			}

			if (!IsAnimating)
			{
				playerData.target = playerData.FindTarget(out double distanceFromTarget, typeof(Player));
				if (playerData.target != null)
				{
					ChangeView(GameColors.CalculateView(distanceFromTarget, playerData.target.DisplayColor()));
				}
				else
				{
					ChangeView();
				}
			}
		}

		internal async Task AnimateTeleportation(Point start, Point end)
		{
			double scale = 5;

			MapView.Scale = scale;

			double offset = playerData.Radius - playerData.strokeThickness,
				mapOffsetX = MapView.Width / 2 - offset,
				mapOffsetY = MapView.Height / 2 - offset,

				startX = mapOffsetX - start.X,
				startY = mapOffsetY - start.Y,
				startScaledX = startX * scale,
				startScaledY = startY * scale,

				endX = mapOffsetX - end.X,
				endY = mapOffsetY - end.Y,
				endScaledX = endX * scale,
				endScaledY = endY * scale,

				midX = GameMath.LinearInterpolation(startX, endX, .5),
				midY = GameMath.LinearInterpolation(startY, endY, .5);

			MapView.TranslationX = startScaledX;
			MapView.TranslationY = startScaledY;

			IsAnimating = true;
			ChangeView(Colors.Transparent);
			MapView.IsVisible = true;

			var translatePlayer = playerData.body.TranslateTo(end.X, end.Y, 4000, Easing.CubicInOut);

			var translateOut = MapView.TranslateTo(midX, midY, 2000, Easing.CubicOut);
			var scaleOut = MapView.ScaleTo(1, 2000, Easing.CubicOut);
			await Task.WhenAll(translateOut, scaleOut);

			var translateIn = MapView.TranslateTo(endScaledX, endScaledY, 2000, Easing.CubicIn);
			var scaleIn = MapView.ScaleTo(scale, 2000, Easing.CubicIn);
			await Task.WhenAll(translateIn, scaleIn);

			await translatePlayer;

			IsAnimating = false;
			MapView.IsVisible = false;

			//frame.Focus();
		}

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

		void Draw()
		{
			foreach (var dimension in Dimension.dimensions)
			{
				MapView.Add(dimension.body);

				foreach (var interactiveObj in dimension.interactiveObjects)
				{
					MapView.Add(interactiveObj.body);
				}
			}
		}

		internal void ChangeView(Color? color = null) => PlayerView.BackgroundColor = color ?? VoidColor;

		public void ClearView()
		{
			MapView.Children.Clear();
			//default stats views
		}

		internal void RemoveFromView(Shape shape)
		{
			MapView.Children.Remove(shape);
		}

		internal void AddBullets(Bullet bullet)
		{
			bullets.Add(bullet);

			MapView.Children.Add(bullet.body);
		}

		private void NegClicked(object sender, EventArgs e) => PlayerMove(false);
		private void PosClicked(object sender, EventArgs e) => PlayerMove(true);
		private void ShootClicked(object sender, EventArgs e) => PlayerAttack();
		private void MapModeClicked(object sender, EventArgs e) => ChangeMapMode();

		public void PlayerMove(bool dir)
		{
			if (dir)
				playerData.PositiveMove();
			else playerData.NegativeMove();
		}
		public void PlayerAttack() => playerData.Attack();
		public void ChangeMapMode()
		{
			isMapVisible = !isMapVisible;
			if (isMapVisible)
			{
				MapView.IsVisible = true;
			}
			else
			{
				MapView.IsVisible = false;
			}
		}

		public static Color VoidColor => Microsoft.Maui.Controls.Application.Current?.RequestedTheme switch
		{
			AppTheme.Dark => Colors.Black,
			AppTheme.Light => Colors.White,
			_ => Colors.Black,
		};
	}
}