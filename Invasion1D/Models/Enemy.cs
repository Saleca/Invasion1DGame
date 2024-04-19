namespace Invasion1D.Models
{
	public class Enemy : Character
	{
		public Enemy(Dimension shape, double position, double speed) : base(shape, position, Colors.Red, speed)
		{
			direction = clockwise; //randomize
		}

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

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}
	}
}