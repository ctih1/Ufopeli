using Jypeli;
using Jypeli.Assets;
using Jypeli.Effects;
using Jypeli.Widgets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

// TODO: Käännä "enemy.png" että se ei ole sivuttain.

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
		// Display
		readonly int WIDTH;
		readonly int HEIGTH;
		double scaling = 1.5;

		// Font
		int margin = 25;
		int fontSize = 52;

		// Score
		int score = 0;
		Label scoreText;

		// Health
		int health = 100;
		Label healthText;
		Shape healthBar; // WIP or scrapped

		// Images
		Image line = LoadImage("line"); // Tee oikeasti muoto myöhemmin
		Image shipImage = LoadImage("ship"); // Miksei esimerkeissä käytetä tiedostopäätettä?? https://github.com/Jypeli-JYU/Jypeli/blob/master/Examples/Koripallo/Koripallo.cs
		Image enemyImage = LoadImage("enemy");

		// Objects
		PhysicsObject ship;
		PlasmaCannon plasmaCannon;
		PhysicsObject enemy;
		PhysicsObject bullet;
		List<PhysicsObject> enemies = new List<PhysicsObject>();
		private int size;

		// Logic 
		RandomMoverBrain logic;
		FollowerBrain approachLogic;

		// Camera
		private int x;
		private int y;

		// Difficulty
		Dictionary<String, int> difficulties = new Dictionary<string, int>();
		String difficulty = "medium";

		// Misc
		PhysicsObject _; // väliaikainen
		Vector direction;
		Random random = new Random();
		Particle fire = new Particle();

		// Game variables
		private readonly int BASE_SPEED = 1250;
		private int speed;
		private readonly int BASE_ENEMY_SPEED = 100;
		private int enemySpeed;
		private readonly int BASE_BULLET_SPEED = 900;
		private int bulletSpeed;
		private readonly double BASE_ROTATION_SPEED = 0.010;
		private double rotationSpeed;
		
		public override void Begin()
		{

			// Restarting 

			foreach(var enemy in enemies)
			{
				enemy.Destroy();
			}

			score = 0;
			enemies.Clear();
			health = 100;

			// Game var 
			speed = BASE_SPEED;
			enemySpeed = BASE_ENEMY_SPEED;
			bulletSpeed = BASE_BULLET_SPEED;
			rotationSpeed = BASE_ROTATION_SPEED;

			// Background
			Level.Background.Color = new Color(18,18,18,255);

			// Score
			scoreText = new Label("0");
			scoreText.TextColor = Color.White;
			scoreText.Layer = Layer.CreateStaticLayer();
			Add(scoreText);

			// Health
			healthText = new Label("" + health);
			healthText.Position = new Vector(Convert.ToInt32((Screen.Size.X - Screen.Size.X * 1.5) * -1) - margin - fontSize, Convert.ToInt32((Screen.Size.Y - Screen.Size.Y * 1.5)) + margin + fontSize);
			healthText.Color = Color.White;
			healthText.TextColor = Color.Red;
			healthText.Font = new Font(fontSize);
			Add(healthText);

			// Difficult
			difficulties.Clear();
			difficulties.Add("easy", 50);
			difficulties.Add("medium", 150);
			difficulties.Add("hard", 300);

			// Timer & Creating a player
			ship = CreatePlayer(50, 80);
			Add(ship);
			Jypeli.Timer.CreateAndStart(3, AddEnemy);
			Jypeli.Timer.CreateAndStart(15, IncreseDifficulty);

			// Collision
			AddCollisionHandler(ship, PlayerCollision);

			// Cannon
			plasmaCannon = new PlasmaCannon(10,10);
			plasmaCannon.Color = Color.Transparent;
			plasmaCannon.AmmoIgnoresGravity = false;
			plasmaCannon.Angle = plasmaCannon.Angle + Angle.FromDegrees(90);
			plasmaCannon.FireRate = 10;
			ship.Add(plasmaCannon);
			
			// Keybinds
			PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
			Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
			Keyboard.Listen(Key.S, ButtonState.Down, Down, "Down");
			Keyboard.Listen(Key.W, ButtonState.Down, Up, "Up");
			Keyboard.Listen(Key.A, ButtonState.Down, Left, "Left");
			Keyboard.Listen(Key.D, ButtonState.Down, Right, "Right");
			Keyboard.Listen(Key.F, ButtonState.Down, Shoot, "S	hoot");
			Keyboard.Listen(Key.E, ButtonState.Down, Shoot, "Shoot");
			Keyboard.Listen(Key.Space, ButtonState.Down, Boost, "Boost");
			Keyboard.Listen(Key.Space, ButtonState.Released, ResetBoost, "ResetBoost");
			Keyboard.Listen(Key.LeftShift, ButtonState.Down, Slowdown, "Slowdown");
			Keyboard.Listen(Key.LeftShift, ButtonState.Released, ResetSlowdown, "ResetSlowdown");
		}

		public PhysicsObject CreatePlayer(int w, int h)
		{
			ship = new PhysicsObject(w, h);
			ship.Shape = Shape.Triangle;
			ship.Image = shipImage;
			ship.LinearDamping = 1.5;
			ship.AngularDamping = 1.5;
			ship.CollisionIgnoreGroup = 2;
			//ship.Add(PlayerLine());
			//ship.IgnoresCollisionWith(shipAimLine);
			ship.Mass = 2;
			ship.Tag = "ship";
			return ship;	
		}


		public PhysicsObject CreateEnemy(PhysicsObject ship)
		{
			size = random.Next(25, 60);
			enemy = new PhysicsObject(size, size);
			enemy.Shape = Shape.Triangle;
			enemy.Image = enemyImage;
			enemy.X = random.Next(0, (int)800); // TODO: set as the screen width
			enemy.Y = random.Next(0, (int)600); // TODO: set as the screen height
			enemy.Brain = AddSmartLogic(ship);
			enemy.CollisionIgnoreGroup = 1;
			enemy.Tag = "Enemy";
			enemy.RelativeAngle = Angle.FromDegrees(180);
			enemy.CanRotate = true;
			return enemy;
		}

		private RandomMoverBrain AddDumbLogic()
		{
			logic = new RandomMoverBrain(200);
			logic.ChangeMovementSeconds = 3;
			return logic;
		}

		public FollowerBrain AddSmartLogic(PhysicsObject target)
		{
			approachLogic = new FollowerBrain(target);
			approachLogic.Speed = enemySpeed;
			approachLogic.DistanceFar = 1200;
			approachLogic.DistanceClose = 200;
			approachLogic.FarBrain = AddDumbLogic();
			approachLogic.TargetClose += EnemyCloseEvent;
			approachLogic.TurnWhileMoving = true;
			return approachLogic;
		}

		public void EnemyCloseEvent()
		{
			
		}

		void PlayerCollision(PhysicsObject collider, PhysicsObject target)
		{
			health -= 10;
			if (health <= 0)
			{
				ship.Destroy();
				Restart();
			}
			healthText.Text = "" + health;
		}
		void BulletCollision(IPhysicsObject collider, IPhysicsObject target)
		{
			if(!target.IgnoresCollisionResponse)
			{
				score++;
				scoreText.Text = "" + score;
				target.Destroy();
			}
		}
		void Shoot()
		{
			bullet = plasmaCannon.Shoot();
			if (bullet != null)
			{
				bullet.MaximumLifetime = new TimeSpan(0,0,30);
				bullet.MaxVelocity = bulletSpeed;
				bullet.Color = Color.Gold;
				bullet.Shape = Shape.Triangle;
				bullet.Tag = "Bullet";
				bullet.Collided += BulletCollision;
			}
		}

		void Boost()
		{
			MessageDisplay.Add("Boosting");
			speed = BASE_SPEED*3;
		}
		void ResetBoost()
		{
			MessageDisplay.Add("!Boosting");

			speed = BASE_SPEED;
		}

		void Slowdown()
		{
			MessageDisplay.Add("Slowdown");

			speed = BASE_SPEED / 3;
			enemySpeed = BASE_ENEMY_SPEED / 3;
			bulletSpeed = BASE_BULLET_SPEED / 3;
			rotationSpeed = BASE_ROTATION_SPEED / 3;
			if(approachLogic!=null)
			{
				approachLogic.Speed = enemySpeed;
				foreach (var enemy in enemies)
				{
					enemy.Brain = AddSmartLogic(ship);
				}
			}
		}

		void ResetSlowdown()
		{
			MessageDisplay.Add("!Slowdown");

			speed = BASE_SPEED;
			enemySpeed = BASE_ENEMY_SPEED;
			bulletSpeed = BASE_BULLET_SPEED;
			rotationSpeed = BASE_ROTATION_SPEED;
			if(approachLogic!=null)
			{
				approachLogic.Speed = enemySpeed;
				foreach (var enemy in enemies)
				{
					enemy.Brain = AddSmartLogic(ship);
				}
			}
		}

		public void IncreseDifficulty()
		{
			MessageDisplay.Add("Increasing Difficulty.");
			enemySpeed += 25;
		}

		void AddEnemy()
		{
			_ = CreateEnemy(ship); // ???
			enemies.Append(_);
			Add(_);
		}

		void Down()
		{
			direction = Vector.FromLengthAndAngle(speed * -1, ship.Angle + Angle.FromDegrees(90));
			ship.Push(direction);
		}
		void Up()
		{
			direction = Vector.FromLengthAndAngle(speed, ship.Angle + Angle.FromDegrees(90));
			ship.Push(direction);
		}
		void Left()
		{
			ship.ApplyTorque(rotationSpeed);
		}
		void Right()
		{
			ship.ApplyTorque(rotationSpeed*-1);
		}

		public void Restart()
		{
			ResetLayers();
			ClearTimers();
			ClearLights();
			ClearControls();
			GC.Collect();
			Camera.Reset();
			ControlContext.Enable();
			IsPaused = false;
			Begin();
		}
	}
}