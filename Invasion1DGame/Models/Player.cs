using Invasion1DGame.Helpers;

namespace Invasion1DGame.Models
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
			IsPositiveTouching = false;
			IsNegativeTouching = false;

			dimension.EnemiesEliminated += TeleportAsync;
		}

		public async void TeleportAsync(object? sender, EventArgs args)
		{
			visitedDimensions.Add(CurrentDimention);
			if (visitedDimensions.Count == Dimension.dimensions.Count)
			{
				visitedDimensions.Clear();
			}

			var unvisitedDimentions = Dimension.dimensions.Except(visitedDimensions).ToArray();
			Random random = new();

			CurrentDimention.EnemiesEliminated -= TeleportAsync; //unsubscribe from old dimension
			CurrentDimention = unvisitedDimentions[random.Next(unvisitedDimentions.Length)];
			CurrentDimention.EnemiesEliminated += TeleportAsync; //subscribe to new dimension

			PositionPercentage = random.NextDouble();

			await MainPage.Instance.AnimateTeleportation(
				start: new(body.TranslationX, body.TranslationY),
				end: new(Position.X, Position.Y));
		}

		public override void Attack()
		{
			//Add cooldown timer
			if (vitalux > attackCost)
			{
				vitalux -= attackCost;

				MainPage.Instance.AddBullets(
					new Bullet(
						shape: CurrentDimention,
						position: direction ?
							GameMath.AddPercentage(PositionPercentage, percentageOffset) :
							GameMath.SubtractPercentage(PositionPercentage, percentageOffset),
						direction: direction));
			}
		}

		public override void NegativeMove() =>
			Move(ref isNegativeTouching, ref isPositiveTouching, !clockwise);

		public override void PositiveMove() =>
			Move(ref isPositiveTouching, ref isNegativeTouching, clockwise);

		void Move(ref bool isTouching, ref bool isTouchingOpposite, bool direction)
		{
			if (isTouching)
				return;
			if (isTouchingOpposite)
				isTouchingOpposite = false;

			this.direction = direction;

			target = FindTarget(out double distanceFromTarget, typeof(Player));

			double stepDistance = speed;
			if (distanceFromTarget < stepDistance)
			{
				stepDistance = distanceFromTarget;
				isTouching = true;
			}

			if (direction)
			{
				PositionPercentage += CurrentDimention.GetPercentageFromDistance(stepDistance);
			}
			else
			{
				PositionPercentage -= CurrentDimention.GetPercentageFromDistance(stepDistance);
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