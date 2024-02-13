namespace Invasion1DGame.Models
{
	public class Bullet : Kinetic
	{
		private bool isPositiveTouching;
		protected override bool IsNegativeTouching { get; set; }

		private bool isNegativeTouching;
		protected override bool IsPositiveTouching { get; set; }

		public double Damage { get; set; }

		public Bullet(Dimension shape, double position, bool direction) : base(shape, position, Colors.Gray, 2)
		{
			this.direction = direction;

			target = FindTarget(out _, typeof(Bullet), typeof(Player));
			Damage = 1;
		}

		public override void NegativeMove() =>
			Move(ref isNegativeTouching, ref isPositiveTouching, !clockwise);

		public override void PositiveMove() =>
			Move(ref isPositiveTouching, ref isNegativeTouching, clockwise);

		void Move(ref bool isTouching, ref bool isTouchingOpposite, bool direction)
		{
			if (isTouching)
				return; //dispose
			if (isTouchingOpposite)
				isTouchingOpposite = false;

			base.direction = direction;

			target = FindTarget(out double distanceFromTarget, typeof(Bullet), typeof(Player));

			double stepDistance = speed;
			if (distanceFromTarget < stepDistance)
			{
				stepDistance = distanceFromTarget;
				target?.TakeDamage(Damage);
				toDispose = true;//fix logic to take damage and allow bullets to take down two enemies(maybe)
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

			PointF newPosition = Position;
			body.TranslationX = newPosition.X;
			body.TranslationY = newPosition.Y;
		}

		public override void TakeDamage(double damage)
		{
			throw new NotImplementedException();
		}
	}
}