using Unity.Mathematics;
using UnityEngine;

namespace Physics2D
{
	public class CircleCollider : BaseCollider
	{
		public float Radius;
		public float2 CircleOffset;

		public override void AddToScene(PhysicsWorld world)
		{
			world.AddRigidBody(Position, Orientation, Velocity, Mass, IsStatic);
			world.AddCircleCollider(Radius, CircleOffset);
		}

		public override void DrawColliderGizmo()
		{
			Gizmos.DrawWireSphere(ConvertTo3D(CircleOffset), Radius);
		}
	}
}