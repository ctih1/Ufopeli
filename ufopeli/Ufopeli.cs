using Jypeli;
using Jypeli.Assets;
using System;
using System.Collections.Generic;

namespace ufopeli
{
	/// @author onni.nevala
	/// @version 24.11.2023
	/// <summary>
	/// 
	/// </summary>
	///

	// TODO: Healthbar
	public class ufopeli : PhysicsGame
	{
		public int score = 0;
		Label scoreText;
		Label healthText;
		private Jypeli.Shape healthBar;
		int Width;
		int Heigth;
		int margin = 25;
		int fontSize = 52;
		Dictionary<String, int> difficulties = new Dictionary<string, int>();
		PhysicsObject _; // väliaikainen
		double scaling = 1.5;
		PhysicsObject ship;

		PlasmaCannon plasmaCannon;
		private int health = 100;
		private EnemyManager enemyManager = new EnemyManager();
		PhysicsObject bullet;
		String difficulty = "medium";

		public override void Begin()
		{
			difficulties.Add("easy", 50);
			difficulties.Add("medium", 150);
			difficulties.Add("hard", 300);

			scoreText = new Label("test");
			Add(scoreText);

			healthText = new Label("" + health);
			healthText.Position = new Vector(Convert.ToInt32((Screen.Size.X - Screen.Size.X * 1.5) * -1) - margin - fontSize, Convert.ToInt32((Screen.Size.Y - Screen.Size.Y * 1.5)) + margin + fontSize);
			healthText.Color = Color.White;
			healthText.TextColor = Color.Red;

			
			Add(healthText);

			Timer.CreateAndStart(1.5, AddEnemy);
			Add(CreatePlayer(50,50));

			Movement movement = new Movement();
			movement.init(ship);

			AddCollisionHandler(ship, PlayerCollision);

			plasmaCannon = new PlasmaCannon(10,10);
			plasmaCannon.Angle = plasmaCannon.Angle + Angle.FromDegrees(90);
			plasmaCannon.FireRate = 3;

			ship.Add(plasmaCannon);

			PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
			Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

			Keyboard.Listen(Key.S, ButtonState.Down, movement.Down, "Down");
			Keyboard.Listen(Key.W, ButtonState.Down, movement.Up, "Up");
			Keyboard.Listen(Key.A, ButtonState.Down, movement.Left, "Left");
			Keyboard.Listen(Key.D, ButtonState.Down, movement.Right, "Right");
			Keyboard.Listen(Key.F, ButtonState.Down, Shoot, "Shoot");
		}

		public PhysicsObject CreatePlayer(int w, int h)
		{
			ship = new PhysicsObject(w, h);
			ship.Shape = Shape.Triangle;
			ship.Color = Color.Green;
			ship.LinearDamping = 1.5;
			ship.AngularDamping = 1.5;
			return ship;
		}

		public static void EnemyCloseEvent()
		{
			
		}

		public void PlayerCollision(PhysicsObject collider, PhysicsObject target)
		{
			health-=10;
			if(health<=0)
			{
				collider.Destroy();
			}
			healthText.Text = ""+health;
		}
		
		public void BulletCollision(IPhysicsObject collider, IPhysicsObject target)
		{
			score++;
			scoreText.Text = "" + score;
			target.Destroy();
		}
		public void Shoot()
		{
			bullet = plasmaCannon.Shoot();
			if (bullet != null)
			{
				bullet.Collided += BulletCollision;
			}
		}

		public static int GetDifficulty()
		{
			return 150;
		}

		void AddEnemy()
		{
			Add(enemyManager.CreateEnemy(ship));
		}

	}
}