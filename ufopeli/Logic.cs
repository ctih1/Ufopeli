using Jypeli;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ufopeli
{
	public class Logic
	{
		private FollowerBrain approachLogic;
		private RandomMoverBrain logic;

		private RandomMoverBrain AddDumbLogic()
		{
			logic = new RandomMoverBrain(200);
			logic.ChangeMovementSeconds = 3;
			return logic;
		}

		public FollowerBrain AddSmartLogic(PhysicsObject target)
		{
			approachLogic = new FollowerBrain(target);
			approachLogic.Speed = ufopeli.GetDifficulty();
			approachLogic.DistanceFar = 600;
			approachLogic.DistanceClose = 200;
			approachLogic.FarBrain = AddDumbLogic();
			approachLogic.TargetClose += ufopeli.EnemyCloseEvent;
			return approachLogic;
		}
	}
}
