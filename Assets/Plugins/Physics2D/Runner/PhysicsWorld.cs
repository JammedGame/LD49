using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using System;
using System.Collections.Generic;
using static Unity.Mathematics.math;
using float2 = Unity.Mathematics.float2;

namespace Physics2D
{
	public class PhysicsWorld
	{
		// rigid bodies
		NativeList<Transform> transforms;
		NativeList<float2> velocities;
		NativeList<float> masses;
		NativeList<bool> isStatic;

		// colliders
		NativeList<Collider> colliders;
		NativeList<float2> colliderData;

		// transient data
		NativeList<AABB> aabbBuffer;
		NativeList<int2> contacts;
		NativeList<Manifold> manifolds;
		NativeList<PositionVelocityChange> positionVelocityChanges;

		public PhysicsWorld()
		{
			transforms = new NativeList<Transform>(Allocator.Persistent);
			velocities = new NativeList<float2>(Allocator.Persistent);
			colliders = new NativeList<Collider>(Allocator.Persistent);
			contacts = new NativeList<int2>(Allocator.Persistent);
			aabbBuffer = new NativeList<AABB>(Allocator.Persistent);
			manifolds = new NativeList<Manifold>(Allocator.Persistent);
			positionVelocityChanges = new NativeList<PositionVelocityChange>(Allocator.Persistent);
			colliderData = new NativeList<float2>(Allocator.Persistent);
			masses = new NativeList<float>(Allocator.Persistent);
			isStatic = new NativeList<bool>(Allocator.Persistent);
		}

		public void Reset()
		{
			transforms.Clear();
			velocities.Clear();
			masses.Clear();
			colliders.Clear();
			colliderData.Clear();
			aabbBuffer.Clear();
			contacts.Clear();
			manifolds.Clear();
			isStatic.Clear();
			positionVelocityChanges.Clear();
		}

		public void AddRigidBody(float2 position, float orientation, float2 velocity, float mass, bool isStatic)
		{
			if (mass <= 0)
			{
				Debug.LogError($"Tried to add collider with mass {mass} - ignored.");
				return;
			}

			this.transforms.Add(new Transform(position, orientation));
			this.velocities.Add(velocity);
			this.masses.Add(mass);
			this.isStatic.Add(isStatic);
		}

		public void AddStaticBody(float2 position, float orientation)
		{
			AddRigidBody(position, orientation, default, float.MaxValue, true);
		}

		public void AddEdgeCollider(float2 edgeStart, float2 edgeEnd)
		{
			// add collider
			colliders.Add(new Collider()
			{
				Type = ColliderType.Edge,
				BodyId = GetActiveBodyId(),
				DataStartIndex = colliderData.Length,
				DataLength = 2
			});

			// add collider data
			colliderData.Add(edgeStart); // offset to origin
			colliderData.Add(edgeEnd); // radius
		}

		public void AddCircleCollider(float radius, float2 offsetToBody = default)
		{
			// add collider
			colliders.Add(new Collider()
			{
				Type = ColliderType.Circle,
				BodyId = GetActiveBodyId(),
				DataStartIndex = colliderData.Length,
				DataLength = 2
			});

			// add collider data
			colliderData.Add(offsetToBody);
			colliderData.Add(radius);
		}

		public void AddBoxCollider(float2 center, float2 size)
		{
			// add collider
			colliders.Add(new Collider()
			{
				Type = ColliderType.Polygon,
				BodyId = GetActiveBodyId(),
				DataStartIndex = colliderData.Length,
				DataLength = 4
			});

			// add collider data
			colliderData.Add(new float2(center.x - size.x, center.y - size.y));
			colliderData.Add(new float2(center.x - size.x, center.y + size.y));
			colliderData.Add(new float2(center.x + size.x, center.y + size.y));
			colliderData.Add(new float2(center.x + size.x, center.y - size.y));
		}

		public void AddPolygonCollider(float2[] points)
		{
			// add collider
			colliders.Add(new Collider()
			{
				Type = ColliderType.Polygon,
				BodyId = GetActiveBodyId(),
				DataStartIndex = colliderData.Length,
				DataLength = points.Length
			});

			// add collider data
			for (int i = 0; i < points.Length; i++)
				colliderData.Add(points[i]);
		}

		public void AddPolygonCollider(List<float2> points, int startIndex, int length)
		{
			// add collider
			colliders.Add(new Collider()
			{
				Type = ColliderType.Polygon,
				BodyId = GetActiveBodyId(),
				DataStartIndex = colliderData.Length,
				DataLength = length
			});

			// add collider data
			for (int i = 0; i < length; i++)
				colliderData.Add(points[startIndex + i]);
		}

		/// <summary>
		/// If composite collider is in progress, we will mark collider-parts with same id so they can be handled specially.
		/// </summary>
		private int GetActiveBodyId()
		{
			return transforms.Length - 1;
		}

		public void Tick(PhysicsSettings settings, float tickDuration)
		{
			var dT = tickDuration / settings.Passes;
			var velocityDragConstant = settings.VelocityDragConstant * dT;
			var velocityDragRelative = math.pow(settings.VelocityDragRelative, 1f / settings.Passes);

			for (int i = 0; i < settings.Passes; i++)
			{
				new UpdateVelocitiesJob()
				{
					dT = dT,
					VelocityDragConstant = velocityDragConstant,
					VelocityDragRelative = velocityDragRelative,

					transforms = transforms,
					velocities = velocities,
				}
				.Run(transforms.Length);

				Resize(ref aabbBuffer, colliders.Length);
				new CalculateAABBJob()
				{
					transforms = transforms,
					colliders = colliders,
					colliderData = colliderData, // =>
					aabbBuffer = aabbBuffer,
				}.
				Run(aabbBuffer.Length);

				contacts.Clear();
				new BroadPhaseJob()
				{
					isStatic = isStatic,
					colliders = colliders,
					aabbBuffer = aabbBuffer, // =>
					contacts = contacts,
				}.
				Run();

				Resize(ref manifolds, contacts.Length);
				new NarrowPhaseJob()
				{
					colliders = colliders,
					colliderData = colliderData,
					transforms = transforms,
					contacts = contacts, // =>
					manifolds = manifolds
				}.
				Run(contacts.Length);

				Resize(ref positionVelocityChanges, contacts.Length);
				new SolveManifoldsJob()
				{
					BounceAbsorbConstant = settings.BounceAbsorbConstant,
					BounceAbsorbRelative = settings.BounceAbsorbRelative,

					masses = masses,
					colliders = colliders,
					velocities = velocities,
					contacts = contacts,
					isStatic = isStatic,
					manifolds = manifolds, // =>
					positionVelocityChanges = positionVelocityChanges
				}.
				Run(manifolds.Length);

				new ApplyChangesJob()
				{
					positionVelocityChanges = positionVelocityChanges, // =>
					transforms = transforms,
					velocities = velocities
				}.
				Run();
			}
		}

		public void Dispose()
		{
			if (contacts.IsCreated) contacts.Dispose();
			if (transforms.IsCreated) transforms.Dispose();
			if (velocities.IsCreated) velocities.Dispose();
			if (colliders.IsCreated) colliders.Dispose();
			if (aabbBuffer.IsCreated) aabbBuffer.Dispose();
			if (manifolds.IsCreated) manifolds.Dispose();
			if (colliderData.IsCreated) colliderData.Dispose();
			if (masses.IsCreated) masses.Dispose();
			if (positionVelocityChanges.IsCreated) positionVelocityChanges.Dispose();
			if (isStatic.IsCreated) isStatic.Dispose();
		}

		public void Resize<T>(ref NativeList<T> list, int size) where T : unmanaged
		{
			list.Clear();
			list.Resize(size, NativeArrayOptions.ClearMemory);
		}

		// API
		public bool HasBroadPhaseContact(int bodyId)
		{
			if (!contacts.IsCreated)
				return false;

			for (int i = 0; i < contacts.Length; i++)
			{
				if (colliders[contacts[i].x].BodyId == bodyId
				 || colliders[contacts[i].y].BodyId == bodyId)
					return true;
			}

			return false;
		}

		public (float2 position, float2 velocity) GetPositionAndVelocity(int i)
		{
			return (transforms[i].Position, velocities[i]);
		}

		/// <summary>
		/// Calculates total kinetic energy of all elements of the physics world.
		/// </summary>
		public float CalcualteKineticEnergy()
		{
			var energy = 0f;

			for (int i = 0; i < velocities.Length; i++)
			{
				var v2 = lengthsq(velocities[i]);
				var m = masses[i];
				energy += m * v2 * 0.5f;
			}

			return energy;
		}

		/// <summary>
		/// Offset given circle position/radius with exisitng static colliders, and returns an updated position.
		/// </summary>
		public float2 CollideWithStatic(float2 circlePosition, float circleRadius)
		{
			var aabb = new AABB(circlePosition - circleRadius, circlePosition + circleRadius);
			var colliderCount = aabbBuffer.Length;
			var offset = float2.zero;
			var colliderData = this.colliderData.AsArray();

			for (int i = 0; i < colliderCount; i++)
			{
				var collider = colliders[i];
				if (!isStatic[collider.BodyId])
					continue;
				if (!AABB.TestIntersection(aabb, aabbBuffer[i]))
					continue;

				var transformRight = transforms[collider.BodyId];
				var colliderDataRight = colliderData.Slice(collider.DataStartIndex, collider.DataLength);

				switch (collider.Type)
				{
					case ColliderType.Circle:
					{
						var position2 = transformRight.TransformPoint(colliderDataRight[0]);
						var radius2 = colliderDataRight[1].x;
						offset += CollisionUtil.Circle2Circle(circlePosition, circleRadius, position2, radius2).offset;
						break;
					}

					case ColliderType.Polygon:
					{
						offset += CollisionUtil.Circle2Polygon(circlePosition, circleRadius, transformRight, colliderDataRight).offset;
						break;
					}

					case ColliderType.Edge:
					{
						var edgeStart = transformRight.TransformPoint(colliderDataRight[0]);
						var edgeEnd = transformRight.TransformPoint(colliderDataRight[1]);
						offset += CollisionUtil.Circle2Edge(circlePosition, circleRadius, edgeStart, edgeEnd).offset;
						break;
					}

					default:
						throw new NotImplementedException();
				}
			}

			return circlePosition + offset * 1.1f;
		}
	}
}