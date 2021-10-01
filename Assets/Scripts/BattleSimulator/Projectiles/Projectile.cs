using System;
using Game.Simulation.Board;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation
{
	public class Projectile : BattleObject
	{
		Vector3 position;
		Vector3 velocity;

		public Vector3 Position => position;
		public Vector3 Velocity => velocity;

		public override string ViewPath => $"View/ProjectileViews/Projectile";

		public Projectile(BattleObject parent, Vector3 position, Vector3 velocity) : base(parent.GameWorld, parent.Owner, parent)
		{
			this.position = position;
			this.velocity = velocity;
		}

		public void Tick()
		{
			var newPosition = position + velocity * GameTick.TickDuration;
			velocity.y -= 9.81f * GameTick.TickDuration;
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