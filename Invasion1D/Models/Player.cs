using Invasion1D.Helpers;

namespace Invasion1D.Models
{
	public class Player : Character
	{
		private bool isPositiveTouching;
		protected override bool IsPositiveTouching { get => isPositiveTouching; set => isPositiveTouching = value; }

		private bool isNegativeTouching;
		protected override bool IsNegativeTouching { get => isNegativeTouching; set => isNegativeTouching = value; }

		public List<Dimension> visitedDimensions = [];

		public Player(Dimension dimension, double position, double speed) : base(dimension, position, Colors.Green, speed)
		{
			direction = clockwise;
		}

		public async void WarpAsync()
		{
			if (warpium > 0)
			{
				visitedDimensions.Add(CurrentDimention);

				if (visitedDimensions.Count == MainPage.Instance.universe.dimensions.Count)
					visitedDimensions.Clear();

				var unvisitedDimentions = MainPage.Instance.universe.dimensions.Except(visitedDimensions).ToArray();

				Dimension cd = CurrentDimention;
				CurrentDimention = unvisitedDimentions[MainPage.Instance.universe.random.Next(unvisitedDimentions.Length)];
				cd.interactiveObjects.Remove(this);

				do
				{
					PercentageInShape = MainPage.Instance.universe.random.NextDouble();
				} while (CurrentDimention.CheckOverlap(this));

				warpium--;

				await MainPage.Instance.WarpAnimation(this,
					start: new(body.TranslationX, body.TranslationY),
					end: new(Position.X, Position.Y));
			}
		}

		public override void Attack()
		{
			//TODO
			//Add cooldown timer
			if (vitalux >= attackCost)
			{
				vitalux -= attackCost;

				MainPage.Instance.AddBullets(
					new Bullet(
						shape: CurrentDimention,
						position: direction ?
								GameMath.AddPercentage(PercentageInShape, sizePercentage) :
								GameMath.SubtractPercentage(PercentageInShape, sizePercentage),
						direction: direction));
			}
		}

		public override void NegativeMove() =>
			Move(!clockwise);

		public override void PositiveMove() =>
			Move(clockwise);

		void Move(bool direction)
		{
			this.direction = direction;

			double stepDistance = default!;
			bool completeStep = false;

			List<Type> ignore = [typeof(Player)];
			while (!completeStep)
			{
				Interactive? target = FindInteractive(out double distanceFromTarget, [.. ignore]);

				stepDistance = speed;
				if (distanceFromTarget < stepDistance)
				{
					if (target is Item item)
					{
						if (item.Power(this))
						{
							continue;
						}
						else
						{
							switch (item)
							{
								case Vitalux:
									ignore.Add(typeof(Vitalux));
									break;
								case Warpium:
									ignore.Add(typeof(Warpium));
									break;
							}
							continue;
						}
					}
					stepDistance = distanceFromTarget;
				}
				completeStep = true;
			}

			if (direction)
			{
				PercentageInShape += CurrentDimention.GetPercentageFromDistance(stepDistance);
			}
			else
			{
				PercentageInShape -= CurrentDimention.GetPercentageFromDistance(stepDistance);
			}

			body.TranslationX = Position.X;
			body.TranslationY = Position.Y;
		}

		public override void TakeDamage(double damage)
		{
			health -= damage;
			if (health <= 0)
			{
				Dispose();
			}
		}
	}
}