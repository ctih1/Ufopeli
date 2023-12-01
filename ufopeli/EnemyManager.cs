using Jypeli;
using System;


namespace ufopeli
{
	public class EnemyManager
	{
		private PhysicsObject enemy;
		private Random random = new Random();
		Logic logic = new Logic();

		public PhysicsObject CreateEnemy(PhysicsObject ship)
		{
			enemy = new PhysicsObject(30, 30);
			enemy.Shape = Shape.Circle;
			enemy.Color = Color.Red;
			enemy.X = random.Next(0, (int) 800);
			enemy.Y = random.Next(0, (int) 600); // Set as the actual screen siez later.
			enemy.Brain = logic.AddSmartLogic(ship);
			return enemy;
		}
	}
}
