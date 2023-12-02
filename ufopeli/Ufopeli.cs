using FarseerPhysics.Common;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Effects;
using Jypeli.Widgets;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using static Jypeli.Physics.Collision;

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
		int Width;
		int Heigth;
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
		Shape healthBar; // WIP

		// Images
		Image line = LoadImage("line"); // Tee oikeasti muoto myöhemmin
		Image shipImage = LoadImage("ship"); // Miksei esimerkeissä käytetä tiedostopäätettä?? https://github.com/Jypeli-JYU/Jypeli/blob/master/Examples/Koripallo/Koripallo.cs

		// Objects
		PhysicsObject ship;
		PhysicsObject shipAimLine;
		PlasmaCannon plasmaCannon;
		PhysicsObject enemy;
		PhysicsObject bullet;
		PhysicsObject shipRadius;

		// Logic 
		RandomMoverBrain logic;
		FollowerBrain approachLogic;

		// Calculations
		private double a;
		private double b;
		private double c;
		private double f;
		private double t;
		
		private double angle;
		// Difficulty
		Dictionary<String, int> difficulties = new Dictionary<string, int>();
		String difficulty = "medium";

		// Misc
		PhysicsObject _; // väliaikainen
		Vector direction;
		Random random = new Random();
		Particle fire = new Particle();
		
		// Game variables
		private int SPEED = 1250;
		
		public override void Begin()
		{

			// Score
			scoreText = new Label("test");
			Add(scoreText);

			// Health
			healthText = new Label("" + health);
			healthText.Position = new Vector(Convert.ToInt32((Screen.Size.X - Screen.Size.X * 1.5) * -1) - margin - fontSize, Convert.ToInt32((Screen.Size.Y - Screen.Size.Y * 1.5)) + margin + fontSize);
			healthText.Color = Color.White;
			healthText.TextColor = Color.Red;
			Add(healthText);

			// Difficult
			difficulties.Add("easy", 50);
			difficulties.Add("medium", 150);
			difficulties.Add("hard", 300);

			// Timer & Creating a player
			Add(CreatePlayerRadius());
			Add(CreatePlayer(50, 80));
			Jypeli.Timer.CreateAndStart(3, AddEnemy);
			Jypeli.Timer.CreateAndStart(0.02, UpdatePlayerRadius);
			

			// Collision
			AddCollisionHandler(ship, PlayerCollision);
			AddCollisionHandler(shipRadius, AimAssist);

			// Cannon
			plasmaCannon = new PlasmaCannon(10,10);
			plasmaCannon.Color = Color.Transparent;
			plasmaCannon.AmmoIgnoresGravity = false;
			plasmaCannon.Angle = plasmaCannon.Angle + Angle.FromDegrees(90);
			plasmaCannon.FireRate = 15;
			ship.Add(plasmaCannon);
			
			// Keybinds
			PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
			Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
			Keyboard.Listen(Key.S, ButtonState.Down, Down, "Down");
			Keyboard.Listen(Key.W, ButtonState.Down, Up, "Up");
			Keyboard.Listen(Key.A, ButtonState.Down, Left, "Left");
			Keyboard.Listen(Key.D, ButtonState.Down, Right, "Right");
			Keyboard.Listen(Key.F, ButtonState.Down, Shoot, "Shoot");
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

		public PhysicsObject PlayerLine() // A line for the player to see where they are about to shoot (not used)
		{
			shipAimLine = new PhysicsObject(3, 512);
			shipAimLine.Shape = Shape.Rectangle;
			shipAimLine.IgnoresCollisionResponse = true;
			shipAimLine.Tag = "line";
			return shipAimLine;
		}

		public PhysicsObject CreatePlayerRadius()
		{
			shipRadius = new PhysicsObject(350,350);
			shipRadius.IgnoresCollisionResponse = true;
			shipRadius.Shape = Shape.Circle;
			shipRadius.CollisionIgnoreGroup = 2;
			shipRadius.Tag = "PlayerRadius";
			return shipRadius;
		}

		public void UpdatePlayerRadius()
		{
			shipRadius.Color = Color.BloodRed;
			shipRadius.Position = new Vector(ship.X,ship.Y);
			shipRadius.Angle = ship.Angle;
		}

		public PhysicsObject CreateEnemy(PhysicsObject ship)
		{
			enemy = new PhysicsObject(30, 30);
			enemy.Shape = Shape.Circle;
			enemy.Color = Color.Red;
			enemy.X = random.Next(0, (int)800); // TODO: set as the screen width
			enemy.Y = random.Next(0, (int)600); // TODO: set as the screen height
			enemy.Brain = AddSmartLogic(ship);
			enemy.CollisionIgnoreGroup = 1;
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
			approachLogic.Speed = GetDifficulty();
			approachLogic.DistanceFar = 600;
			approachLogic.DistanceClose = 200;
			approachLogic.FarBrain = AddDumbLogic();
			approachLogic.TargetClose += EnemyCloseEvent;
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
				//collider.Destroy();
			}
			healthText.Text = "" + health;
		}

		void AimAssist(PhysicsObject collider, PhysicsObject target)
		{
			a = target.X - ship.X;
			b = target.Y - ship.Y;
			c = Math.Sqrt(a * a + b * b);
			angle = Math.Atan(b / a);
			if(a>0)
			{
				angle = angle * (180 / Math.PI);
			}
			else if(a<=0)
			{
				angle = angle * (180 / Math.PI) + 180;
			}
			ship.ApplyTorque(angle/1000) ;
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
				bullet.Collided += BulletCollision;
			}
		}

		public int GetDifficulty()
		{
			return 150;
		}


		void AddEnemy()
		{
			Add(CreateEnemy(ship));
		}

		void Down()
		{
			direction = Vector.FromLengthAndAngle(SPEED * -1, ship.Angle + Angle.FromDegrees(90));
			ship.Push(direction);
		}
		void Up()
		{
			direction = Vector.FromLengthAndAngle(SPEED, ship.Angle + Angle.FromDegrees(90));
			ship.Push(direction);
		}
		void Left()
		{
			ship.ApplyTorque(0.015);
		}
		void Right()
		{
			ship.ApplyTorque(-0.015);
		}



	}
}