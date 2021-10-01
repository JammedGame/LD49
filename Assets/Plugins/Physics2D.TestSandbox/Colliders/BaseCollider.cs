using System;
using Physics2D;
using Unity.Mathematics;
using UnityEngine;

namespace Physics2D
{
	public abstract class BaseCollider : MonoBehaviour
	{
		public bool IsStatic;
		public float2 Velocity;
		public float Mass;
		public float Orientation;

		public float2 Position
		{
			get => new float2(transform.localPosition.x, transform.localPosition.y);
			set => transform.localPosition = new Vector3(value.x, value.y, 0);
		}

		public abstract void AddToScene(PhysicsWorld world);

		public void ApplyNewPosition(float2 newPosition, float2 newVelocity)
		{
			Position = newPosition;
			Velocity = newVelocity;
		}

		public Vector3 ConvertTo3D(float2 point)
		{
			return new Vector3(point.x, point.y, 0);
		}

		public void DrawGizmos()
		{
			Gizmos.matrix = Matrix4x4.TRS(transform.localPosition, Quaternion.Euler(0, 0, Orientation), Vector3.one);
			DrawColliderGizmo();
		}

		public abstract void DrawColliderGizmo();
	}
}