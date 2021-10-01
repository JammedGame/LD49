using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Physics2D
{
	public class EdgeCollider : BaseCollider
	{
		public List<float2> positions;

		public override void AddToScene(PhysicsWorld scene)
		{
			if (positions.Count < 2)
				return;

			scene.AddRigidBody(Position, Orientation, Velocity, Mass, IsStatic);

			for (int i = 0; i < positions.Count - 1; i++)
			{
				scene.AddEdgeCollider(positions[i], positions[i + 1]);
			}
		}

		public override void DrawColliderGizmo()
		{
			if (positions.Count < 2)
				return;

			for (int i = 0; i < positions.Count - 1; i++)
			{
				Gizmos.DrawLine(
					ConvertTo3D(positions[i]),
					ConvertTo3D(positions[i + 1])
				);
			}
		}
	}
}