using System;
using Game.Simulation.Board;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation
{
	public class Projectile : BattleObject
	{
		private const float HomingAmount = 10f;

		Vector3 position;
		Vector3 velocity;
		private readonly BattleObject target;

		public Vector3 Position => position;
		public Vector3 Velocity => velocity;

		public override string ViewPath => $"View/ProjectileViews/Projectile";

		public Projectile(BattleObject parent, Vector3 position, Vector3 velocity, BattleObject target) : base(parent.GameWorld, parent.Owner, parent)
		{
			this.position = position;
			this.velocity = velocity;
			this.target = target;
		}

		public void Tick()
		{
			var newPosition = position + velocity * GameTick.TickDuration;
			if (target != null)
			{
				var currentDirection = velocity.normalized;
				var targetDirection = (target.GetPosition3D() - position).normalized;
				var newDirection =
					Vector3.Lerp(currentDirection, targetDirection, HomingAmount * GameTick.TickDuration);
				velocity = velocity.magnitude * newDirection;
			}
			else
			{
				velocity.y -= 9.81f * GameTick.TickDuration;
			}

			position = newPosition;
			if (position.y < 0)
			{
				Deactivate();
			}
		}

		public override float2 GetPosition2D()
		{
			return new float2(position.x, position.z);
		}

		public override Vector3 GetPosition3D()
		{
			return position;
		}
	}
}