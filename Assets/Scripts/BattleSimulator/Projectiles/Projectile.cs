using System;
using Game.Simulation.Board;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation
{
	public class Projectile : BattleObject
	{
		private const float HomingAmount = 20f;

		Vector3 position;
		Vector3 velocity;
		private readonly Unit target;
		private readonly float damage;

		public Vector3 Position => position;
		public Vector3 Velocity => velocity;

		public override string ViewPath => $"View/ProjectileViews/Projectile";

		public Projectile(BattleObject parent, Vector3 position, Vector3 velocity, Unit target, float damage) : base(parent.GameWorld, parent.Owner, parent)
		{
			this.position = position;
			this.velocity = velocity;
			this.target = target;
			this.damage = damage;
		}

		public void Tick()
		{
			var newPosition = position + velocity * GameTick.TickDuration;
			if (target != null)
			{
				var targetPosition = target.GetPosition3D() + target.Settings.Height * 0.5f * Vector3.up;
				var targetDirection = (targetPosition - position).normalized;
				velocity += targetDirection * HomingAmount * GameTick.TickDuration;
			}
			else
			{
				velocity.y -= 9.81f * GameTick.TickDuration;
			}

			position = newPosition;
			if (target != null)
			{
				var targetPosition = target.GetPosition3D() + target.Settings.Height * 0.5f * Vector3.up;
				var vectorToTarget = targetPosition - position;
				if (vectorToTarget.magnitude < target.Radius || Vector3.Dot(vectorToTarget, velocity) < 0)
				{
					target.DealDamage(damage, this);
					Deactivate();
				}
			}
			else
			{
				if (position.y <= 0)
				{
					Deactivate();
				}
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