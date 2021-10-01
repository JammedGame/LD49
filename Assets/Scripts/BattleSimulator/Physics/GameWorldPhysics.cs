using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using float2 = Unity.Mathematics.float2;
using Physics2D;

namespace Game.Simulation.Physics
{
	public class GameWorldPhysics
	{
		public readonly PhysicsWorld physicsWorld = new PhysicsWorld();

		public void Run(GameWorld gameWorld, float dT)
		{
			// fetch buffers for the physics job
			var unitList = gameWorld.AllUnits;
			var unitCount = unitList.Count;
			var settings = gameWorld.Data.PhysicsSettings;

			physicsWorld.Reset();

			// make colliders from units.
			foreach (var unit in unitList)
			{
				physicsWorld.AddRigidBody(unit.Position, unit.Orientation, unit.Velocity, unit.Radius, unit.IsStatic);
				physicsWorld.AddCircleCollider(unit.Radius);
			}

			// run the physics
			physicsWorld.Tick(gameWorld.Data.PhysicsSettings, dT);

			// apply values back to the units
			for (int i = 0; i < unitList.Count; i++)
			{
				Unit unit = unitList[i];
				if (unit.IsStatic)
					continue;

				var (newPosition, newVelocity) = physicsWorld.GetPositionAndVelocity(i);
				unit.MoveToPosition(newPosition);
				unit.Velocity = newVelocity;
			}
		}

		public void Dispose()
		{
			physicsWorld?.Dispose();
		}

		/// <summary>
		/// Run one-off collision of a circle with static environment. Returns position of the circle that is free.
		/// </summary>
		public float2 GetClosestFreePosition(float2 circlePosition, float circleRadius)
		{
			return physicsWorld.CollideWithStatic(circlePosition, circleRadius);
		}
	}

	/*
		readonly List<(Unit left, Unit right)> dispatchQueue = new List<(Unit left, Unit right)>();
		public void DispatchCollisionEvents(NativeArray<int2> events, List<Unit> objects)
		{
			try
			{
				foreach(var evt in events)
				{
					var left = evt.x >= 0 && evt.x < objects.Count ? objects[evt.x] : null;
					if (left == null) continue;

					var right = evt.y >= 0 && evt.y < objects.Count ? objects[evt.x] : null;
					dispatchQueue.Add((left, right));
				}

				foreach(var evt in dispatchQueue)
				{
					if (evt.right != null)
					{
						evt.left.OnCollisionWith(evt.right);
						evt.right.OnCollisionWith(evt.left);
					}
					else
					{
						evt.left.OnCollisionWithWall();
					}
				}
			}
			finally
			{
				dispatchQueue.Clear();
			}
		}
	*/
}