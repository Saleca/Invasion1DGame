namespace Invasion1DGame.Models
{
	public class Bullet : Kinetic
	{
		protected override bool IsNegativeTouching { get; set; }
		protected override bool IsPositiveTouching { get; set; }

		public double Damage { get; set; }

		public Bullet(Dimension shape, double position, bool direction) : base(shape, position, Colors.Gray, 5)
		{
			this.direction = direction;

			Damage = 1;
		}

		public override void NegativeMove() =>
			Move(!clockwise);

		public override void PositiveMove() =>
			Move(clockwise);

		void Move(bool direction)
		{
			base.direction = direction;

			Kinetic? target = FindInteractive(out double distanceFromTarget, typeof(Bullet), typeof(Vitalux), typeof(Warpium)) as Kinetic;

			double stepDistance = speed;
			if (distanceFromTarget < stepDistance)
			{
				stepDistance = distanceFromTarget;
				target?.TakeDamage(Damage);
				toDispose = true;
			}

			if (direction)
			{
				PercentageInShape += CurrentDimention.GetPercentageFromDistance(stepDistance);
			}
			else
			{
				PercentageInShape -= CurrentDimention.GetPercentageFromDistance(stepDistance);
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