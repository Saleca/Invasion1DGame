namespace Invasion1DGame.Models
{
	public class Enemy : Character
	{
		public Enemy(Dimension shape, double position, double speed) : base(shape, position, Colors.Red, speed)
		{
			direction = clockwise; //randomize
		}

		protected override bool IsPositiveTouching { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		protected override bool IsNegativeTouching { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public override void Attack()
		{
			throw new NotImplementedException();
		}

		public override void TakeDamage(double damage)
		{
			health -= damage;
			if (health <= 0)
			{
				Dispose();
			}
		}

		public override void NegativeMove()
		{
			throw new NotImplementedException();
		}

		public override void PositiveMove()
		{
			throw new NotImplementedException();
		}

		//Make move method and ignore warpium
	}
}