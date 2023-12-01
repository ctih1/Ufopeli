using FarseerPhysics.Common;
using Jypeli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ufopeli
{
	public class Movement
	{
		private PhysicsObject ship;
		private int SPEED=1250;
		private Vector direction;
		public void init(PhysicsObject ship)
		{
			this.ship = ship;
		}

		public void Down()
		{
			direction = Vector.FromLengthAndAngle(SPEED * -1, ship.Angle + Angle.FromDegrees(90));
			ship.Push(direction);
		}
		public void Up()
		{
			direction = Vector.FromLengthAndAngle(SPEED, ship.Angle + Angle.FromDegrees(90));
			ship.Push(direction);
		}
		public void Left()
		{ ship.ApplyTorque(0.001); }
		public void Right()
		{ ship.ApplyTorque(-0.001); }

	}
}
