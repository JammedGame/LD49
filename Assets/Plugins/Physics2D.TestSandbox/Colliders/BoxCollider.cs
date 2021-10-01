using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Physics2D
{
	public class BoxCollider : BaseCollider
	{
		public float2 size;

		public override void AddToScene(PhysicsWorld scene)
		{
			scene.AddRigidBody(Position, Orientation, Velocity, Mass, IsStatic);

			scene.AddEdgeCollider(new float2(-size.x, -size.y) / 2, new float2(-size.x,  size.y) / 2);
			scene.AddEdgeCollider(new float2(-size.x,  size.y) / 2, new float2( size.x,  size.y) / 2);
			scene.AddEdgeCollider(new float2( size.x,  size.y) / 2, new float2( size.x, -size.y) / 2);
			scene.AddEdgeCollider(new float2( size.x, -size.y) / 2, new float2(-size.x, -size.y) / 2);
		}

		public override void DrawColliderGizmo()
		{
			Gizmos.DrawWireCube(default, new Vector3(size.x, size.y, 0.1f));
		}
	}
}