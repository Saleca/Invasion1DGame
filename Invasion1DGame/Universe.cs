using Invasion1DGame.Data;
using Invasion1DGame.Helpers;
using Invasion1DGame.Models;

namespace Invasion1DGame
{
	public class Universe
	{
		public List<Dimension> dimensions = [];
		public List<Bullet> bullets = [];

		public Player playerData;

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

		public Universe()
		{
			IsAnimating = false;

			_ = new Seed(this);

			double pp = .9f;
			playerData = new((Circular)dimensions[0], pp, 10);

			MainPage.Instance.ChangeView(playerData.GetView());
		}

		public void ReStart()
		{
			MainPage.Instance.ClearView();
			//clear all dimentions data

			bullets.Clear();
			IsAnimating = false;
			_ = new Seed(this);
		}
	}
}