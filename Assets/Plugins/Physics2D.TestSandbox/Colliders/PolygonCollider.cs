using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Physics2D
{
	public class PolygonCollider : BaseCollider
	{
		public float2[] positions;

		public override void AddToScene(PhysicsWorld scene)
		{
			if (positions == null)
				return;

			if (positions.Length < 3)
				return;

			scene.AddRigidBody(Position, Orientation, Velocity, Mass, IsStatic);
			scene.AddPolygonCollider(positions);
		}

		public override void DrawColliderGizmo()
		{
			if (positions == null)
				return;

			var pCount = positions.Length;
			if (pCount < 3)
				return;

			for (int i = 0; i < pCount - 1; i++)
			{
				Gizmos.DrawLine(
					ConvertTo3D(positions[i]),
					ConvertTo3D(positions[i + 1])
				);
			}

			Gizmos.DrawLine(
				ConvertTo3D(positions[pCount - 1]),
				ConvertTo3D(positions[0])
			);
		}
	}
}